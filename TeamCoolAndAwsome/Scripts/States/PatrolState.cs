using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PatrolState : BaseState
{
    private SmartTank smartTank;
    private int lowAmmo = 0;
    private int lowHP = 0;
    private int lowFuel = 0;


    public PatrolState(SmartTank smartTank)
    {
        this.enShip = enShip;
    }

    public override Type StateUpdate()
    {
        if (smartTank.TankCurrentAmmo() < lowAmmo)
        {
            if (smartTank.VisibleConsumables.ContainsKey()) //Does the Tank see Ammo pickups?
            {
                return typeof(ResupplyState);
            }
        }
        else if (smartTank.TankCurrentFuel() < lowFuel)
        {
            if (smartTank.VisibleConsumables.ContainsKey()) //Does the Tank see Fuel pickups?
            {
                return typeof(ResupplyState);
            }
        }
        else if (smartTank.TankCurrentHealth < lowHP) 
        {
            if (smartTank.VisibleConsumables.ContainsKey()) //Does the Tank see HP pickups?
            {
                return typeof(ResupplyState);
            }
        }
        else
        {
            return null;
        }
    }

    public override Type StateEnter()
    {
        return null;
    }

    public override Type StateExit()
    {
        return null;
    }
}
