using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

public class HeatmapMenu : EditorWindow
{
    private Vector2 scrollPosition;

    ServerActionDeserialize _data;
    GameObject _heatmap;
    HeatmapObject _heatmapComponent;

    bool _useAllSessions = false;
    private int heatmapTypeSelectedOption = 0;
    
    

    [MenuItem("Window/Heatmap")]
    private static void HeatmapMenuItem()
    {
        EditorWindow.GetWindow(typeof(HeatmapMenu));
    }

    private void OnEnable()
    {
        if (_data == null)
        {
            _data = ServerActionDeserialize.Instance();
        }
        _data.LoadSessions();
    }

    private void OnDisable()
    {
        DestroyImmediate(_heatmap);
    }

    private void OnGUI()
    {
        // Reloads the session data from the ddbb
        ReloadDataButton();

        // Sessions UI
        SessionTable();

        //Slider
        GUILayout.Label("Heatmap Color Weight:", EditorStyles.boldLabel);
        if (_heatmapComponent != null)
        {
            _heatmapComponent.heatWeight = EditorGUILayout.Slider(_heatmapComponent.heatWeight, 0.01f, 1.0f);
        }

        // Filters for the type of heatmap
        HeatmapTypesUI();

        // Filters for heatmap (ONLY WORK WITH HEATMAP TYPE 'HEAT')
        EventTypeSelectorUI();

        if (GUILayout.Button("Load"))
        {
            CreateHeatmap();
        }
    }

    private void CreateHeatmap()
    {
        _data.LoadEvents();

        switch (heatmapTypeSelectedOption)
        {
            case 0:
                Debug.Log("Executing Pathing Heatmap");
                CreateHeatmapObject(HeatmapType.PATHING);
                break;
            case 1:
                Debug.Log("Executing Heat Heatmap");
                CreateHeatmapObject(HeatmapType.HEAT);
                break;
            case 2:
                Debug.Log("Executing Option 3");
                break;
            default:
                break;
        }
    }

    private void CreateHeatmapObject(HeatmapType type)
    {
        if (_heatmap != null)
        {
            _heatmapComponent._heatmapType = type;
            return;
        }

        _heatmap = new GameObject();
        _heatmap.name = "Heatmap";
        _heatmapComponent = _heatmap.AddComponent<HeatmapObject>();

        _heatmapComponent.Initialize();
        _heatmapComponent._heatmapType = type;
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

    private void HeatmapTypesUI()
    {
        GUILayout.Label("Heatmap type:", EditorStyles.boldLabel);

        string[] options = { "Pathing", "Heat", "Option 3" };
        heatmapTypeSelectedOption = EditorGUILayout.Popup(heatmapTypeSelectedOption, options);
    }

    private void EventTypeSelectorUI()
    {
        if (heatmapTypeSelectedOption != 1 || _heatmapComponent == null) return;

        _heatmapComponent.eventTypeSelectedOptions = EditorGUILayout.MaskField("Event Types", _heatmapComponent.eventTypeSelectedOptions, _heatmapComponent.eventTypeOptions);

        GUILayout.Label("Heat pool: " + _heatmapComponent.heatList.Count, EditorStyles.boldLabel);
    }
}
