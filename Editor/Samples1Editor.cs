/*
using UnityEditor;
using UnityEngine;
using LF.TaskManager.Runtime;
using LF.TaskManager.Samples;

[CustomEditor(typeof(Samples1))]
public class Samples1Editor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get reference to the target object (Samples1)
        var samples1 = (Samples1)target;

        GUILayout.Space(10);
        GUILayout.Label("Task Controls", EditorStyles.boldLabel);

        // Task 1 Button and Status Box
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Run Task 1", GUILayout.Height(30)))
        {
            samples1.Task1();
        }
        DrawStatusBox(samples1.task1State);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Task 2 Button and Status Box
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Run Task 2", GUILayout.Height(30)))
        {
            samples1.Task2();
        }
        DrawStatusBox(samples1.task2State);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Task With Priority Button and Status Box
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Run Task With Priority", GUILayout.Height(30)))
        {
            samples1.TaskWithPriority(LF.TaskManager.Runtime.TaskQueuePriority.Critical, 3000);
        }
        DrawStatusBox(samples1.taskPriorityState);
        GUILayout.EndHorizontal();

        // Call default inspector for other fields
        DrawDefaultInspector();

        // Force the editor to repaint to update the status boxes
        Repaint();
    }

    // Function to draw a colored box based on task state
    private void DrawStatusBox(Samples1.TaskState state)
    {
        Color color = Color.red; // Default red (Not Running)
        if (state == Samples1.TaskState.Running)
        {
            color = Color.green; // Green for Running
        }
        else if (state == Samples1.TaskState.Completed)
        {
            color = Color.blue; // Blue for Completed
        }

        // Draw the colored box
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = EditorGUIUtility.whiteTexture;
        boxStyle.normal.textColor = Color.white;
        GUILayout.Box("", boxStyle, GUILayout.Width(20), GUILayout.Height(20));

        // Draw the background color
        var rect = GUILayoutUtility.GetLastRect();
        EditorGUI.DrawRect(rect, color);
    }
}
*/