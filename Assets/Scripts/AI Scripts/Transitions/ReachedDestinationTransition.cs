using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Transitions/ReachedDestinationTransition")]
public class ReachedDestinationTransition : Transition
{
    public override bool DoTransition(StateController controller)
    {
        if(controller.AIVariables.reachedDestination)
        {
            controller.TransitionToState(trueState);
            return true;
        }

        return false;
    }

 
}
