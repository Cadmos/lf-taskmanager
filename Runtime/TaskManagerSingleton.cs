using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LF.TaskManager.Runtime
{
    public enum TaskQueuePriority : int
    {
        Critical = 0,
        High = 1,
        Normal = 2,
        Low = 3,
        Lowest = 4
    }

    public class TaskManagerSingleton : MonoBehaviour
    {
        private ConcurrentQueue<Func<Task>>[] _taskQueues;
        [SerializeField] private int _priorityLevels;
        [SerializeField] private int _workerCount;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public static TaskManagerSingleton Instance { get; private set; }
        [SerializeField] private bool _isInitialized;

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

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            ClearAllQueues();
        }

        #endregion

        private bool Initialize()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            _priorityLevels = Enum.GetValues(typeof(TaskQueuePriority)).Length;
            _taskQueues = new ConcurrentQueue<Func<Task>>[_priorityLevels];

            for (int i = 0; i < _priorityLevels; i++)
            {
                _taskQueues[i] = new ConcurrentQueue<Func<Task>>();
            }

            // Set the number of worker tasks; adjust as needed
            _workerCount = Environment.ProcessorCount;

            StartProcessingTasks();
            return true;
        }

        private void StartProcessingTasks()
        {
            for (int i = 0; i < _workerCount; i++)
            {
                Task.Factory.StartNew(ProcessTasksAsync, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        private void ClearAllQueues()
        {
            foreach (var queue in _taskQueues)
            {
                while (queue.TryDequeue(out _)) { }
            }
        }

        public void CancelAllTasks()
        {
            ClearAllQueues();
        }

        /// <summary>
        /// Enqueues a task to be processed with the specified priority.
        /// </summary>
        /// <param name="taskFunc">The asynchronous function representing the task.</param>
        /// <param name="priority">The priority level of the task.</param>
        /// <returns>A Task that completes when the enqueued task has been processed.</returns>
        public Task EnqueueTask(Func<Task> taskFunc, TaskQueuePriority priority)
        {
            var tcs = new TaskCompletionSource<bool>();

            _taskQueues[(int)priority].Enqueue(async () =>
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

            return tcs.Task;
        }

        private async Task ProcessTasksAsync()
        {
            int idleDelay = 1000; // Start with a 1-second delay
            int maxIdleDelay = 5000; // Maximum delay of 5 seconds
            int delayIncrement = 1000; // Increment delay by 1 second each time

            while (!_cancellationToken.IsCancellationRequested || destroyCancellationToken.IsCancellationRequested)
            {
                bool taskProcessed = false;

                // Check queues starting from highest priority
                for (int i = 0; i < _priorityLevels; i++)
                {
                    if (_taskQueues[i].TryDequeue(out var taskToRun))
                    {
                        try
                        {
                            await taskToRun();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error processing task in priority {i}: {ex.Message}");
                        }

                        taskProcessed = true;
                        // Reset idle delay after processing a task
                        idleDelay = 1000; // Reset to initial delay
                        break;
                    }
                }

                if (!taskProcessed)
                {
                    // Wait for the current idle delay
                    await Task.Delay(idleDelay);

                    // Increase the idle delay for next time, up to the maximum
                    idleDelay = Math.Min(idleDelay + delayIncrement, maxIdleDelay);
                }
            }
        }
    }
}
