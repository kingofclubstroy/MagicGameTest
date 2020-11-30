using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RunAction")]
public class RunAction : Action
{
    public override void Act(StateController controller)
    {
        //For now we will just run away from the closest enemy
        GameObject enemy = controller.AIVariables.FocusedEnemy;


        if (enemy == null)
        {

            //Debug.LogError("Focused enemy is null");
            return;
        }
       
        

        Vector2 dir = Vector2.zero;
        float distance = 0;

        float tempDistance = Vector2.Distance(controller.gameObject.transform.position, enemy.transform.position);

        if(distance == 0 || tempDistance < distance)
        {
            distance = tempDistance;
            dir = (controller.gameObject.transform.position - enemy.transform.position).normalized;
        }
        

        if (distance != 0)
        {
            controller.AIVariables.MoveThisDirection(dir);
        }
    }
}
