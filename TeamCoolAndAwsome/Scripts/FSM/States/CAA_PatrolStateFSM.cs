using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;

public class CAA_PatrolStateFSM : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank; //Tank object
    private int lowAmmo = 0; //Variable for how low the ammo can go 
    private int lowHP = 15; //Variable for how low the HP can go
    private int lowFuel = 30; //Variable for how low the Fuel can go
    float t; //Time variable
    public HeuristicMode heuristicMode;

    public CAA_PatrolStateFSM(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateUpdate()
    {
        //Does the Tank see Ammo pickups?
        if (smartTank.VisibleConsumables.Count > 0 && (smartTank.TankCurrentHealth < 125 || smartTank.TankCurrentFuel < 100 || smartTank.TankCurrentAmmo < 20))
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.First().Key, 0.8f);
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
            smartTank.FollowPathToRandomWorldPoint(0.5f, heuristicMode);
            t += Time.deltaTime;
            if (t > 10)
            {
                smartTank.GenerateNewRandomWorldPoint();
                t = 0;
            }
            return null;
        }
    }

    public override Type StateEnter()
    {
        t = 0; //Reset time Variable
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }
}
