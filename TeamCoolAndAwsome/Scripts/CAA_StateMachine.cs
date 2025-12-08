using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CAA_StateMachine : MonoBehaviour
{
    private Dictionary<Type, CAA_BaseState> states;

    private CAA_BaseState currentState;

    public CAA_BaseState CurrentState
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

    public void SetStates(Disctionary<Type, CAA_BaseState> states)
    {
        this.states = states;
    }

    void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
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

