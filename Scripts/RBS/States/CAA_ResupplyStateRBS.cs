using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CAA_ResupplyStateRBS : CAA_BaseStateRBS
{
    private CAA_SmartTankRBS smartTank;

    public CAA_ResupplyStateRBS(CAA_SmartTankRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        smartTank.stats["resupplyState"] = true;
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["resupplyState"] = false;
        return null;
    }

    public override Type StateUpdate()
    {
        if (smartTank.VisibleConsumables.Count > 0)
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.First().Key, 0.8f); // Move towards the consumable
            smartTank.TurretFaceWorldPoint(smartTank.VisibleConsumables.First().Key); // Rotate turret to face the consumable so it doesn't lose track of it
        }
        else
        {
            smartTank.stats["collectableSpotted"] = false;
            smartTank.stats["collectableGone"] = true;
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
}
