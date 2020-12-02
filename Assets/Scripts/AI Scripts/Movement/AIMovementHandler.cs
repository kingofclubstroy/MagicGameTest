using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementHandler : MonoBehaviour
{

    Vector2 targetPosition;
    bool hasTarget = false;

    [SerializeField]
    int leftAngle = 135;

    [SerializeField]
    int rightAngle = 45;

    public int speed;

    [SerializeField]
    Animator animator;

    CircleCollider2D circleCollider; 

    public Direction currentDirection = Direction.Down;

    public bool isIdle = true;

    public Vector2 targetDirection;



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
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            
            hasTarget = true;
            Vector2 testPos = Camera.main.ScreenToWorldPoint(transform.position);
            float angle = Vector2.SignedAngle(testPos, (Vector2) transform.position - targetPosition);
            isIdle = false;
            
        }

        if(hasTarget)
        {

            Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;

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
                transform.position += (Vector3)  (dir * frameSpeed);
               

            }

        }

       
    }

    public void SetDirection(Vector2 dir)
    {
        targetPosition = (Vector2) this.transform.position + dir;
        hasTarget = true;
        isIdle = false;
    }

    

    void SetAnimationDirection(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(1, 0));

        if (Mathf.Abs(angle) <= rightAngle)
        {
            if (currentDirection != Direction.RIGHT)
            {
                currentDirection = Direction.RIGHT;

                //animator.SetTrigger("Walking_Right");
                Animate.ChangeAnimationState("WalkRight", animator, currentDirection);

            }
        }
        else if (Mathf.Abs(angle) >= leftAngle)
        {
            if (currentDirection != Direction.LEFT)
            {
                currentDirection = Direction.LEFT;

                //animator.SetTrigger("Walking_Left");
                Animate.ChangeAnimationState("WalkLeft", animator, currentDirection);


            }
        }
        else if (angle > 0)
        {
            if (currentDirection != Direction.Down)
            {
                currentDirection = Direction.Down;

                //animator.SetTrigger("Walking_Down");
                Animate.ChangeAnimationState("WalkDown", animator, currentDirection);

            }
        }
        else
        {
            if (currentDirection != Direction.UP)
            {
                currentDirection = Direction.UP;
                Animate.ChangeAnimationState("WalkUp", animator, currentDirection);

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


}
