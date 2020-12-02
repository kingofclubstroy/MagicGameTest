using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/FollowPathAction")]
public class FollowPathAction : Action
{
    public override void Act(StateController controller)
    {
        //For now we will just run away from the closest enemy
        //GameObject enemy = controller.AIVariables.FocusedEnemy;

        controller.AIVariables.MoveAlongPath();
        
       
        

      
    }
}
