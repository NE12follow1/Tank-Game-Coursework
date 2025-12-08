using System;
using UnityEngine;


private int low health = 30;                        // Variable for low health threshold
private int high health = 60;                       // Variable for high health threshold
private int evade distance = 10;                    // Distance to move away when evading

public class EvadeState : BaseState                 // Evade state for SmartTank FSM
{
    private SmartTank smartTank;                    // Reference to the SmartTank

    public EvadeState(SmartTank smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }

    public override Type StateUpdate()
    {

       
        // If health is lower than 30%, switch to EvadeState
        if (smartTank.TankCurrentHealth < smartTank.TankMaxHealth * 0.3f)         // Check if health is low
        {
            // Evade logic: Move away from the player
            Vector3 directionAwayFromPlayer = (smartTank.transform.position - smartTank.PlayerTank.transform.position).normalized;
            Vector3 evadeTargetPosition = smartTank.transform.position + directionAwayFromPlayer * 10f; // Move 10 units away
            smartTank.MoveToPosition(evadeTargetPosition);
            return null; // Stay in EvadeState
        }
        else if (smartTank.TankCurrentHealth >= smartTank.TankMaxHealth * 0.6f)       // If health is above 60%, switch to AttackState
        {
            return typeof(AttackState);
        }
        else
        {
            // Resupply, find health
            smartTank.ResupplyState();
            return null;
        }
    }
}
