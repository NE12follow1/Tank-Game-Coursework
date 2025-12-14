using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CAA_StateMachineRBS : MonoBehaviour
{
    private Dictionary<Type, CAA_BaseStateRBS> states;

    private CAA_BaseStateRBS currentState;

    public CAA_BaseStateRBS CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            currentState = value;
        }
    }

    public void SetStates(Dictionary<Type, CAA_BaseStateRBS> states)
    {
        this.states = states;
    }

    void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
            CurrentState.StateEnter();
        }
        else
        {
            var nextState = CurrentState.StateUpdate();

            if (nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState);
            }
        }
    }

    void SwitchToState(Type nextState)
    {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }
}
