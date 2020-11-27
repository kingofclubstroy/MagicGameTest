using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/RunAction")]
public class RunAction : Action
{
    public override void Act(StateController controller)
    {
        //For now we will just run away from the closest enemy
        List<GameObject> enemies = controller.AIVariables.GetNearbyEnemies();

        if (enemies.Count == 0) return;

        Vector2 dir;
        float distance = 0;
        
        for (int i = 0; i < enemies.Count; i++)
        {

            //int tempDistance = Vector2.Distance(controller.gameObject.transform.position, )
        }
    }
}
