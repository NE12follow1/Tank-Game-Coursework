using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CAA_AttackStateRBS : CAA_BaseStateRBS
{
    private CAA_SmartTankRBS smartTank;

    public CAA_AttackStateRBS(CAA_SmartTankRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["attackState"] = true;
        return null;
    }

    public override Type StateUpdate()
    {
        // Does the tank have a valid enemy tank target?
        if (smartTank.VisibleEnemyTanks.Count > 0)
        {
            smartTank.TurretFireAtPoint(smartTank.VisibleEnemyTanks.First().Key); // Fire at the target enemy tank
            smartTank.TurretFaceWorldPoint(smartTank.VisibleEnemyTanks.First().Key); // Rotate turret to face the tank so it doesn't lose track of it
        }
        // Otherwise, does the tank have a valid enemy base target?
        else if (smartTank.VisibleEnemyBases.Count > 0)
        {
            smartTank.TurretFireAtPoint(smartTank.VisibleEnemyBases.First().Key); // Fire at the target enemy base
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
        smartTank.stats["attackState"] = false;
        return null;
    }
}
