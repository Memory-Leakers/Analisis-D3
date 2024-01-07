using Gamekit3D;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerActionsSerialize : MonoBehaviour
{
    private class DataToSend
    {
        public string Url { get { return _url; } }
        protected string _url = "citmalumnes.upc.es/~robertri/RAW.php";

        // Getter/Setter
        public string RequestDate { get => _requestDate.ToString("yyyy-MM-dd hh:mm:ss"); }
        public uint ID { get { return _ID; } set => _ID = value; }

        // Parameters
        private uint _ID;
        protected DateTime _requestDate;

        // Functions
        public virtual WWWForm GetForm() { return new WWWForm(); }

        public ActionSuccesfull onAccionSuccesfull = null;
    }

    private class SessionStart : DataToSend
    {
        // Constructor
        public SessionStart(DateTime startDate, ActionSuccesfull actionSuccesfull)
        {
            _requestDate = startDate;
            onAccionSuccesfull = actionSuccesfull;
            ID = 0;

            _url = "citmalumnes.upc.es/~robertri/Create_Session.php";
        }

        // Functions
        public override WWWForm GetForm()
        {
            WWWForm form = new();

            form.AddField("start_datetime", RequestDate);

            return form;
        }
    }

    private class SessionEnd : DataToSend
    {
        // Constructor
        public SessionEnd(uint sessionID, DateTime endDate, ActionSuccesfull actionSuccesfull)
        {
            _requestDate = endDate;
            onAccionSuccesfull = actionSuccesfull;
            ID = sessionID;

            _url = "citmalumnes.upc.es/~robertri/Close_Session.php";
        }

        // Functions
        public override WWWForm GetForm()
        {
            WWWForm form = new();

            form.AddField("id", (int)ID);
            form.AddField("end_datetime", RequestDate);

            return form;
        }
    }

    private class Events : DataToSend
    {
        public Events(string type, int level, float positionX, float positionY, float positionZ, int session_id, DateTime date, int step, ActionSuccesfull actionSuccesfull)
        {
            _type = type;
            _level = level;
            _positionX = positionX;
            _positionY = positionY;
            _positionZ = positionZ;
            _session_id = session_id;
            _requestDate = date;
            _step = step;
            onAccionSuccesfull = actionSuccesfull;
            ID = 0;

            _url = "citmalumnes.upc.es/~robertri/Register_Event.php";
        }

        // Getter/Setter
        public string Type { get { return _type; } }
        public int Level { get { return _level; } }
        public float PositionX { get { return _positionX; } }
        public float PositionY { get { return _positionY; } }
        public float PositionZ { get { return _positionZ; } }
        public int Session_Id { get { return _session_id; } }
        public int Step { get { return _step; } }

        // Parameters
        private readonly string _type;
        private readonly int _level;
        private readonly float _positionX;
        private readonly float _positionY;
        private readonly float _positionZ;
        private readonly int _session_id;
        private readonly int _step;

        public override WWWForm GetForm()
        {
            WWWForm form = new();

            form.AddField("Type", _type);
            form.AddField("Level", _level);
            form.AddField("Position_X", _positionX.ToString());
            form.AddField("Position_Y", _positionY.ToString());
            form.AddField("Position_Z", _positionZ.ToString());
            form.AddField("Session_id", _session_id);
            form.AddField("Date", RequestDate);
            form.AddField("Step", _step);

            Debug.Log(RequestDate);

            return form;
        }
    }

    private Events _events;

    private SessionStart _sessionStart;

    private SessionEnd _sessionEnd;

    private int _step = 0;

    delegate void ActionSuccesfull(UnityWebRequest www);

    private void OnEnable()
    {
        //PlayerController.Die += OnPlayerDeath;
    }

    private void OnDisable()
    {
        
    }

    private IEnumerator WebRequest(DataToSend dataToSend)
    {
        WWWForm form = dataToSend.GetForm();
        UnityWebRequest www = UnityWebRequest.Post(dataToSend.Url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(www.error);
        else
            dataToSend.onAccionSuccesfull?.Invoke(www);
    }

    //Callbacks
    private void OnSessionStart()
    {
        DateTime dateTime = DateTime.Now;
        _sessionStart = new SessionStart(dateTime, SessionCreatedSuccessfully);

        _step = 0;

        StartCoroutine(WebRequest(_sessionStart));
    }

    private void OnSessionEnd()
    {
        DateTime dateTime = DateTime.Now;
        _sessionEnd = new SessionEnd(_sessionStart.ID, dateTime, SessionClosedSuccessfully);

        _step = 0;

        StartCoroutine(WebRequest(_sessionEnd));
    }

    private void OnPlayerMoving(int level, float positionX, float positionY, float positionZ)
    {
        DateTime dateTime = DateTime.Now;
        _events = new Events("Position", level, positionX, positionY, positionZ, (int)_sessionStart.ID, dateTime, _step, EventPositionSuccessfully);
        
        _step++;
        
        StartCoroutine(WebRequest(_events));
    }

    private void OnPlayerDeath(int level, float positionX, float positionY, float positionZ)
    {
        DateTime dateTime = DateTime.Now;

        _events = new Events("Death", level, positionX, positionY, positionZ, (int) _sessionStart.ID, dateTime, _step, EventDeathSuccessfully);

        //_step++;
    }

    //When Action successfully
    private void SessionCreatedSuccessfully(UnityWebRequest www)
    {
        Debug.Log("Session Started/Created successfully: " + www.downloadHandler.text);
        _sessionStart.ID = uint.Parse(www.downloadHandler.text);
        //CallbackEvents.On
    }

    private void SessionClosedSuccessfully(UnityWebRequest www)
    {
        Debug.Log("Session Closed/Finished successfully:" + www.downloadHandler.text);
        //Callback
    }

    private void EventPositionSuccessfully(UnityWebRequest www)
    {
        Debug.Log("Event Position successffully: " + www.downloadHandler.text);
        //callback
    }

    private void EventDeathSuccessfully(UnityWebRequest www)
    {
        Debug.Log("Event Position successfully: " + www.downloadHandler.text);
        //Callback
    }
}
