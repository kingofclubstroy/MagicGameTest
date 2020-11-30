using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveToTargetAction")]
public class MoveToTargetAction : Action
{
    public override void Act(StateController controller)
    {
        if (controller.AIVariables.FocusedEnemy == null) return;

        Debug.Log("MoveTOTargetAction");

        Vector2 targetPos = controller.AIVariables.FocusedEnemy.transform.position;

        Vector2 dir = (targetPos - (Vector2)controller.transform.position).normalized;

        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, dir);

        if (hit.collider != null)
        {
            Debug.Log("hit something of name: " + hit.collider.name);

            if(hit.collider.gameObject != controller.AIVariables.FocusedEnemy)
            {
                Debug.Log("Something is in the way");
                //TODO: do something here
            } else
            {
                controller.AIVariables.MoveThisDirection(dir);
            }
        }

      
    }


}
