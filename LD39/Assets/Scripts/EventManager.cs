using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{

    private Dictionary<string, GlowingObjectEvent> eventDictionary;

    public static EventManager instance;

    public class GlowingObjectEvent : UnityEvent<GlowingObject>
    {

    }

    private void Awake()
    {
        // Ensure the instance is of the type GameManager
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, GlowingObjectEvent>();
        }

        // Persist the GameManager instance across scenes
        DontDestroyOnLoad(gameObject);
    }

    public void StartListening(string eventName, UnityAction<GlowingObject> listener)
    {
        GlowingObjectEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new GlowingObjectEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<GlowingObject> listener)
    {
        if (instance == null) return;
        GlowingObjectEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, GlowingObject gameObject)
    {
        GlowingObjectEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(gameObject);
        }
    }
}