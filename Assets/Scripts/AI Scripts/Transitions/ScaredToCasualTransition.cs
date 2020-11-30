using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Transitions/ScaredToCasual")]
public class ScaredToCasualTransition : Transition
{
    public int MaxDistance;
    public override bool DoTransition(StateController controller)
    {
        //TODO: need to add a focused enemy, but for now i will just look at the first in list of nearby enemies of the aiVariables
        GameObject enemy;

        if(controller.AIVariables.FocusedEnemy != null)
        {
           
            enemy = controller.AIVariables.FocusedEnemy;

            if(Vector2.Distance(enemy.transform.position, controller.AIVariables.transform.position) >= MaxDistance)
            {
                controller.AIVariables.FocusedEnemy = null;

                Debug.Log("transitioning to casual state");
                controller.TransitionToState(trueState);
                return true;
            }
        }

        return false;

    }

    
}
