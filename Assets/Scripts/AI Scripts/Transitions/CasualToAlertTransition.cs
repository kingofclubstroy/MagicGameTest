using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Transitions/CasualToAlert")]
public class CasualToAlertTransition : Transition
{

    public int AwarenessTransition;
    public override bool DoTransition(StateController controller)
    {

        if(controller.AIVariables.FocusedEnemy != null)
        {
            Debug.Log("transitioning to scared");
            controller.TransitionToState(trueState);
            return true;
        }

        else
        {
            return false;
        }
    }
}
