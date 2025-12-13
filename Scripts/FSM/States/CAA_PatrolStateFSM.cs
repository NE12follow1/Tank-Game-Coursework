using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;

public class CAA_PatrolStateFSM : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank; //Tank object
    float t; //Time variable

    public CAA_PatrolStateFSM(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateUpdate()
    {
        //Does the Tank see any pickups?
        if (smartTank.VisibleConsumables.Count > 0)
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.First().Key, 0.8f);
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

            if (t > 3)
            {
                if (smartTank.transform.position.x > 0 && smartTank.transform.position.z > 0)
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(-60, 0, 60);
                }
                else if (smartTank.transform.position.x > 0 && smartTank.transform.position.z < 0)
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(60, 0, 60);
                }
                else if (smartTank.transform.position.x < 0 && smartTank.transform.position.z < 0)
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(60, 0, 0);
                }
                else if (smartTank.transform.position.x < 0 && smartTank.transform.position.z > 0)
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(-60, 0, -60);
                }

                t = 0;
            }

            smartTank.FollowPathToWorldPoint(smartTank.patrolTargetObj, ((smartTank.TankCurrentFuel / 2) / 100) + 0.1f);
            Debug.Log(t);
            return null;
        }
    }
    

    public override Type StateEnter()
    {
        t = 0; //Reset time Variable
        Debug.Log("PATROL STATE");
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }
}
