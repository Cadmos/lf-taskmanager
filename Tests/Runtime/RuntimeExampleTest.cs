using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LF.TaskManager.Runtime;
using System.Collections;

namespace LF.TaskManager.Tests
{
    public class RuntimeExampleTest
    {
        [SetUp]
        public void Setup()
        {
            // Ensure the TaskManagerSingleton instance is created before each test
            if (TaskManagerSingleton.Instance == null)
            {
                // Manually create the GameObject if no instance exists
                var taskManagerGO = new GameObject("TaskManagerSingleton");
                taskManagerGO.AddComponent<TaskManagerSingleton>();
                Object.DontDestroyOnLoad(taskManagerGO);
            }
        }


        [TearDown]
        public void Teardown()
        {
            // Clean up after each test
            if (TaskManagerSingleton.Instance != null)
            {
                Object.DestroyImmediate(TaskManagerSingleton.Instance.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator TaskManager_ShouldExecuteSimpleTask()
        {
            bool taskExecuted = false;

            // Add a simple task that sets taskExecuted to true
            TaskManagerSingleton.Instance.AddTask(() =>
            {
                taskExecuted = true;
            });

            // Wait for one frame to allow the task to be processed
            yield return null;

            // Assert that the task was executed
            Assert.IsTrue(taskExecuted, "The task was not executed as expected.");
        }

        [UnityTest]
        public IEnumerator TaskManager_ShouldExecuteDelayedTask_AfterDelay()
        {
            bool delayedTaskExecuted = false;

            // Add a delayed task that sets delayedTaskExecuted to true after 2 seconds
            TaskManagerSingleton.Instance.AddDelayedTask(() =>
            {
                delayedTaskExecuted = true;
            }, 2f);

            // Wait for 1 second (task should not have executed yet)
            yield return new WaitForSeconds(1f);
            Assert.IsFalse(delayedTaskExecuted, "The delayed task executed too early.");

            // Wait for 2 more seconds (task should have executed now)
            yield return new WaitForSeconds(2f);
            Assert.IsTrue(delayedTaskExecuted, "The delayed task was not executed after the expected delay.");
        }

        [UnityTest]
        public IEnumerator TaskManager_ShouldCancelAllTasks()
        {
            bool taskExecuted = false;

            // Add a task that should be canceled
            TaskManagerSingleton.Instance.AddTask(() =>
            {
                taskExecuted = true;
            });

            // Cancel all tasks before they are executed
            TaskManagerSingleton.Instance.CancelAllTasks();

            // Wait for one frame to allow the task to be processed (but it should have been canceled)
            yield return null;

            // Assert that the task was not executed
            Assert.IsFalse(taskExecuted, "The task was executed despite being canceled.");
        }

        [UnityTest]
        public IEnumerator TaskManager_ShouldHandleMultipleTasks()
        {
            int taskExecutionCount = 0;

            // Add multiple tasks
            TaskManagerSingleton.Instance.AddTask(() => taskExecutionCount++);
            TaskManagerSingleton.Instance.AddTask(() => taskExecutionCount++);
            TaskManagerSingleton.Instance.AddTask(() => taskExecutionCount++);

            // Wait for one frame to allow the tasks to be processed
            yield return null;

            // Assert that all tasks were executed
            Assert.AreEqual(3, taskExecutionCount, "Not all tasks were executed as expected.");
        }
    }
}
