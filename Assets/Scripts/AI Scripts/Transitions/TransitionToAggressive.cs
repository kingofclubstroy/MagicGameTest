using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Transitions/TransitionToAggressive")]
public class TransitionToAggressive : Transition
{
    public override bool DoTransition(StateController controller)
    {
        if(controller.AIVariables.IsAggressive && controller.AIVariables.FocusedEnemy != null)
        {
            controller.TransitionToState(trueState);
            return true;
        }

        return false;
    }

 
}
