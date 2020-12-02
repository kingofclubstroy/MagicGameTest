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

                    Debug.Log("looking toward");

                    //controller.AIVariables.TargetSeen(gameObject);

                    //TODO: need to fix this as it is currently not properly raycasting

                    //Vector2 targetPos = controller.AIVariables.FocusedEnemy.transform.position;

                    dir = (gameObject.transform.position - controller.gameObject.transform.position).normalized;

                    // Cast a ray straight down.
                    RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, dir);

                    if (hit.collider != null)
                    {

                        if (hit.collider.gameObject != gameObject)
                        {
                            Debug.Log("Something is in the way");

                            //We now need to pathfind
                            //controller.AIVariables.SetPathfindingParams(controller.AIVariables.FocusedEnemy.transform.position);
                            return;



                        }
                        else
                        {
                            //controller.AIVariables.MoveThisDirection(dir);
                            controller.AIVariables.TargetSeen(gameObject);
                        }
                    } else
                    {
                        Debug.Log("nothing hit");
                    }


                //    RaycastHit2D[] raycastHit;

                //    bool targetFound = false;

                //    float distance = Vector2.Distance(controller.gameObject.transform.position, gameObject.transform.position);

                //    raycastHit = Physics2D.RaycastAll(controller.gameObject.transform.position, (controller.gameObject.transform.position - gameObject.transform.position).normalized, distance);

                   

                //    foreach (RaycastHit2D hit in raycastHit)
                //    {

                //        if (hit.collider.gameObject == gameObject)
                //        {
                //            //we have vision of the target!!
                //            Debug.Log("we see the target!!");
                //            targetFound = true;
                //            //Lets tell the AI we see a target
                               
                //        } else if (hit.collider.tag == "Obstacle")
                //        {
                //            //We have found an obstacle between the two points so we dont see the target
                //            return;
                //        }

                        

                //    }

                //    if (targetFound)
                //    {

                //        controller.AIVariables.TargetSeen(gameObject);

                //    }


                }


            }

        }

    }

}
