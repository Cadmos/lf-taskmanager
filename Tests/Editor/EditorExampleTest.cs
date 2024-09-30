using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

namespace LF.TaskManager.Editor.Tests
{
    public class EditorExampleTest
    {
        private const string TaskManagerName = "TaskManagerSingleton";
        private const string TestScenePath = "Assets/Tests/Editor/TestScene.unity";

        [SetUp]
        public void Setup()
        {
            // Check if the scene asset exists at the specified path
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(TestScenePath);
            if (sceneAsset == null)
            {
                // Create a new scene and save it to the specified path
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(newScene, TestScenePath);
            }

            // Load the test scene
            EditorSceneManager.OpenScene(TestScenePath, OpenSceneMode.Single);
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

        [UnityTest]
        public IEnumerator MenuCommand_AddTaskManagerSingleton_CreatesGameObject()
        {
            // Ensure no TaskManagerSingleton exists before the test
            Assert.IsNull(GameObject.Find(TaskManagerName), "TaskManagerSingleton should not exist before the test.");

            // Execute the menu item that adds the TaskManagerSingleton
            EditorApplication.ExecuteMenuItem("Tools/TaskManager/Add TaskManager");

            // Wait for the editor to process the delayCall
            yield return null;

            // Check if the TaskManagerSingleton GameObject was added to the scene
            var taskManagerGo = GameObject.Find(TaskManagerName);

            // Assert that the TaskManagerSingleton GameObject was created
            Assert.IsNotNull(taskManagerGo, "TaskManagerSingleton GameObject was not created through the menu item.");

            // Check that the GameObject has the correct components
            var hasMonoBehaviour = taskManagerGo.GetComponent<MonoBehaviour>() != null;
            Assert.IsTrue(hasMonoBehaviour, "TaskManagerSingleton does not have any MonoBehaviour components.");
        }
        [UnityTest]
        public IEnumerator MenuCommand_AddTaskManagerSingleton_EnforcesSingleton()
        {
            // Ensure no TaskManagerSingleton exists before the test
            Assert.IsNull(GameObject.Find(TaskManagerName), "TaskManagerSingleton should not exist before the test.");

            // Execute the menu item to add the TaskManagerSingleton
            EditorApplication.ExecuteMenuItem("Tools/TaskManager/Add TaskManager");

            // Wait for the editor to process the first delayCall
            yield return null;

            // Ensure the TaskManagerSingleton exists
            GameObject taskManager1 = GameObject.Find(TaskManagerName);
            Assert.IsNotNull(taskManager1, "First TaskManagerSingleton GameObject was not created.");

            // Attempt to add another TaskManagerSingleton
            EditorApplication.ExecuteMenuItem("Tools/TaskManager/Add TaskManager");

            // Wait for the editor to process the second delayCall
            yield return null;

            // Count the number of TaskManagerSingleton GameObjects by name
            int count = 0;
            GameObject[] allGameObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in allGameObjects)
            {
                if (go.name == TaskManagerName)
                {
                    count++;
                }
            }

            // Assert that only one instance exists
            Assert.AreEqual(1, count, "Multiple TaskManagerSingleton instances were created. Singleton pattern is not enforced.");
        }
    }
}
