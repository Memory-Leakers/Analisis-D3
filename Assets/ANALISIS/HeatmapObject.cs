using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using static ServerActionDeserialize;
using UnityEditor.PackageManager;
using static UnityEditor.Progress;

public enum HeatmapType
{
    NONE,
    HEAT,
    PATHING
}

public struct Heat
{
    public Vector3 Position { get { return pos; } }
    public Color Intensity { get { return intensity; } }

    private readonly Vector3 pos;
    private readonly Color intensity;

    public Heat(Vector3 pos, Color intensity)
    {
        this.pos = pos;
        this.intensity = intensity;
    }
}

public class HeatmapObject : MonoBehaviour
{
    ServerActionDeserialize _data;
    public HeatmapType _heatmapType = HeatmapType.NONE;

    //Material 

    //Options
    public string[] eventTypeOptions = { "Position", "Death", "Damage" };
    public int eventTypeSelectedOptions = -1;

    //Gizmos settings
    private float arrowSize = 1f;
    private float lineThickness = 10.0f;
    public float heatWeight = 0.05f;

    public void Initialize()
    {
        if (_data == null)
        {
            _data = ServerActionDeserialize.Instance();
        }
    }

    public void ForceUpdate()
    {
        
    }

    private void OnDrawGizmos()
    {
        switch (_heatmapType)
        {
            case HeatmapType.PATHING:
                {
                    Pathing();
                }
                break;
            case HeatmapType.HEAT:
                {
                    Heat();
                }
                break;
            default:
                {
                    Debug.LogError("Incorrect Heatmap type!");
                }
                break;
        }
    }

    #region Path Heatmap

    private void Pathing()
    {
        foreach (var s in _data._sessions)
        {
            if (s.active)
            {
                DrawPath(s);
            }
        }
    }

    private void DrawPath(Session session)
    {
        Handles.color = session.Color;

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
        Handles.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
        Handles.DrawLine(Vector3.zero, Quaternion.Euler(0, 120, 0) * Vector3.forward * arrowSize, lineThickness);
        Handles.DrawLine(Vector3.zero, Quaternion.Euler(0, -120, 0) * Vector3.forward * arrowSize, lineThickness);
        Handles.matrix = Matrix4x4.identity; // Reset matrix
    }
    #endregion

    #region Heat Heatmap

    private void Heat()
    {
        List<LoadEvent> pool = new List<LoadEvent>();
        List<Heat> heatList = new List<Heat>();

        foreach (var s in _data._sessions)
        {
            if (s.active)
            {
                //Load all event types
                if (eventTypeSelectedOptions == -1) pool.AddRange(s.events);
                else
                {
                    foreach (var e in s.events)
                    {
                        for (int i = 0; i < eventTypeOptions.Length; i++)
                        {
                            if ((eventTypeSelectedOptions & (1 << i)) != 0 &&
                                eventTypeOptions[i].Equals(e.Type, System.StringComparison.CurrentCultureIgnoreCase))
                            {
                                pool.Add(e);
                            }
                        }
                    }
                }
                
            }
        }

        Color minIntensity = new Color(0.0f, 1.0f, 0.0f);
        Color maxIntensity = new Color(1.0f, 0.0f, 0.0f);
        float proximityRadius = 10.0f;

        for (int i = 0; i < pool.Count; i++)
        {
            Vector3 pos = new Vector3(pool[i].PositionX, pool[i].PositionY, pool[i].PositionZ);
            Color intensity = minIntensity;

            for (int j = 0; j < pool.Count; j++) 
            {
                if (i != j)
                {
                    Vector3 checkPos = new Vector3(pool[j].PositionX, pool[j].PositionY, pool[j].PositionZ);
                    float distance = Vector3.Distance(pos, checkPos);
                
                    if (distance <= proximityRadius)
                    {
                        intensity.r += heatWeight;
                        intensity.g -= heatWeight;
                    }
                }
            }

            intensity.r = Mathf.Clamp(intensity.r, minIntensity.r, maxIntensity.r);
            intensity.g = Mathf.Clamp(intensity.g, maxIntensity.g, minIntensity.g);

            heatList.Add(new Heat(pos, intensity));
        }

        DrawHeat(heatList);
    }

    private void DrawHeat(List<Heat> heat)
    {
        Debug.Log("Total Heat data-> "+heat.Count);
        foreach (Heat h in heat) 
        {
            Gizmos.color = h.Intensity;
            Gizmos.DrawCube(h.Position, new Vector3(5.0f, 5.0f, 5.0f));
        }
    }

    #endregion
}
