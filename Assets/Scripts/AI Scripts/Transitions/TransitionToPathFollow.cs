using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Transitions/TransitionToPathFollow")]
public class TransitionToPathFollow : Transition
{
    public override bool DoTransition(StateController controller)
    {
        if(controller.AIVariables.findingPath)
        {
            controller.TransitionToState(trueState);
            return true;
        }

        return false;
    }

 
}
