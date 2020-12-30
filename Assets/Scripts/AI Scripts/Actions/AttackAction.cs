using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/AttackAction")]
public class AttackAction : Action
{
    public override void Act(StateController controller)
    {
        //TODO: may need to change this, as there may be cases were we are moving towards a target that isnt a object. Like a point in space
        if (controller.AIVariables.FocusedEnemy == null) return;

        Vector2 targetPos = controller.AIVariables.FocusedEnemy.transform.position;

        if(Vector2.Distance(targetPos, controller.transform.position) <= controller.AIVariables.AttackRange)
        {
            //We are within range, so lets attack!!
            //We are handling attack cooldowns on the objects end
            controller.AIVariables.Attack();
        }

      
    }

}
