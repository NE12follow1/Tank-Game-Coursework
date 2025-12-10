using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CAA_PatrolStateFSM : CAA_BaseStateFSM
{
    private SmartTank smartTank; //Tank object
    private int lowAmmo = 0; //Variable for how low the ammo can go 
    private int lowHP = 0; //Variable for how low the HP can go
    private int lowFuel = 0; //Variable for how low the Fuel can go
    float t; //Time variable
    public HeuristicMode heuristicMode;

    public CAA_PatrolStateFSM(SmartTank smartTank)
    {
        this.enShip = enShip;
    }

    public override Type StateUpdate()
    {
        //Does the Tank see Ammo pickups?
        if (smartTank.VisibleConsumables.Count > 0 && (smartTank.TankCurrentHealth <= lowHP || smartTank.TankCurrentFuel <= lowFuel || smartTank.TankCurrentAmmo <= lowAmmo))
        {
            return null;
        }
        else if (smartTank.VisibleEnemyBases.Count > 0)
        {
            return typeof(CAA_ChaseStateFSM);
        }
        //Does the Tank see an enemy and isn't on low HP?
        else if (smartTank.VisibleEnemyTanks.Count > 0 && smartTank.TankCurrentHealth > lowHP)
        {
            return typeof(CAA_ChaseStateFSM);
        }
        else
        {
            FollowPathToRandomWorldPoint(0.5f, heuristicMode);
            t += Time.deltaTime;
            if (t > 10)
            {
                GenerateNewRandomWorldPoint();
                t = 0;
            }
            return null;
        }
    }

    public override Type StateEnter()
    {
        t = 0; //Reset time Variable
    }

    public override Type StateExit()
    {
    }
}
