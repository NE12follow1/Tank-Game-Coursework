using System;
using UnityEngine;

public class CAA_EvadeStateFSM : BaseState
{
    private SmartTank tank;
    private float lowHealth = 30f;                                                                                         // what the health needs to be to evade
    private float highHealth = 60f;                                                                                        // what the health needs to be to stop evading
    private float evadeDistance = 10f;                                                                                     // distance to move away when evading

    public CAA_EvadeStateFSM(SmartTank tank)
    {
        this.tank = tank;
    }

    public override Type StateEnter()                                                                                      // on entering evade state
    {
        return null;
    }

    public override Type StateExit()                                                                                       // on exiting evade state
    {
        return null;
    }

    public override Type StateUpdate()                                                                                     // during evade state
    {
        if (tank.TankCurrentHealth < tank.TankMaxHealth * 0.3f)                                                            // if health is below 30, evade
        {
            Vector3 directionAway = (tank.transform.position - tank.PlayerTank.transform.position).normalized;
            Vector3 evadeTarget = tank.transform.position + directionAway * evadeDistance;
            tank.MoveToPosition(evadeTarget);
            return null;
        }
        else if (tank.TankCurrentHealth >= tank.TankMaxHealth * 0.6f)                                                      // if health is above 60, go back to patrol
        {
            return typeof(PatrolState);                                                                                    // Go back to patrol
        }

        return null;
    }
}
