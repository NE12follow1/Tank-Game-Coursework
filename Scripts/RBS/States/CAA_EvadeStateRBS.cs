using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Runtime.InteropServices;

public class CAA_EvadeStateRBS : CAA_BaseStateRBS
{
    private CAA_SmartTankRBS smartTank; // Reference to the smart tank
    private AStar aStarScript; // Reference to the AStar script

    public CAA_EvadeStateRBS(CAA_SmartTankRBS smartTank)
    {
        this.smartTank = smartTank; // Set reference to smart tank
    }

    public override Type StateEnter()
    {
        Debug.Log("EvadeState");
        smartTank.stats["evadeState"] = true;
        aStarScript = GameObject.Find("AStarPlane").GetComponent<AStar>(); // Give the AStar script the grid of the game

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

        return null;
    }

    public override Type StateUpdate()
    {
        smartTank.FollowPathToWorldPoint(smartTank.PatrolTargetObj, ((smartTank.TankCurrentFuel / 2) / 100) + 0.2f); // Move towards the patrol target at a slightly faster speed

        foreach (var item in smartTank.rules.GetRules)
        {
            if (item.CheckRule(smartTank.stats) != null)
            {
                return item.CheckRule(smartTank.stats);
            }
        }

        return null;
    }

    public override Type StateExit()
    {
        smartTank.stats["evadeState"] = false;
        return null;
    }
}
