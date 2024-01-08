using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Heatmap : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    [MenuItem("Window/Heatmap   ")]
    private static void HeatmapMenuItem()
    {
        EditorWindow.GetWindow(typeof(Heatmap));
    }

    private void OnGUI()
    {
        //GUILayout.Label ("Test", EditorStyles.boldLabel);
        //myString = EditorGUILayout.TextField("Text Field", myString);

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("Load"))
        {
            Debug.Log("Loading data!");
        }
    }
}
