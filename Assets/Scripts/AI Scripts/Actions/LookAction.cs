using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "PluggableAI/Actions/LookAction")]
public class LookAction : Action
{

    public override void Act(StateController controller)
    {

        if(controller.AIVariables.GetNearbyEnemies().Count > 0)
        {

            Direction direction = controller.AIVariables.GetDirection();

            //There are enemies around the agent, so lets cast a ray to see if I see them
            foreach(GameObject gameObject in controller.AIVariables.GetNearbyEnemies())
            {

                Vector2 dir = controller.gameObject.transform.position - gameObject.transform.position;

                float angle = Vector2.SignedAngle(dir, new Vector2(1, 0));

                bool lookingToward = false;

                switch (direction) {

                    case Direction.UP:
                        if(angle >= 45 && angle <= 135)
                        {
                            lookingToward = true;
                        }
                        break;

                    case Direction.LEFT:
                        if(angle <= 45 && angle >= -45)
                        {
                            lookingToward = true;
                        }
                        break;

                    case Direction.RIGHT:

                        if(Mathf.Abs(angle) >= 135)
                        {
                            lookingToward = true;
                        }
                        break;

                    case Direction.Down:
                        if (angle <= -45 && angle >= -135)
                        {
                            lookingToward = true;
                        }
                        break;

                }

                if(lookingToward)
                {

                    controller.AIVariables.TargetSeen(gameObject);

                    //TODO: need to fix this as it is currently not properly raycasting
                    
                    
                    RaycastHit2D raycastHit;

                    raycastHit = Physics2D.Raycast(controller.gameObject.transform.position, (controller.gameObject.transform.position - gameObject.transform.position).normalized);
                      
                    if(raycastHit.collider != null) { 
                        
                        Debug.Log("we hit something");

                        if(raycastHit.collider.gameObject == gameObject)
                        {
                            //we have vision of the target!!
                            Debug.Log("we see the target!!");

                            //Lets tell the AI we see a target
                            controller.AIVariables.TargetSeen(gameObject);
                        }

                    }
                }


            }

        }

    }

}
