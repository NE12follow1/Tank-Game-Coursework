using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CAA_EvadeStateFSM : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank;                                                            // reference to the smart tank
    private GameObject evadeTargetObj;                                                             // target to evade to

    public float lowHealth = 25;                                                                   // below this means stay in evade
    public float safeHealth = 50;                                                                  // above this means return to patrol
    public float evadeDistance = 20;                                                               // how far to run away
    public CAA_EvadeStateFSM(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;                                                                // set reference to smart tank
        evadeTargetObj = new GameObject("EvadeTarget");                                            // create dummy target object to move to
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

        // if there are visible enemies then continue to evade
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

            // move the dummy target into LOS
            evadeTargetObj.transform.position = evadeTarget;

            // follow this target 
            smartTank.FollowPathToWorldPoint(evadeTargetObj, 1);

            // face the enemy 
            smartTank.TurretFaceWorldPoint(closestEnemy);

            return null;
        }

        // If no enemies visible then back to patrol 
        return typeof(CAA_PatrolStateFSM);
    }

    public override Type StateExit()
    {
        return null;
    }
}
