using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AStar;
using System.Linq;

public class CAA_PatrolStateRBS : CAA_BaseStateRBS
{
    private CAA_SmartTankRBS smartTank; //Tank object
    private float t = 0; //Time variable
    private AStar aStarScript; 

    public CAA_PatrolStateRBS(CAA_SmartTankRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    public override Type StateUpdate()
    {
        t += Time.deltaTime; // Increment the timer

        if (t > 3) // After 3 seconds
        {
            Node targetNode = aStarScript.NodePositionInGrid(new Vector3(60, 0, 60)); // Node for the movement target

            // Sets the movement target to the mext corner (going in an anitclockwise direction)
            if (smartTank.transform.position.x > 0 && smartTank.transform.position.z > 0) // If in the top right corner of the map
            {
                targetNode = aStarScript.NodePositionInGrid(new Vector3(-60, 0, 60)); // Set movement target to the top left corner of the map
            }
            else if (smartTank.transform.position.x < 0 && smartTank.transform.position.z > 0) // If in the top left corner of the map
            {
                targetNode = aStarScript.NodePositionInGrid(new Vector3(-60, 0, -60)); // Set movement target to the bottom left corner of the map 
            }
            else if (smartTank.transform.position.x < 0 && smartTank.transform.position.z < 0) // If in the bottom left corner of the map
            {
                targetNode = aStarScript.NodePositionInGrid(new Vector3(50, 0, -50)); // Set movement target to the bottom right corner of the map
            }
            else if (smartTank.transform.position.x > 0 && smartTank.transform.position.z < 0) // If in the bottom right corner of the map
            {
                targetNode = aStarScript.NodePositionInGrid(new Vector3(60, 0, 60)); // Set movement target to the top right corner of the map
            }

            smartTank.PatrolTargetObj.transform.position = targetNode.nodePos; // Convert the position from a node into the position of the movement target
            t = 0; // Reset the timer
        }

        smartTank.FollowPathToWorldPoint(smartTank.PatrolTargetObj, smartTank.TankCurrentFuel / 125); // Move to the patrol target at a speed relative to the amount of fuel left

        foreach (var item in smartTank.rules.GetRules)
        {
            if (item.CheckRule(smartTank.stats) != null)
            {
                return item.CheckRule(smartTank.stats);
            }
        }

        return null;
    }


    public override Type StateEnter()
    {
        aStarScript = GameObject.Find("AStarPlane").GetComponent<AStar>();
        smartTank.stats["patrolState"] = true;
        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["patrolState"] = false;
        return null;
    }
}

