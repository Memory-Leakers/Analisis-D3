using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static ServerActionDeserialize;
using static ServerActionsSerialize;

public class ServerActionDeserialize
{
    [Serializable]
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

    [Serializable]
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

        public LoadEvent[] events;

        public Session(int session_id, DateTime start_datetime, DateTime end_datetime)
        {
            _session_id = session_id;
            _start_datetime = start_datetime;
            _end_datetime = end_datetime;

            
        }

        //UI
        public bool active = false;
        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            active = EditorGUILayout.Toggle("", active);
            GUILayout.Label(Session_id.ToString());
            GUILayout.Label(Start_datetime.ToString());
            GUILayout.Label(End_datetime.ToString());
            EditorGUILayout.EndHorizontal();
        }
    }

    public List<Session> _sessions;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10)) 
        {
            LoadSessions();
        }
    }

    public void LoadData()
    {

    }

    public void LoadSessions()
    {
        _sessions = null;
        string url = "citmalumnes.upc.es/~robertri/Select_All_Sessions.php";

        if (this != null)
        {
            CoroutineRunner.instance.StartCoroutine(SendGetRequest(url));
        }
        else Debug.LogError("MonoBehaviour instance is null");
    }

    public void LoadEvents()
    {
        string url = "citmalumnes.upc.es/~robertri/Select_Events.php";
        List<int> ids = new List<int>();

        foreach (Session s in _sessions)
        {
            if (s.active)
            {
                ids.Add(s.Session_id);
            }
        }


        string stringIds = string.Join(",", ids);

        if (this != null)
        {
            CoroutineRunner.instance.StartCoroutine(SendEventPostRequest(url, stringIds));
        }
        else Debug.LogError("MonoBehaviour instance is null");
    }

    private IEnumerator SendGetRequest(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
        else
        {
            Debug.Log("Load all sessions successfully" + www.downloadHandler.text);
            LoadAllSessionsSuccessfully(www);
        }
            
    }

    private IEnumerator SendEventPostRequest(string url, string ids)
    {
        WWWForm form = new WWWForm();

        form.AddField("Ids", ids);

        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
        else
        {
            Debug.Log("Load all sessions successfully" + www.downloadHandler.text);
            //LoadAllSessionsSuccessfully(www);
        }
    }

    //Callbacks
    private void LoadAllSessionsSuccessfully(UnityWebRequest www)
    {       
        string aux = www.downloadHandler.text;

        _sessions = ParseSessions(aux);


        Debug.Log("Successfully loaded sessions: "+ _sessions.Count);
    }

    #region Parser
    private List<Session> ParseSessions(string jsonData)
    {
        List<Session> sessions = new List<Session>();

        // Split the JSON array into individual objects
        string[] sessionObjects = jsonData.Split(new[] { "},{" }, StringSplitOptions.None);

        foreach (string sessionObject in sessionObjects)
        {
            // Remove leading and trailing square brackets
            string cleanedObject = sessionObject.Trim('[', ']');

            // Split the object into key-value pairs
            string[] keyValuePairs = cleanedObject.Split(',');

            // Extract values for each key
            int session_id = GetInt(keyValuePairs, "_session_id");
            DateTime start_datetime = GetDateTime(keyValuePairs, "_start_datetime");
            DateTime end_datetime = GetDateTime(keyValuePairs, "_end_datetime");

            sessions.Add(new Session((int) session_id, start_datetime, end_datetime));
        }

        return sessions;
    }

    private string GetString(string[] keyValuePairs, string key)
    {
        foreach (string pair in keyValuePairs)
        {
            string[] keyValue = pair.Split(':');

            if (keyValue.Length == 2 && keyValue[0].Contains(key))
            {
                return keyValue[1].Trim('\"', ' ', '\t', '\n', '\r');
            }
        }

        return null; 
    }

    private int GetInt(string[] keyValuePairs, string key)
    {
        string aux = GetString(keyValuePairs, key);
        if (aux != null)
        {
            return int.Parse(aux);
        }

        return -1;
    }

    private DateTime GetDateTime(string[] keyValuePairs, string key)
    {
        string aux = ""; GetString(keyValuePairs, key);
        foreach (string pair in keyValuePairs)
        {
            string[] keyValue = pair.Split(':', 2);
            if (keyValue.Length == 2 && keyValue[0].Contains(key))
            {
                aux = keyValue[1].Trim('\"', ' ', '\t', '\n', '\r');
                break;
            }
        }

        if (aux != null)
        {
            DateTime dateTime;
            DateTime.TryParseExact(aux, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateTime);
            return dateTime;
        }

        return DateTime.MinValue;
    }
    #endregion
}
