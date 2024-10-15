using LF.ManagerCore.Runtime;
using UnityEditor;
using UnityEngine;
using LF.TaskManager.Runtime; // Ensure this namespace is correctly referenced

namespace LF.TaskManager.Editor
{
    public class Tools
    {
        private const string MenuPath = "Tools/LF/Add TaskManager";

        /// <summary>
        /// Adds a TaskManagerSingleton to the current scene if one does not already exist.
        /// </summary>
        [MenuItem(MenuPath)]
        public static void AddTaskManager()
        {
            // Check if a TaskManagerSingleton already exists in the scene
            TaskManagerSingleton existingTaskManager = ManagerBase.GetInstance<TaskManagerSingleton>();

            if (existingTaskManager != null)
            {
                // Optionally, select the existing TaskManagerSingleton in the hierarchy
                Selection.activeGameObject = existingTaskManager.gameObject;
                EditorUtility.DisplayDialog("TaskManager Exists",
                    "A TaskManagerSingleton already exists in the scene and has been selected.", "OK");
                return;
            }

            // Create a new GameObject named "TaskManagerSingleton"
            GameObject taskManagerGO = new GameObject("TaskManagerSingleton");

            // Register the creation in the Undo system for undo support
            Undo.RegisterCreatedObjectUndo(taskManagerGO, "Create TaskManagerSingleton");

            // Add the TaskManagerSingleton component to the new GameObject
            TaskManagerSingleton taskManager = taskManagerGO.AddComponent<TaskManagerSingleton>();

            // Optionally, initialize any default settings or properties here
            InitializeTaskManager(taskManager);

            // Select the newly created TaskManagerSingleton in the hierarchy
            Selection.activeGameObject = taskManagerGO;

            // Notify the user
            EditorUtility.DisplayDialog("TaskManager Created",
                "A new TaskManagerSingleton has been added to the scene.", "OK");
        }

        /// <summary>
        /// Initializes the TaskManagerSingleton with default settings.
        /// </summary>
        /// <param name="taskManager">The TaskManagerSingleton instance to initialize.</param>
        private static void InitializeTaskManager(TaskManagerSingleton taskManager)
        {
            // Example: Initialize variables or setup default state
            // taskManager.SomeProperty = someDefaultValue;
            // Add any initialization code here as needed
        }

        /// <summary>
        /// Validates whether the menu item should be enabled.
        /// This ensures the option is only available in the Editor and not during play mode.
        /// </summary>
        /// <returns>True if the menu item should be enabled; otherwise, false.</returns>
        [MenuItem(MenuPath, true)]
        private static bool ValidateAddTaskManager()
        {
            // Disable the menu item if in play mode
            return !Application.isPlaying;
        }
    }
}
