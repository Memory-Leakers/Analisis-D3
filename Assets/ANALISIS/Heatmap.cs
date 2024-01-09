using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class Heatmap : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled = false;
    bool myBool = true;
    float myFloat = 1.23f;

    private Vector2 scrollPosition;

    ServerActionDeserialize _data;

    bool _useAllSessions = false;
    private int selectedOption = 0;
    private string[] options = { "All data", "Option 2", "Option 3" };




    [MenuItem("Window/Heatmap")]
    private static void HeatmapMenuItem()
    {
        EditorWindow.GetWindow(typeof(Heatmap));
    }

    private void OnEnable()
    {
        if (_data == null)
        {
            _data = new ServerActionDeserialize();
        }
        _data.LoadSessions();
    }

    private void OnGUI()
    {
        //GUILayout.Label("Select an option:", EditorStyles.boldLabel);

        //selectedOption = EditorGUILayout.Popup(selectedOption, options);

        //if (GUILayout.Button("Load data from "))
        //{
        //    ExecuteSelectedOption();
        //}

        ReloadDataButton();

        SessionTable();
    }

    private void SessionTable()
    {
        bool turnAll = false;
        

        if (_data != null && _data._sessions != null)
        {
            GUILayout.Label("Session List", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.BeginHorizontal();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.toggle);
            buttonStyle.normal = _useAllSessions ? GUI.skin.toggle.onActive : GUI.skin.toggle.normal;
            buttonStyle.focused = GUI.skin.toggle.normal;
            buttonStyle.onFocused = GUI.skin.toggle.normal;

            if (GUILayout.Button("", buttonStyle))
            {
                _useAllSessions = !_useAllSessions;


                turnAll = true;
            }

            GUILayout.Label("Session ID", EditorStyles.boldLabel);
            GUILayout.Label("Start Datetime", EditorStyles.boldLabel);
            GUILayout.Label("End Datetime", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            // Data rows
            foreach (ServerActionDeserialize.Session session in _data._sessions)
            {
                //if (_useAllSessions) session.active = true;
                if (turnAll && _useAllSessions) session.active = true;
                else if (turnAll && !_useAllSessions) session.active = false;
                
                session.OnGUI();
                if (!session.active) _useAllSessions = false;
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.red;
            style.fontStyle = FontStyle.Bold;

            GUILayout.Label("Loading data from database...", style);
        }
    }

    private void ExecuteSelectedOption()
    {
        // Add your logic for each option here
        switch (selectedOption)
        {
            case 0:
                Debug.Log("Executing Option 1");
                break;
            case 1:
                Debug.Log("Executing Option 2");
                break;
            case 2:
                Debug.Log("Executing Option 3");
                break;
            default:
                break;
        }
    }

    private void ReloadDataButton()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 20;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        if (GUILayout.Button("Reload", buttonStyle, GUILayout.Width(90)))
        {
            _data.LoadSessions();
        }
        GUILayout.EndHorizontal();
    }
}
