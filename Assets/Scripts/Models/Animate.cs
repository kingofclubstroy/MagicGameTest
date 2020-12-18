using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate
{

    public static void ChangeAnimationState(string newState, Animator animator, Direction currentDirection)
    {
        switch (newState)
        {

            case "Walk":

                switch (currentDirection)
                {
                    case Direction.WEST:
                        animator.Play("West_Walk_Animation");

                        break;

                    case Direction.EAST:
                        animator.Play("East_Walk_Animation");

                        break;

                    case Direction.SOUTH:
                        animator.Play("South_Walk_Animation");

                        break;

                    case Direction.NORTH:
                        animator.Play("North_Walk_Animation");

                        break;

                    

                }
                break;

            case "Casting":
                animator.Play("Casting_Animation");
                break;

            case "Attacking":

                switch (currentDirection) {

                    case Direction.NORTH:
                        animator.Play("North_Attack_Animation");
                        break;

                    case Direction.SOUTH:
                        Debug.LogError("playing down attack animation");
                        animator.Play("South_Attack_Animation");
                        break;

                    case Direction.WEST:
                        Debug.LogError("FIXME: playing south attack animation insted of left");
                        animator.Play("South_Attack_Animation");
                        break;

                    case Direction.EAST:
                        Debug.LogError("FIXME: playing south attack animation insted of left");
                        animator.Play("South_Attack_Animation");
                        break;

                }


                break;

            case "Hit":


                switch (currentDirection)
                {
                    case Direction.NORTH:
                        animator.Play("North_Hit_Animation");
                        break;

                    case Direction.SOUTH:
                        animator.Play("South_Hit_Animation");
                        break;

                    case Direction.WEST:
                        animator.Play("West_Hit_Animation");
                        break;

                    case Direction.EAST:
                        animator.Play("East_Hit_Animation");
                        break;

                }

                break;

            case "Idle":

                Debug.Log("setting idle animation, current direction = " + currentDirection);

                switch (currentDirection)
                {
                    case Direction.NORTH:
                        animator.Play("North_Idle_Animation");
                        break;

                    case Direction.SOUTH:
                        animator.Play("South_Idle_Animation");
                        break;

                    case Direction.WEST:
                        animator.Play("West_Idle_Animation");
                        break;

                    case Direction.EAST:
                        animator.Play("East_Idle_Animation");
                        break;

                }

                break;
        }

    }
}
