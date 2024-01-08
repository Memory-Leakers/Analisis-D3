using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static ServerActionsSerialize;

public class ServerActionDeserialize : MonoBehaviour
{
    public class LoadEvent
    {
        // Getter/Setter
        public int Id { get { return _id; } }
        public string Type { get { return _type; } }
        public int Level { get { return _level; } }
        public float PositionX { get { return _positionX; } }
        public float PositionY { get { return _positionY; } }
        public float PositionZ { get { return _positionZ; } }
        public int Session_Id { get { return _session_id; } }
        public int Step { get { return _step; } }

        // Parameters
        private readonly int _id;
        private readonly string _type;
        private readonly int _level;
        private readonly float _positionX;
        private readonly float _positionY;
        private readonly float _positionZ;
        private readonly int _session_id;
        private readonly int _step;

        public LoadEvent (int id, string type, int level, float positionX, float positionY, float positionZ, int session_id, int step)
        {
            _id = id;
            _type = type;
            _level = level;
            _positionX = positionX;
            _positionY = positionY;
            _positionZ = positionZ;
            _session_id = session_id;
            _step = step;
        }
    }

    public class Session
    {
        //Getter/Setter
        public int Session_id { get { return _session_id; } }
        public DateTime Start_datetime { get { return _start_datetime; } }
        public DateTime End_datetime { get { return _end_datetime; } }

        // Parameters
        private readonly int _session_id;
        private readonly DateTime _start_datetime;
        private readonly DateTime _end_datetime;

        public List<LoadEvent> events;

        public Session(int session_id, DateTime start_datetime, DateTime end_datetime)
        {
            _session_id = session_id;
            _start_datetime = start_datetime;
            _end_datetime = end_datetime;

            
        }
    }

    public List<Session> _sessions;


    private void Awake()
    {
        _sessions = new List<Session>();
    }

    public void LoadData()
    {

    }

    public void LoadSessions()
    {
        string url = "citmalumnes.upc.es/~robertri/Select_All_Sessions.php";

        StartCoroutine(SendRequest(url));
    }

    private IEnumerator SendRequest(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                Debug.Log("Load all sessions successfully" + www.downloadHandler.text);
            }
        }
    }

    //Callbacks
    private void LoadAllSessionsSuccessfully(UnityWebRequest www)
    {
        Debug.Log("Load all sessions successfully" + www.downloadHandler.text);
        
    }
}
