using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;

public class CAA_PatrolStateFSM : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank; //Tank object
    private GameObject patrolTargetObj;
    float t; //Time variable

    public CAA_PatrolStateFSM(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateUpdate()
    {
        //Does the Tank see Ammo pickups?
        if (smartTank.VisibleConsumables.Count > 0)
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.First().Key, 0.8f); //Temp Resupply
            return null;
        }
        //Does the Tank see an enemy and isn't on low HP OR sees any bases?
        else if ((smartTank.VisibleEnemyTanks.Count > 0 && smartTank.TankCurrentHealth > smartTank.lowHP) || smartTank.VisibleEnemyBases.Count > 0)
        {
            return typeof(CAA_ChaseStateFSM);
        }
        else
        {
            t += Time.deltaTime;
            if (t < 10)
            {
                smartTank.GeneratePathToWorldPoint(patrolTargetObj);
            }

            return null;
        }
    }
    

    public override Type StateEnter()
    {
        t = 0; //Reset time Variable
        Debug.Log("PATROL STATE");
        patrolTargetObj = new GameObject("PatrolTarget");
        patrolTargetObj.transform.position = new Vector3(smartTank.transform.position.x, 2, smartTank.transform.position.y);
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }
}
