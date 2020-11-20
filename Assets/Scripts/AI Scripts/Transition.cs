using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Transition : ScriptableObject
{
    public Decision decision;
    public State trueState;
    public State falseState;

    public abstract void DoTransition(StateController controller);


}
