using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AStar;

/// <summary>
/// Class <c>DumbTank</c> is an example class used to demonstrate how to use the functions available from the <c>AITank</c> base class. 
/// Copy this class when creating your smart tank class.
/// </summary>
public class CAA_SmartTankRBS : AITank
{
    public Dictionary<string, bool> stats = new Dictionary<string, bool>(); // Dictionary to hold all the stats for the tank that the rules use
    public CAA_RulesRBS rules = new CAA_RulesRBS(); // List of all the rules

    public HeuristicMode heuristicMode; /*!< <c>heuristicMode</c> Which heuristic used for find path. */
    GameObject patrolTargetObj; // Game object to point to where to patrol to next

    public float lowHealth = 30; // Variable of what is considered low health
    public float lowFuel = 50; // Variable of what is considered low fuel
    public float lowAmmo = 0; // Variable of what is considered low ammo

    /// <summary>
    ///WARNING, do not use void <c>Start()</c> function, use this <c>AITankStart()</c> function instead if you want to use Start method from Monobehaviour.
    ///Use this function to initialise your tank variables etc.
    /// </summary>
    public override void AITankStart()
    {
        InitialiseStateMachine();
        InitialiseStats();
        InitialiseRules();
        patrolTargetObj = GameObject.Instantiate(new GameObject("PatrolTarget"), Vector3.zero, Quaternion.identity);
        UpdateStats();
    }

    /// <summary>
    ///WARNING, do not use void <c>Update()</c> function, use this <c>AITankUpdate()</c> function instead if you want to use Start method from Monobehaviour.
    ///Function checks to see what is currently visible in the tank sensor and updates the Founds list. <code>First().Key</code> is used to get the first
    ///element found in that dictionary list and is set as the target, based on tank health, ammo and fuel. 
    /// </summary>
    public override void AITankUpdate()
    {
        UpdateStats();
    }

    void InitialiseStateMachine()
    {
        Dictionary<Type, CAA_BaseStateRBS> states = new Dictionary<Type, CAA_BaseStateRBS>();

        states.Add(typeof(CAA_PatrolStateRBS), new CAA_PatrolStateRBS(this));
        states.Add(typeof(CAA_ChaseStateRBS), new CAA_ChaseStateRBS(this));
        states.Add(typeof(CAA_AttackStateRBS), new CAA_AttackStateRBS(this));
        states.Add(typeof(CAA_EvadeStateRBS), new CAA_EvadeStateRBS(this));
        states.Add(typeof(CAA_ResupplyStateRBS), new CAA_ResupplyStateRBS(this));

        GetComponent<CAA_StateMachineRBS>().SetStates(states);
    }

    void InitialiseStats()
    {
        stats.Add("lowHealth", false);
        stats.Add("lowAmmo", false);
        stats.Add("lowFuel", false);
        stats.Add("targetSpotted", false);
        stats.Add("targetReached", false);
        stats.Add("noTargetInRange", false);
        stats.Add("collectableSpotted", false);
        stats.Add("collectableGone", false);
        stats.Add("atEvadeTarget", false);
        stats.Add("attackState", false);
        stats.Add("chaseState", false);
        stats.Add("evadeState", false);
        stats.Add("patrolState", false);
        stats.Add("resupplyState", false);
    }

    void InitialiseRules()
    {
        rules.AddRule(new CAA_RuleRBS("resupplyState", "collectableGone", typeof(CAA_PatrolStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("resupplyState", "collectableSpotted", typeof(CAA_ResupplyStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("patrolState", "collectableSpotted", typeof(CAA_ResupplyStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("evadeState", "collectableSpotted", typeof(CAA_ResupplyStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("chaseState", "lowHealth", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("attackState", "lowHealth", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("patrolState", "lowHealth", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("chaseState", "lowAmmo", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("attackState", "lowAmmo", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("patrolState", "lowAmmo", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("evadeState", "atEvadeTarget", typeof(CAA_PatrolStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("evadeState", "lowHealth", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("evadeState", "lowAmmo", typeof(CAA_EvadeStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("patrolState", "targetSpotted", typeof(CAA_ChaseStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("patrolState", "targetSpotted", typeof(CAA_PatrolStateRBS), CAA_RuleRBS.Predicate.nAnd));
        rules.AddRule(new CAA_RuleRBS("chaseState", "targetReached", typeof(CAA_AttackStateRBS), CAA_RuleRBS.Predicate.And));
        rules.AddRule(new CAA_RuleRBS("attackState", "noTargetInRange", typeof(CAA_PatrolStateRBS), CAA_RuleRBS.Predicate.And));
    }

    void UpdateStats()
    {
        if (VisibleEnemyTanks.Count > 0 || VisibleEnemyBases.Count > 0)
        {
            stats["targetSpotted"] = true;
        }
        else
        {
            stats["targetSpotted"] = false;
        }

        if (stats["targetSpotted"] == true)
        {
            if (VisibleEnemyTanks.Count > 0)
            {
                if (Vector3.Distance(this.transform.position, VisibleEnemyTanks.First().Key.transform.position) < 30f)
                {
                    stats["targetReached"] = true;
                    stats["noTargetInRange"] = false;
                }
                else
                {
                    stats["targetReached"] = false;
                    stats["noTargetInRange"] = true;
                }
            }
            else
            {
                if (Vector3.Distance(this.transform.position, VisibleEnemyBases.First().Key.transform.position) < 30f)
                {
                    stats["targetReached"] = true;
                    stats["noTargetInRange"] = false;
                }
                else
                {
                    stats["targetReached"] = false;
                    stats["noTargetInRange"] = true;
                }
            }
        }

        if (VisibleConsumables.Count > 0)
        {
            stats["collectableSpotted"] = true;
            stats["collectableGone"] = false;
        }
        else
        {
            stats["collectableSpotted"] = false;
            stats["collectableGone"] = true;
        }

        if (TankCurrentHealth <= lowHealth)
        {
            stats["lowHealth"] = true;
        }
        else
        {
            stats["lowHealth"] = false;
        }

        if (TankCurrentFuel <= lowFuel)
        {
            stats["lowFuel"] = true;
        }
        else
        {
            stats["lowFuel"] = false;
        }

        if (TankCurrentAmmo <= lowAmmo)
        {
            stats["lowAmmo"] = true;
        }
        else
        {
            stats["lowAmmo"] = false;
        }

        if (Vector3.Distance(gameObject.transform.position, patrolTargetObj.transform.position) <= 10f)
        {
            stats["atEvadeTarget"] = true;
        }
        else
        {
            stats["atEvadeTarget"] = false;
        }
    }

    /// <summary>
    ///WARNING, do not use void <c>OnCollisionEnter()</c> function, use this <c>AIOnCollisionEnter()</c> function instead if you want to use OnColiisionEnter function from Monobehaviour.
    ///Use this function to see if tank has collided with anything.
    /// </summary>
    public override void AIOnCollisionEnter(Collision collision)
    {
    }



    /*******************************************************************************************************       
    Below are a set of functions you can use. These reference the functions in the AITank Abstract class
    and are protected. These are simply to make access easier if you an not familiar with inheritance and modifiers
    when dealing with reference to this class. This does mean you will have two similar function names, one in this
    class and one from the AIClass. 
    *******************************************************************************************************/

    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject). If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void GeneratePathToWorldPoint(GameObject pointInWorld)
    {
        a_FindPathToPoint(pointInWorld);
    }

    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void GeneratePathToWorldPoint(GameObject pointInWorld, HeuristicMode heuristic)
    {
        a_FindPathToPoint(pointInWorld, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). 
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield. 
    ///If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed)
    {
        a_FollowPathToRandomPoint(normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToRandomPoint(normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate new random point
    /// </summary>
    public void GenerateNewRandomWorldPoint()
    {
        a_GenerateRandomPoint();
    }

    /// <summary>
    /// Stop Tank at current position.
    /// </summary>
    public void TankStop()
    {
        a_StopTank();
    }

    /// <summary>
    /// Continue Tank movement at last know speed and pointInWorld path.
    /// </summary>
    public void TankGo()
    {
        a_StartTank();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFaceWorldPoint(GameObject pointInWorld)
    {
        a_FaceTurretToPoint(pointInWorld);
    }

    /// <summary>
    /// Reset turret to forward facing position
    /// </summary>
    public void TurretReset()
    {
        a_ResetTurret();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject) and fire (has delay).
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFireAtPoint(GameObject pointInWorld)
    {
        a_FireAtPoint(pointInWorld);
    }

    /// <summary>
    /// Returns true if the tank is currently in the process of firing.
    /// </summary>
    public bool TankIsFiring()
    {
        return a_IsFiring;
    }

    /// <summary>
    /// Returns float value of remaining health.
    /// </summary>
    /// <returns>Current health.</returns>
    public float TankCurrentHealth
    {
        get
        {
            return a_GetHealthLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining ammo.
    /// </summary>
    /// <returns>Current ammo.</returns>
    public float TankCurrentAmmo
    {
        get
        {
            return a_GetAmmoLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining fuel.
    /// </summary>
    /// <returns>Current fuel level.</returns>
    public float TankCurrentFuel
    {
        get
        {
            return a_GetFuelLevel;
        }
    }

    /// <summary>
    /// Returns list of friendly bases. Does not include bases which have been destroyed.
    /// </summary>
    /// <returns>List of your own bases which are. </returns>
    public List<GameObject> MyBases
    {
        get
        {
            return a_GetMyBases;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject target, float distance) of visible targets (tanks in TankMain LayerMask).
    /// </summary>
    /// <returns>All enemy tanks currently visible.</returns>
    public Dictionary<GameObject, float> VisibleEnemyTanks
    {
        get
        {
            return a_TanksFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject consumable, float distance) of visible consumables (consumables in Consumable LayerMask).
    /// </summary>
    /// <returns>All consumables currently visible.</returns>
    public Dictionary<GameObject, float> VisibleConsumables
    {
        get
        {
            return a_ConsumablesFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject base, float distance) of visible enemy bases (bases in Base LayerMask).
    /// </summary>
    /// <returns>All enemy bases currently visible.</returns>
    public Dictionary<GameObject, float> VisibleEnemyBases
    {
        get
        {
            return a_BasesFound;
        }
    }

    public GameObject PatrolTargetObj
    {
        get
        {
            return patrolTargetObj;
        }
    }
}

