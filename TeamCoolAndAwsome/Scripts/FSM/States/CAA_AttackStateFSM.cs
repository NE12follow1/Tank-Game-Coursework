using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CAA_AttackStateFSM : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank;

    public int lowAmmo = 0;
    public float lowFuel = 30;
    public float lowHealth = 15;
    public float firingRange = 30;

    public CAA_AttackStateFSM(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateUpdate()
    {
        // Is the tank at low health?
        if (smartTank.TankCurrentHealth <= lowHealth)
        {
            return typeof(CAA_EvadeStateFSM); // If it is, go to the evade state
        }
        // Does the tank have a target and sufficient fuel and ammo?
        else if (smartTank.TankCurrentFuel <= lowFuel || smartTank.TankCurrentAmmo <= lowAmmo || (smartTank.VisibleEnemyTanks.Count == 0 && smartTank.VisibleEnemyBases.Count == 0))
        {
            return typeof(CAA_PatrolStateFSM); // If it doesn't have either or both, go to the patrol state
        }
        else
        {
            // Does the tank have a valid enemy tank target?
            if (smartTank.VisibleEnemyTanks.Count > 0)
            {
                // Is the enemy tank target within firing range?
                if (Vector3.Distance(smartTank.transform.position, smartTank.VisibleEnemyTanks.First().Key.transform.position) > firingRange)
                {
                    return typeof(CAA_ChaseStateFSM); // If not, then go to the chase state
                }
                else
                {
                    smartTank.TurretFireAtPoint(smartTank.VisibleEnemyTanks.First().Key); // Otherwise fire at the target enemy tank
                    smartTank.TurretFaceWorldPoint(smartTank.VisibleEnemyTanks.First().Key); // Rotate turret to face the tank so it doesn't lose track of it
                    return null;
                }
            }
            // Otherwise, does the tank have a valid enemy base target?
            else if (smartTank.VisibleEnemyBases.Count > 0)
            {
                // Is the enemy base target within firing range?
                if (Vector3.Distance(smartTank.transform.position, smartTank.VisibleEnemyBases.First().Key.transform.position) > firingRange)
                {
                    return typeof(CAA_ChaseStateFSM); // If not, then go to the chase state
                }
                else
                {
                    smartTank.TurretFireAtPoint(smartTank.VisibleEnemyBases.First().Key); // Otherwise fire at the target enemy base
                    smartTank.TurretFaceWorldPoint(smartTank.VisibleEnemyBases.First().Key); // Rotate turret to face the base so it doesn't lose track of it
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }

    public override Type StateExit()
    {
        return null;
    }
}
