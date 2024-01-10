using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using static ServerActionDeserialize;

public class HeatmapObject : MonoBehaviour
{
    ServerActionDeserialize _data;


    //Gizmos settings
    private float arrowSize = 1f;
    private float lineThickness = 5.0f;

    private void OnEnable()
    {
        if (_data == null)
        {
            _data = ServerActionDeserialize.Instance();
        }
        _data.LoadSessions();
    }

    public Color gizmoColor = Color.red;


    public void Initialize()
    {
        
    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(new Vector3(0, 0, 0), 1);
        //Gizmos.DrawSphere(new Vector3(0, 0, 10), 1);

        //Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, 10));



        foreach (var s in _data._sessions) 
        {
            if (s.active)
            {
                DrawPath(s);
            }
        }
        
    }


    #region Path Heatmap

    private void DrawPath(Session session)
    {
        // Draw the path
        for (int i = 0; i < session.events.Count - 1; i++)
        {
            Vector3 v1 = new Vector3(session.events[i].PositionX, session.events[i].PositionY, session.events[i].PositionZ);
            Vector3 v2 = new Vector3(session.events[i + 1].PositionX, session.events[i + 1].PositionY, session.events[i + 1].PositionZ);

            Handles.DrawLine(v1, v2, lineThickness);
        }

        // Draw arrows along the path
        for (int i = 0; i < session.events.Count; i++)
        {
            Vector3 v1 = new Vector3(session.events[i].PositionX, session.events[i].PositionY, session.events[i].PositionZ);
            Vector3 v2 = new Vector3(session.events[i + 1].PositionX, session.events[i + 1].PositionY, session.events[i + 1].PositionZ);
            float t = i / (float)(session.events.Count - 1);  // Parameterize the path

            Vector3 arrowPosition = GetPointOnPath(t, v1, v2);
            Quaternion arrowRotation = GetDirectionOnPath(t, v1, v2);

            DrawArrow(arrowPosition, arrowRotation);
        }
    }

    private Vector3 GetPointOnPath(float t, Vector3 v1, Vector3 v2)
    {
        return Vector3.Lerp(v1, v2, t);
    }

    private Quaternion GetDirectionOnPath(float t, Vector3 v1, Vector3 v2)
    {
        return Quaternion.LookRotation(v2 - v1);
    }

    private void DrawArrow(Vector3 position, Quaternion rotation)
    {
        // Draw arrow using Gizmos
        Handles.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Handles.DrawLine(Vector3.zero, Vector3.forward * arrowSize, lineThickness);
        Handles.DrawLine(Vector3.zero, Quaternion.Euler(0, 120, 0) * Vector3.forward * arrowSize, lineThickness);
        Handles.DrawLine(Vector3.zero, Quaternion.Euler(0, -120, 0) * Vector3.forward * arrowSize, lineThickness);
        Handles.matrix = Matrix4x4.identity; // Reset matrix
    }
    #endregion 
}
