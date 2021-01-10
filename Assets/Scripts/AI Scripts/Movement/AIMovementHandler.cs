using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementHandler : MonoBehaviour
{

    [SerializeField]
    float turnRate;

    float timeElapsed;

    public Vector2 targetPosition;
    bool hasTarget = false, turning = false;

    [SerializeField]
    int leftAngle = 135;

    [SerializeField]
    int rightAngle = 45;

    public int speed;

    [SerializeField]
    public Animator animator;

    CircleCollider2D circleCollider; 

    public Direction currentDirection = Direction.SOUTH;

    public bool isIdle = true;

    public Vector2 targetDirection;

    Vector2 previousDirection;

    bool IsAttacking = false;



    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


            //hasTarget = true;
            //Vector2 testPos = Camera.main.ScreenToWorldPoint(transform.position);
            //float angle = Vector2.SignedAngle(testPos, (Vector2)transform.position - targetPosition);
            //isIdle = false;



        }

        if (IsAttacking)
        {
            Debug.Log("update loop stooped cause attacting");
            return;
        }

        if (hasTarget)
        {

            Vector2 dir;

            if (turning)
            {
                if (timeElapsed < turnRate)
                {
                    timeElapsed += Time.deltaTime;
                    dir = Vector3.Lerp(previousDirection, (targetPosition - (Vector2)transform.position).normalized, timeElapsed / turnRate);

                } else
                {
                    dir = (targetPosition - (Vector2)transform.position).normalized;
                    turning = false;
                }
            }
            else
            {

                dir = (targetPosition - (Vector2)transform.position).normalized;

            }

            SetAnimationDirection(dir);

            float frameSpeed = speed * Time.deltaTime;

            if (Vector2.Distance(transform.position, targetPosition) <= frameSpeed)
            {
                //we will be at destination after this is done
                //so lets set is postion to target and set target to null
                transform.position = targetPosition;
                hasTarget = false;
                //animator.SetTrigger("Idle");

                Animate.ChangeAnimationState("Idle", animator, currentDirection);
                isIdle = true;

            }
            else
            {

                transform.position += (Vector3)(dir * frameSpeed);               

            }

        }

       
    }

    #region Movement

    public void SetDirection(Vector2 dir)
    {
        targetPosition = (Vector2) this.transform.position + dir;
        hasTarget = true;
        isIdle = false;
    }

    public void SetNewWaypoint(Vector2 waypoint)
    {
        hasTarget = true;
        previousDirection = (targetPosition - (Vector2) transform.position).normalized;

        targetPosition = (Vector2) this.transform.position + waypoint;

        turning = true;

        timeElapsed = 0;
    }

    

    void SetAnimationDirection(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(1, 0));

        if (Mathf.Abs(angle) <= rightAngle)
        {
            if (currentDirection != Direction.EAST)
            {
                currentDirection = Direction.EAST;

                //animator.SetTrigger("Walking_Right");
                Animate.ChangeAnimationState("Walk", animator, currentDirection);

            }
        }
        else if (Mathf.Abs(angle) >= leftAngle)
        {
            if (currentDirection != Direction.WEST)
            {
                currentDirection = Direction.WEST;

                //animator.SetTrigger("Walking_Left");
                Animate.ChangeAnimationState("Walk", animator, currentDirection);


            }
        }
        else if (angle > 0)
        {
            if (currentDirection != Direction.SOUTH)
            {
                currentDirection = Direction.SOUTH;

                //animator.SetTrigger("Walking_Down");
                Animate.ChangeAnimationState("Walk", animator, currentDirection);

            }
        }
        else
        {
            if (currentDirection != Direction.NORTH)
            {
                currentDirection = Direction.NORTH;
                Animate.ChangeAnimationState("Walk", animator, currentDirection);

            }
        }
    }

    public Direction GetDirection()
    {
        return currentDirection;
    }

    public Vector2 GetTargetDirection()
    {
        if (hasTarget == false) return Vector2.zero;

        return (targetPosition - (Vector2)transform.position).normalized;
    }

    #endregion

    #region Attacking

    public void Attack()
    {
        IsAttacking = true;
        Animate.ChangeAnimationState("Attacking", animator, currentDirection);

        PrepareAttackHitboxes(currentDirection);
    }

    void StoppedAttactingAlert()
    {
        Debug.Log("Stoped attacking");
        Animate.ChangeAnimationState("Idle", animator, currentDirection);
        isIdle = true;
        IsAttacking = false;

    }

    void PrepareAttackHitboxes(Direction direction)
    {
        if (direction == Direction.SOUTH)
        {
            GetComponent<HitBoxController>().SetNewAnimation("SouthAttack");
        }
        else if (direction == Direction.NORTH)
        {
            GetComponent<HitBoxController>().SetNewAnimation("NorthAttack");
        }
        else if (direction == Direction.WEST)
        {
            GetComponent<HitBoxController>().SetNewAnimation("WestAttack");
        }
        else if (direction == Direction.EAST)
        {
            GetComponent<HitBoxController>().SetNewAnimation("EastAttack");
        }
    }

    #endregion






    #region collision Handling


    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("collision");
    //    Debug.Log(collision.gameObject.name);
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    Debug.Log("Collision exit");
    //    Debug.Log(collision.gameObject.name);
    //}

    #endregion

    public void SetIdle()
    {
        Debug.Log("Setting idle");
        isIdle = true;
        Animate.ChangeAnimationState("Idle", animator, currentDirection);
    }


}
