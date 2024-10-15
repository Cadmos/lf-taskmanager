using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LF.TaskManager.Runtime;
using System.Collections;
using System.Threading.Tasks;
using LF.ManagerCore.Runtime;

namespace LF.TaskManager.Tests
{
    public class RuntimeExampleTest
    {
        [SetUp]
        public void Setup()
        {
            // Ensure the TaskManagerSingleton instance is created before each test
            if (ManagerBase.GetInstance<TaskManagerSingleton>() == null)
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
            if (ManagerBase.GetInstance<TaskManagerSingleton>() != null)
            {
                Object.DestroyImmediate(TaskManagerSingleton.Instance.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator TaskManager_ShouldExecuteSimpleTask()
        {
            bool taskExecuted = false;

            // Add a simple task that sets taskExecuted to true
            ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                taskExecuted = true;
                await Task.CompletedTask;
            }, TaskQueuePriority.Normal);

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
            ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                await Task.Delay(2000);
                delayedTaskExecuted = true;
            }, TaskQueuePriority.Normal);

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
            ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                taskExecuted = true;
                await Task.CompletedTask;
            }, TaskQueuePriority.Normal);

            // Cancel all tasks before they are executed
            ManagerBase.GetInstance<TaskManagerSingleton>().CancelAllTasks();

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
            ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                taskExecutionCount++;
                await Task.CompletedTask;
            }, TaskQueuePriority.Normal);
            ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                taskExecutionCount++;
                await Task.CompletedTask;
            }, TaskQueuePriority.Normal);
            ManagerBase.GetInstance<TaskManagerSingleton>().EnqueueTask(async () =>
            {
                taskExecutionCount++;
                await Task.CompletedTask;
            }, TaskQueuePriority.Normal);

            // Wait for one frame to allow the tasks to be processed
            yield return null;

            // Assert that all tasks were executed
            Assert.AreEqual(3, taskExecutionCount, "Not all tasks were executed as expected.");
        }
    }
}