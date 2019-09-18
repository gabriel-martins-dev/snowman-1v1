using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MOVE TO SERVER GAMEOBJECT -> ServerEventManager
/// </summary>

public static class ServerEventName
{
    public static string WaitingForPlayers { get { return "WaitingForPlayers";  } }
    public static string RoundStarting(int time) { return "RoundStarting" + time; }
    public static string RoundStarted { get { return "RoundStarted"; } }

}

public class ServerEventManager : MonoBehaviour
{
    private Dictionary<string, Action> eventDictionary;

    private static ServerEventManager eventManager;

    public static ServerEventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(ServerEventManager)) as ServerEventManager;

                if (!eventManager)
                {
                    Debug.LogError("Missing EventManager");
                }
                else
                {
                    eventManager.Initialize();
                }
            }

            return eventManager;
        }
    }

    void Initialize()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action>();
        }
    }

    public static void Listen(string eventName, Action listener)
    {
        Debug.Log("Listen on " + eventName);

        Action action = null;

        if (instance.eventDictionary.TryGetValue(eventName, out action))
        {
            action += listener;
        }
        else
        {
            action = new Action(listener);
            instance.eventDictionary.Add(eventName, action);
        }
    }

    public static void Clear(string eventName, Action listener)
    {
        if (eventManager == null) return;

        Action action = null;

        if (instance.eventDictionary.TryGetValue(eventName, out action))
        {
            action -= listener;
        }
    }

    public static void TriggerEvent(string eventName)
    {
        Debug.Log("Trigger on " + eventName);

        Action action = null;
        if (instance.eventDictionary.TryGetValue(eventName, out action))
        {
            action?.Invoke();
        }
    }
}
