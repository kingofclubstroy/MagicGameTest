using System;
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
        //Debug.Log(Description);
        //if(listeners == null)
        //{
        //    Debug.Log("noone cares about this event");
        //}
        listeners?.Invoke(this as T);
    }
}

public class DebugEvent : Event<DebugEvent>
{
    public int VerbosityLevel;
}

public class UnitDeathEvent : Event<UnitDeathEvent>
{   
    public UnitDeathEvent()
    {
        Description = "Unit died, firing callbacks";
    }
    public GameObject UnitGO;
    /*
    Info about cause of death, our killer, etc...
    Could be a struct, readonly, etc...
    */
}

public class SpellSelectedEvent : Event<SpellSelectedEvent>
{ 
    public SpellSelectedEvent()
    {
        Description = "Spell has been selected, telling everything that cares";
    }
    public Spell_Icon_Script spell;
    public bool selected = true;
}

public class SpellUnSelectedEvent : Event<SpellUnSelectedEvent>
{
    public SpellUnSelectedEvent()
    {
        Description = "Spell unselected, telling everything that cares";
    }
}

public class CastingLocationChangedEvent : Event<CastingLocationChangedEvent>
{
    public CastingLocationChangedEvent()
    {
        Description = "Casting location changed, telling eveything that cares";
    }
    public GameObject go;
}

public class StartedCastingEvent : Event<StartedCastingEvent>
{
    public StartedCastingEvent()
    {
        Description = "Started to cast, telling eveything that cares";
    }

}

public class StopCastingCall : Event<StopCastingCall>
{
    public StopCastingCall()
    {
        Description = "Call to stop casting, probably due to lack of stamina";
    }
}

public class StoppedCastingEvent : Event<StoppedCastingEvent>
{
    public StoppedCastingEvent()
    {
        Description = "Stopped casting, telling everything that cares";
    }
}

public class CastingProjectionCreatedEvent : Event<CastingProjectionCreatedEvent>
{
    public CastingProjectionCreatedEvent()
    {
        Description = "Casting projection created, telling everything that cares";
    }
    public GameObject castingProjection;
}

public class CastingProjectionDestroyedEvent : Event<CastingProjectionDestroyedEvent>
{
    public CastingProjectionDestroyedEvent()
    {
        Description = "Casting ProjectionDestroyed, telling everyone that cares";
    }
}

//TODO: may not want to deal with water adding and being removed here, but for now its fine
public class WaterCreatedEvent : Event<WaterCreatedEvent>
{
    public WaterCreatedEvent()
    {
        Description = "Water was created, telling everything that cares";
    }

    public Vector2 waterPosition;

}

public class WaterRemovedEvent : Event<WaterRemovedEvent>
{
    public WaterRemovedEvent()
    {
        Description = "Water was removed, telling everything that cares";
    }

    public Vector2 waterPosition;

}

public class RemoveWaterEvent : Event<RemoveWaterEvent>
{
    public RemoveWaterEvent()
    {
        Description = "Wanting to remove water, telling everything that cares";
    }

    public Vector2 position;
}

public class SpellCastCall : Event<SpellCastCall>
{
    public SpellCastCall()
    {
        Description = "Wanting to cast a spell now";
    }
}

public class RotationEvent : Event<RotationEvent>
{
    public float rotation;
    public RotationEvent(float newRotation)
    {
        rotation = newRotation;
    }
}

public class SpellCastEvent : Event<SpellCastEvent>
{

}
