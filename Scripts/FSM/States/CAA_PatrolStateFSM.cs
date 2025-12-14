using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;

//// Patrol State ////
public class CAA_PatrolStateFSM : CAA_BaseStateFSM
{
    private CAA_SmartTankFSM smartTank; //Tank object
    float t; //Time variable

    //Constructor
    public CAA_PatrolStateFSM(CAA_SmartTankFSM smartTank)
    {
        this.smartTank = smartTank;
    }

    //State Update
    public override Type StateUpdate()
    {
        //Does the Tank see any consumables?
        if (smartTank.VisibleConsumables.Count > 0)
        {
            smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.First().Key, 0.8f);
            return null;
        }
        //Does the Tank see an enemy and isn't on low HP OR sees any bases?
        else if ((smartTank.VisibleEnemyTanks.Count > 0 && smartTank.TankCurrentHealth > smartTank.lowHP) || smartTank.VisibleEnemyBases.Count > 0)
        {
            return typeof(CAA_ChaseStateFSM); //If so transition to the Chase State
        }
        else
        {
            t += Time.deltaTime; //Counts every second

            if (t > 3) //Checks every 3 seconds
            {
                if (smartTank.transform.position.x > 0 && smartTank.transform.position.z > 0) //If the tank is in the top right quadrant
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(-60, 0, 60); //Go to the top left
                }
                else if (smartTank.transform.position.x > 0 && smartTank.transform.position.z < 0) //If the tank is in the top left quadrant
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(60, 0, 60); //Go to the top left
                }
                else if (smartTank.transform.position.x < 0 && smartTank.transform.position.z < 0) //If the tank is in the bottom left quadrant
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(60, 0, 0); //Go to the top left
                }
                else if (smartTank.transform.position.x < 0 && smartTank.transform.position.z > 0) //If the tank is in the bottom right quadrant
                {
                    smartTank.patrolTargetObj.transform.position = new Vector3(-60, 0, -60); //Go to the top left
                }

                t = 0; //Reset the timer
            }

            //Follow the path to the quadrant decided by the code above, and go at a speed depening on how much fuel you have left.
            //The value of the speed is the current fuel, divided by 2, divided by 100 and + 0.1 so it actually caps off and doesn't just stop.
            //For example if you had 90 fuel, you'd move at a speed of 0.55 (90/2 = 45, 45/100 = 0.45, 0.45+0.1 = 0.55)
            //This is to use the fuel as efficiently as possible, as it seems to give quite the advantage in certain circumstances
            smartTank.FollowPathToWorldPoint(smartTank.patrolTargetObj, ((smartTank.TankCurrentFuel / 2) / 100) + 0.1f);

            //Debug.Log(t); // <<=== This line was just to see what the timer was doing while debugging

            return null; //Return null as the state (as it doesn't change
        }
    }
    
    //State Enter
    public override Type StateEnter() 
    {
        t = 0; //Reset time Variable
        Debug.Log("PATROL STATE"); // <<=== This line was also for debugging so we could see what states are being transitioned to
        return null;
    }

    //State Exit
    public override Type StateExit()
    {
        return null;
    }
}
