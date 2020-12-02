using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/MoveToTargetAction")]
public class MoveToTargetAction : Action
{
    public override void Act(StateController controller)
    {
        //TODO: may need to change this, as there may be cases were we are moving towards a target that isnt a object. Like a point in space
        if (controller.AIVariables.FocusedEnemy == null) return;

        Vector2 targetPos = controller.AIVariables.FocusedEnemy.transform.position;

        Vector2 dir = (targetPos - (Vector2)controller.transform.position).normalized;

        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, dir);

        if (hit.collider != null)
        {

            if(hit.collider.gameObject != controller.AIVariables.FocusedEnemy)
            {
                Debug.Log("Something is in the way");

                //We now need to pathfind
                controller.AIVariables.SetPathfindingParams(controller.AIVariables.FocusedEnemy.transform.position);

                

            } else
            {
                controller.AIVariables.MoveThisDirection(dir);
            }
        }

      
    }

}
