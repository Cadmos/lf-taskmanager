using System;
using System.Collections.Generic;
using UnityEngine;

namespace LF.TaskManager.Runtime
{
    public class TaskManagerSingleton : MonoBehaviour
    {
        public static TaskManagerSingleton Instance { get; private set; }

        private Queue<Action> taskQueue = new Queue<Action>();
        private Queue<(Action task, float executeTime)> delayedTaskQueue = new Queue<(Action, float)>();
        private bool isShuttingDown = false;
        private static readonly object lockObj = new object();

        private void Awake()
        {
            lock (lockObj)
            {
                if (Instance != null && Instance != this)
                {
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void AddTask(Action task, Action onComplete = null)
        {
            lock (lockObj)
            {
                taskQueue.Enqueue(() =>
                {
                    task?.Invoke();
                    onComplete?.Invoke();
                });
            }
        }

        public void AddDelayedTask(Action task, float delay, Action onComplete = null)
        {
            lock (lockObj)
            {
                delayedTaskQueue.Enqueue((() =>
                {
                    task?.Invoke();
                    onComplete?.Invoke();
                }, Time.time + delay));
            }
        }

        private void Update()
        {
            // Process delayed tasks
            while (delayedTaskQueue.Count > 0 && Time.time >= delayedTaskQueue.Peek().executeTime)
            {
                var taskPair = delayedTaskQueue.Dequeue();
                try
                {
                    taskPair.task?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"TaskManager: Error executing delayed task: {e.Message}");
                }
            }

            // Process immediate tasks
            if (taskQueue.Count > 0)
            {
                Action task = taskQueue.Dequeue();
                try
                {
                    task?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"TaskManager: Error executing task: {e.Message}");
                }
            }
        }

        public void CancelAllTasks()
        {
            lock (lockObj)
            {
                taskQueue.Clear();
                delayedTaskQueue.Clear();
                Debug.Log("TaskManager: All tasks have been canceled.");
            }
        }

        private void OnApplicationQuit()
        {
            isShuttingDown = true;
        }

        private void OnDestroy()
        {
            if (!isShuttingDown)
            {
                Instance = null;
            }
        }
    }
}
