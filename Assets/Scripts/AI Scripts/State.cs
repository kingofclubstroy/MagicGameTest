using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/State")]
public class State : ScriptableObject
{

    public Action[] actions;
    public Transition[] transitions;

    public void UpdateState(StateController controller)
   {
        DoActions(controller);
   }

    private void DoActions(StateController controller)
    {
        foreach(Action action in actions)
        {
            action.Act(controller);
        }
    } 
}
