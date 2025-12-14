using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CAA_ChaseStateRBS : CAA_BaseStateRBS
{
    private CAA_SmartTankRBS smartTank;

    public CAA_ChaseStateRBS(CAA_SmartTankRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["chaseState"] = true;
        return null;
    }

    public override Type StateUpdate()
    {
        // Does the tank have a valid enemy tank target?
        if (smartTank.VisibleEnemyTanks.Count > 0)
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleEnemyTanks.First().Key, 1f); // Go towards the next visible enemy tank at max speed
            smartTank.TurretFaceWorldPoint(smartTank.VisibleEnemyTanks.First().Key); // Rotate turret to face the tank so it doesn't lose track of it
        }
        // Otherwise, does the tank have a valid enemy base target?
        else if (smartTank.VisibleEnemyBases.Count > 0)
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleEnemyBases.First().Key, 1f); // Go towards the next visible enemy base at max speed
            smartTank.TurretFaceWorldPoint(smartTank.VisibleEnemyBases.First().Key); // Rotate turret to face the base so it doesn't lose track of it
        }

        foreach (var item in smartTank.rules.GetRules)
        {
            if (item.CheckRule(smartTank.stats) != null)
            {
                return item.CheckRule(smartTank.stats);
            }
        }

        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["chaseState"] = false;
        return null;
    }
}
