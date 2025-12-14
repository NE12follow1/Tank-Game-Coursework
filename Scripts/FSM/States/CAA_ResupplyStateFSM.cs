using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CAA_ResupplyState : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank; // Tank object

    public CAA_ResupplyState(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        Debug.Log("RESUPPLY STATE"); // Show state transition
        return null;
    }

    public override Type StateUpdate()
    {
        // Are there consumables nearby?
        if (smartTank.VisibleConsumables.Count > 0)
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.First().Key, 0.8f); // If visible, move towards consumable
            smartTank.TurretFaceWorldPoint(smartTank.VisibleConsumables.First().Key); // Rotate turret to not lose track of consumable
            return null;
        }
        // If there are no consumables, return to patrol state
        else
        {
            return typeof(CAA_PatrolStateFSM);
        }


    }

    public override Type StateExit()
    {
        return null;
    }
}
