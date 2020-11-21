using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementHandler : MonoBehaviour
{

    Vector2 targetPosition;
    bool hasTarget = false;

    public int speed;

    [SerializeField]
    Animator animator;

    public enum Direction {

        RIGHT,
        LEFT,
        UP,
        Down

    }

    Direction currentDirection = Direction.Down;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hasTarget = true;
            float angle = Vector2.SignedAngle(transform.position, targetPosition.normalized);
            Debug.Log("angle = " + angle);
            Debug.Log("targetPosition = " + targetPosition);

            // the vector that we want to measure an angle from
            Vector3 referenceForward = transform.position;/* some vector that is not Vector3.up */
                                       // the vector perpendicular to referenceForward (90 degrees clockwise)
                                       // (used to determine if angle is positive or negative)
            Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
            // the vector of interest
            Vector3 newDirection = targetPosition; /* some vector that we're interested in */
                                   // Get the angle in degrees between 0 and 180
            float angle2 = Vector3.Angle(newDirection, referenceForward);
            // Determine if the degree value should be negative.  Here, a positive value
            // from the dot product means that our vector is on the right of the reference vector   
            // whereas a negative value means we're on the left.
            float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
            float finalAngle = sign * angle2;

            Debug.Log("new signed angle = " + finalAngle);
            
        }

        if(hasTarget)
        {

            Vector2 dir = (Vector2)(targetPosition - (Vector2)transform.position).normalized;

            float angle = Vector2.SignedAngle(transform.position, targetPosition);

            

            if(dir.x != 0)
            {
                if(dir.x > 0)
                {
                    if(currentDirection != Direction.RIGHT)
                    {
                        currentDirection = Direction.RIGHT;
                        animator.SetBool("Walking_Up", false);
                        animator.SetBool("Walking_Down", false);
                        animator.SetBool("Walking_Left", false);
                        animator.SetBool("Walking_Right", true);

                    }
                } else if (currentDirection != Direction.LEFT)
                {
                    currentDirection = Direction.LEFT;
                    animator.SetBool("Walking_Up", false);
                    animator.SetBool("Walking_Down", false);
                    animator.SetBool("Walking_Left", true);
                    animator.SetBool("Walking_Right", false);
                }
            } else
            {
                if(dir.y > 0)
                {
                    if(currentDirection != Direction.UP)
                    {
                        currentDirection = Direction.UP;
                        animator.SetBool("Walking_Up", true);
                        animator.SetBool("Walking_Down", false);
                        animator.SetBool("Walking_Left", false);
                        animator.SetBool("Walking_Right", false);
                    } 
                } else if(currentDirection != Direction.Down)
                {
                    currentDirection = Direction.Down;
                    animator.SetBool("Walking_Up", false);
                    animator.SetBool("Walking_Down", true);
                    animator.SetBool("Walking_Left", false);
                    animator.SetBool("Walking_Right", false);
                }
            }


            float frameSpeed = speed * Time.deltaTime;

            if (Vector2.Distance(transform.position, targetPosition) <= frameSpeed)
            {
                //we will be at destination after this is done
                //so lets set is postion to target and set target to null
                transform.position = targetPosition;
                hasTarget = false;
            }
            else
            {
                transform.position += (Vector3)  (dir * frameSpeed);
            }

        }

       
    }
}
