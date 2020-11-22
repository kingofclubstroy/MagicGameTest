﻿using System.Collections;
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
            Vector2 testPos = Camera.main.ScreenToWorldPoint(transform.position);
            float angle = Vector2.SignedAngle(testPos, (Vector2) transform.position - targetPosition);
           

           
            
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
                animator.SetTrigger("Idle");

            }
            else
            {
                transform.position += (Vector3)  (dir * frameSpeed);
            }

        }

       
    }

    void SetAnimationDirection(Vector2 dir)
    {
        float angle = Vector2.SignedAngle(dir, new Vector2(1, 0));

        if (Mathf.Abs(angle) <= rightAngle)
        {
            if (currentDirection != Direction.RIGHT)
            {
                currentDirection = Direction.RIGHT;
               
                animator.SetTrigger("Walking_Right");

            }
        }
        else if (Mathf.Abs(angle) >= leftAngle)
        {
            if (currentDirection != Direction.LEFT)
            {
                currentDirection = Direction.LEFT;
                
                animator.SetTrigger("Walking_Left");
              
            }
        }
        else if (angle > 0)
        {
            if (currentDirection != Direction.Down)
            {
                currentDirection = Direction.Down;
               
                animator.SetTrigger("Walking_Down");
               
            }
        }
        else
        {
            if (currentDirection != Direction.UP)
            {
                currentDirection = Direction.UP;
                animator.SetTrigger("Walking_Up");
               
            }
        }
    }
}