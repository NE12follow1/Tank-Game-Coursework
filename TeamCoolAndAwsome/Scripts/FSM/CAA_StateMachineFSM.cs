using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CAA_StateMachineFSM : MonoBehaviour
{
    private Dictionary<Type, CAA_BaseStateFSM> states;

    private CAA_BaseStateFSM currentState;

    public CAA_BaseStateFSM CurrentState
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

    public void SetStates(Dictionary<Type, CAA_BaseStateFSM> states)
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
