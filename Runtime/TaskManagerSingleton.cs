using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LF.TaskManager.Runtime
{
    public enum TaskQueuePriority
    {
        Critical,
        High,
        Normal,
        Low,
        Lowest
    }

    public class TaskManagerSingleton : MonoBehaviour
    {
        private Dictionary<TaskQueuePriority, ConcurrentQueue<Func<Task>>> _taskQueues;
        public static TaskManagerSingleton Instance { get; private set; }
        private bool _isInitialized;

        #region Unity Methods
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            if (!_isInitialized)
                _isInitialized = Initialize();
        }

        private void OnDisable()
        {
            ClearAllQueues();
        }

        private void OnDestroy()
        {
            ClearAllQueues();
        }

        #endregion

        private bool Initialize()
        {
            _taskQueues = new Dictionary<TaskQueuePriority, ConcurrentQueue<Func<Task>>>();

            foreach (TaskQueuePriority priority in Enum.GetValues(typeof(TaskQueuePriority)))
            {
                _taskQueues.Add(priority, new ConcurrentQueue<Func<Task>>());
            }

            StartProcessingTasks();
            return true;
        }

        private void StartProcessingTasks()
        {
            Task.Factory.StartNew(ProcessTasksByPriority, TaskCreationOptions.LongRunning);
        }

        private void ClearAllQueues()
        {
            foreach (var queue in _taskQueues.Values)
            {
                queue.Clear();
            }
        }
        public void CancelAllTasks()
        {
            ClearAllQueues();
        }

        public async Task EnqueueTask(Func<Task> taskFunc, TaskQueuePriority priority)
        {
            if (!_taskQueues.ContainsKey(priority))
            {
                Debug.LogError($"Task queue does not exist for priority: {priority}");
                return;
            }

            var tcs = new TaskCompletionSource<bool>();
            
            _taskQueues[priority].Enqueue(async () =>
            {
                try
                {
                    await taskFunc();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Task execution failed in {priority} queue: {ex.Message}");
                    tcs.SetException(ex);
                }
            });

            await tcs.Task;
        }

        private async Task ProcessTasksByPriority()
        {
            await Awaitable.BackgroundThreadAsync();

            while (!destroyCancellationToken.IsCancellationRequested)
            {
                bool taskProcessed = false;

                foreach (TaskQueuePriority priority in Enum.GetValues(typeof(TaskQueuePriority)))
                {
                    if (_taskQueues[priority].TryDequeue(out var taskToRun))
                    {
                        Debug.Log($"Processing task from {priority} queue.");

                        try
                        {
                            await taskToRun();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error processing task in {priority} queue: {ex.Message}");
                        }
                        
                        Debug.Log($"Finished processing task from {priority} queue.");
                        
                        taskProcessed = true;
                        break;
                    }
                }

                if (!taskProcessed)
                {
                    await Awaitable.NextFrameAsync();
                }
            }
        }
    }
}
