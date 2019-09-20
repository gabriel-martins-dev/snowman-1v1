using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EventDelegate<T>(T e) where T : GameEvent;

    private Dictionary<Type, Delegate> delegates;

    private static EventManager singleton;

    public static EventManager Singleton
    {
        get
        {
            if (!singleton)
            {
                singleton = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!singleton)
                {
                    Debug.LogError("Missing EventManager");
                }
                else
                {
                    singleton.Initialize();
                }
            }

            return singleton;
        }
    }

    void Initialize()
    {
        if (delegates == null)
        {
            delegates = new Dictionary<Type, Delegate>();
        }
    }

    public static void Listen<T>(EventDelegate<T> del) where T : GameEvent
    {
        if (Singleton.delegates.ContainsKey(typeof(T)))
        {
            Delegate tempDel = Singleton.delegates[typeof(T)];

            Singleton.delegates[typeof(T)] = Delegate.Combine(tempDel, del);
        }
        else
        {
            Singleton.delegates[typeof(T)] = del;
        } 
    }

    public static void Clear<T>(EventDelegate<T> del) where T : GameEvent
    {
        if (Singleton.delegates.ContainsKey(typeof(T)))
        {
            var currentDel = Delegate.Remove(Singleton.delegates[typeof(T)], del);

            if (currentDel == null)
            {
                Singleton.delegates.Remove(typeof(T));
            }
            else
            {
                Singleton.delegates[typeof(T)] = currentDel;
            }
        }
    }

    public static void Trigger(GameEvent gameEvent)
    {
        if (Singleton.delegates.ContainsKey(gameEvent.GetType()))
        {
            Singleton.delegates[gameEvent.GetType()].DynamicInvoke(gameEvent);
        }
    }
}
