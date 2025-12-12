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
            smartTank.FollowPathToWorldPoint(patrolTargetObj, ((smartTank.TankCurrentFuel / 2) / 100) + 0.1f);
            t += Time.deltaTime;
            if (t > 10)
            {
                if (smartTank.transform.position.x > 0)
                {
                    GameObject.Destroy(patrolTargetObj);
                    patrolTargetObj = GameObject.Instantiate(new GameObject(), new Vector3(-60, 0, -60), Quaternion.identity);
                }
                else
                {
                    GameObject.Destroy(patrolTargetObj);
                    patrolTargetObj = GameObject.Instantiate(new GameObject(), new Vector3(60, 0, 60), Quaternion.identity);
                }

            }

            //Debug.Log(t);
            return null;
        }
    }
    

    public override Type StateEnter()
    {
        t = 0; //Reset time Variable
        Debug.Log("PATROL STATE");
        patrolTargetObj = GameObject.Instantiate(new GameObject(), new Vector3(-60,0,-60), Quaternion.identity);
        return null;
    }

    public override Type StateExit()
    {
        GameObject.Destroy(patrolTargetObj);
        return null;
    }
}
