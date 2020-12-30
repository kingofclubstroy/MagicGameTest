using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public State currentState;

    [HideInInspector ]public AIVariables AIVariables;

    [HideInInspector] float stateTimeElapsed;

    // Start is called before the first frame update
    void Start()
    {
        AIVariables = GetComponent<AIVariables>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    private void OnDrawGizmos()
    {
        if(currentState != null)
        {
            //todo: Draw gizmos if we want them
        }
    }

    public void TransitionToState(State nextState)
    {
       
        
        currentState = nextState;
        OnExitState();
        
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }
}
