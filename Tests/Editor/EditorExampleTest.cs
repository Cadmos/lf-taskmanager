using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LF.TaskManager.Editor.Tests
{
    public class EditorExampleTest
    {
        private const string TaskManagerName = "TaskManagerSingleton";

        [SetUp]
        public void Setup()
        {
            // Clear any existing TaskManagerSingleton instance before the test
            var existingTaskManager = GameObject.Find(TaskManagerName);
            if (existingTaskManager != null)
            {
                Object.DestroyImmediate(existingTaskManager);
            }
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up after each test
            var taskManager = GameObject.Find(TaskManagerName);
            if (taskManager != null)
            {
                Object.DestroyImmediate(taskManager);
            }
        }

        [Test]
        public void AddTaskManagerSingleton_ThroughMenuCommand()
        {
            // Call the menu item that adds the TaskManagerSingleton
            EditorApplication.ExecuteMenuItem("Tools/TaskManager/Add TaskManager");

            // Check if the TaskManagerSingleton was added to the scene
            var taskManager = GameObject.Find(TaskManagerName);

            // Assert that the TaskManagerSingleton was created
            Assert.IsNotNull(taskManager, "TaskManagerSingleton was not created through the menu item.");
        }
    }
}