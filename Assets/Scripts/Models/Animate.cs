using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate
{

    public static void ChangeAnimationState(string newState, Animator animator, Direction currentDirection)
    {
        switch (newState)
        {
            case "WalkLeft":
                animator.Play("Walk_Left_Animation");
               
                break;

            case "WalkRight":
                animator.Play("Walk_Right_Animation");
               
                break;

            case "WalkDown":
                animator.Play("Walk_Down_Animation");
                
                break;

            case "WalkUp":
                animator.Play("Walk_Up_Animation");
                
                break;

            case "Casting":
                animator.Play("Casting_Animation");
                break;

            case "Attacking":

                switch (currentDirection) {

                    case Direction.UP:
                        animator.Play("Attack_Up_Animation");
                        break;

                    case Direction.Down:
                        Debug.LogError("playing down attack animation");
                        animator.Play("Attack_Down_Animation");
                        break;

                    case Direction.LEFT:
                        animator.Play("Attack_Down_Animation");
                        break;

                    case Direction.RIGHT:
                        animator.Play("Attack_Down_Animation");
                        break;

                }


                break;

            case "Idle":


                switch (currentDirection)
                {
                    case Direction.UP:
                        animator.Play("Up_Idle_Animation");
                        break;

                    case Direction.Down:
                        animator.Play("Down_Idle_Animation");
                        break;

                    case Direction.LEFT:
                        animator.Play("Left_Idle_Animation");
                        break;

                    case Direction.RIGHT:
                        animator.Play("Right_Idle_Animation");
                        break;

                }

                break;
        }

    }
}
