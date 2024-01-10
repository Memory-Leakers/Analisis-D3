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
    private float lineThickness = 10.0f;

    private void OnEnable()
    {
        
    }

    public Color gizmoColor = Color.red;


    public void Initialize()
    {
        if (_data == null)
        {
            _data = ServerActionDeserialize.Instance();
        }
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
        Handles.color = session.Color;

        // Draw the path
        for (int i = 0; i < session.events.Count - 1; i++)
        {
            Vector3 v1 = new Vector3(session.events[i].PositionX, session.events[i].PositionY, session.events[i].PositionZ);
            Vector3 v2 = new Vector3(session.events[i + 1].PositionX, session.events[i + 1].PositionY, session.events[i + 1].PositionZ);

            Handles.DrawLine(v1, v2, lineThickness);

            Quaternion arrowRotation = GetDirectionOnPath(v1, v2);

            DrawArrow(v1, arrowRotation);
        }
    }

    private Quaternion GetDirectionOnPath(Vector3 v1, Vector3 v2)
    {
        if ((v2 - v1) != Vector3.zero)
        {
            return Quaternion.LookRotation(v2 - v1);
        }
        else
        {
            return Quaternion.identity;
        }

    }

    private void DrawArrow(Vector3 position, Quaternion rotation)
    {
        // Draw arrow using Gizmos
        Handles.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Handles.DrawLine(Vector3.zero, Quaternion.Euler(0, 120, 0) * Vector3.forward * arrowSize, lineThickness);
        Handles.DrawLine(Vector3.zero, Quaternion.Euler(0, -120, 0) * Vector3.forward * arrowSize, lineThickness);
        Handles.matrix = Matrix4x4.identity; // Reset matrix
    }
    #endregion 
}
