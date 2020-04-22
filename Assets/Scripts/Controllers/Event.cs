﻿using System;
using UnityEngine;


public abstract class Event<T> where T : Event<T>
{
    /*
        * The base Event,
        * might have some generic text
        * for doing Debug.Log?
        */
    public string Description;

    public delegate void EventListener(T info);
    private static event EventListener listeners;

    public static void RegisterListener(EventListener listener)
    {
        listeners += listener;
    }

    public static void UnregisterListener(EventListener listener)
    {
        listeners -= listener;
    }

    public void FireEvent()
    {
            
        if (listeners != null)
        {
            listeners(this as T);
        }
    }
}

public class DebugEvent : Event<DebugEvent>
{
    public int VerbosityLevel;
}

public class UnitDeathEvent : Event<UnitDeathEvent>
{
    public GameObject UnitGO;
    /*
    Info about cause of death, our killer, etc...
    Could be a struct, readonly, etc...
    */
}

public class SpellSelectedEvent : Event<SpellSelectedEvent>
{
    public SpellTest spell;
    public bool selected = true;
}

public class SpellUnSelectedEvent : Event<SpellUnSelectedEvent>
{
}
