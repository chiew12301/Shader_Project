using System.Collections.Generic;
using UnityEngine.Events;

/*
 * How To Use? Tutorial
 * When creating a new event, please create an event based on AnEvent class.
 * Then when adding Listener add according to example in OnEnable()->  EventManager.INSTANCE.AddListener(new ADeEvent(), AFunction);
 * Remember to add RemoveListener in OnDisable by replacing AddListener to RemoveListener
 * After that, When you trying to call event, please use AddEvent([YourEventClass]). Example -> EventManager.INSTANCE.AddEvent(new BDeEvent());
 * That's all!
 * 
 * Refer to AnEvent to create an event
 * 
 */

public class EventManager : MonobehaviourSingleton<EventManager>
{
    private Dictionary<AnEvent, UnityEvent> m_eventsDict = null;

    private Dictionary<AnEvent, UnityEvent<object>> m_events2Dict = null;

    private Dictionary<AnEvent, List<System.Action<AnEvent>>> m_event3Dict = null;

    //==================================================

    #region OVERRIDE

    protected override void OnEnable()
    {
        base.OnEnable();
        this.Init();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.Uninit();
    }

    #endregion OVERRIDE

    //==================================================

    #region FUNCTIONS

    //Public Function Below

    public void AddListenerTwo<T>(AnEvent eventToAssign, UnityAction<T> listener) where T : AnEvent
    {
        if (GetInstance() == null) return;

        UnityEvent<T> thisEventTwo = new UnityEvent<T>();

        System.Action<T> thisEventThree = new System.Action<T>(listener);


        if (null == GetInstance().m_events2Dict)
        {
            GetInstance().m_events2Dict = new Dictionary<AnEvent, UnityEvent<object>>();
        }

        if(null == GetInstance().m_event3Dict)
        {
            GetInstance().m_event3Dict = new Dictionary<AnEvent, List<System.Action<AnEvent>>>();
        }

        foreach (AnEvent e in GetInstance().m_event3Dict.Keys)
        {
            if (e.GetType().ToString() == eventToAssign.GetType().ToString())
            {
                GetInstance().m_event3Dict[e].Add(thisEventThree as System.Action<AnEvent>);
                return;
            }
        }

        //List<System.Action<AnEvent>> newList = new List<System.Action<AnEvent>>();
        //newList.Add(thisEventThree);

        //INSTANCE.m_event3Dict.Add(eventToAssign, newList);

        foreach (AnEvent e in GetInstance().m_events2Dict.Keys)
        {
            if (e.GetType().ToString() == eventToAssign.GetType().ToString())
            {
                thisEventTwo = GetInstance().m_events2Dict[e] as UnityEvent<T>;
                thisEventTwo.AddListener(listener);              
                return;
            }
        }

        thisEventTwo.AddListener(listener);
        UnityEvent<object> tryCast = thisEventTwo as UnityEvent<object>;
        //UnityEngine.Debug.Log(tryCast.GetType().ToString());
        if(tryCast == null)
        {
            UnityEngine.Debug.Log(":O");
            object obj = System.Convert.ChangeType(thisEventTwo, typeof(UnityEvent<object>));
        }
        UnityEngine.Debug.Log(listener.GetType().ToString() + " " + thisEventTwo.GetType().ToString());
        GetInstance().m_events2Dict.Add(eventToAssign, tryCast);
    }

    public void AddListener(AnEvent eventToAssign, UnityAction listener)
    {
        if (GetInstance() == null) return;

        UnityEvent thisEvent = null;

        if (null == GetInstance().m_eventsDict)
        {
            GetInstance().m_eventsDict = new Dictionary<AnEvent, UnityEvent>();
        }

        foreach (AnEvent e in GetInstance().m_eventsDict.Keys)
        {
            if(e.GetType().ToString() == eventToAssign.GetType().ToString())
            {
                thisEvent = GetInstance().m_eventsDict[e];
                thisEvent.AddListener(listener);
                return;
            }
        }

        thisEvent = new UnityEvent();
        thisEvent.AddListener(listener);
        GetInstance().m_eventsDict.Add(eventToAssign, thisEvent);
    }

    public void RemoveListener(AnEvent eventToAssign, UnityAction listener)
    {
        if (GetInstance() == null) return;

        UnityEvent thisEvent = null;

        foreach (AnEvent e in GetInstance().m_eventsDict.Keys)
        {
            if (e.GetType().ToString() == eventToAssign.GetType().ToString())
            {
                thisEvent = GetInstance().m_eventsDict[e];
                thisEvent.RemoveListener(listener);
                return;
            }
        }
    }

    public void RemoveListenerTwo<T>(AnEvent eventToAssign, UnityAction<T> listener) where T : AnEvent
    {
        if (GetInstance() == null) return;

        UnityEvent<T> thisEvent = null;

        foreach (AnEvent e in GetInstance().m_events2Dict.Keys)
        {
            if (e.GetType().ToString() == eventToAssign.GetType().ToString())
            {
                thisEvent = GetInstance().m_events2Dict[e] as UnityEvent<T>;
                thisEvent.RemoveListener(listener);
                return;
            }
        }
    }

    public void AddEvent(AnEvent eventToAssign)
    {
        if (GetInstance() == null) return;

        UnityEvent thisEvent = null;

        foreach (AnEvent e in GetInstance().m_eventsDict.Keys)
        {
            if (e.GetType().ToString() == eventToAssign.GetType().ToString())
            {
                thisEvent = GetInstance().m_eventsDict[e];
                thisEvent?.Invoke();
                return;
            }
        }
    }

    public void AddEvent2(AnEvent eventToAssign)
    {
        if (GetInstance() == null) return;

        foreach (AnEvent e in GetInstance().m_events2Dict.Keys)
        {
            if (e.GetType().ToString() == eventToAssign.GetType().ToString())
            {
                UnityEngine.Debug.Log(GetInstance().m_events2Dict[e].ToString());
                GetInstance().m_events2Dict[e]?.Invoke(eventToAssign);
                return;
            }
        }
    }    

    //Private Function Below

    private void Init()
    {
        if(null == this.m_eventsDict)
        {
            this.m_eventsDict = new Dictionary<AnEvent, UnityEvent>();
        }
    }

    private void Uninit()
    {
        if(null != this.m_eventsDict)
        {
            foreach(AnEvent e in GetInstance().m_eventsDict.Keys)
            {
                GetInstance().m_eventsDict[e].RemoveAllListeners();
            }
        }
    }

    #endregion FUNCTIONS

    //==================================================
}
