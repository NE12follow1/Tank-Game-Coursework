using System;
using UnityEngine;
using System.Linq;

public class CAA_EvadeStateFSM : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank;

    public float lowHealth = 30;                                                                   // below this means stay in evade
    public float safeHealth = 60;                                                                  // above this means return to patrol
    public float evadeDistance = 20;                                                               // how far to run away

    public CAA_EvadeStateFSM(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        // If health is now safe then return to patrol
        if (smartTank.TankCurrentHealth >= safeHealth)
        {
            return typeof(CAA_PatrolStateFSM);
        }

        // Still too low health then continue evading
        if (smartTank.VisibleEnemyTanks.Count > 0)                                                                   
        {
            // find closest visible enemy tank
            var closestEnemy = smartTank.VisibleEnemyTanks
                .OrderBy(e => Vector3.Distance(smartTank.transform.position, e.Key.transform.position))
                .First().Key;

            // move away from enemy
            Vector3 directionAway =                                                                                  
                (smartTank.transform.position - closestEnemy.transform.position).normalized;

            Vector3 evadeTarget =
                smartTank.transform.position + directionAway * evadeDistance;

            smartTank.FollowPathToWorldPoint(evadeTarget, 1);
            smartTank.TurretFaceWorldPoint(closestEnemy.transform.position);

            return null;
        }

        // If no enemies visible then return to patrol 
        return typeof(CAA_PatrolStateFSM);
    }

    public override Type StateExit()
    {
        return null;
    }
}
