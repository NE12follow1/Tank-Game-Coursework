using System.Collections.Generic;
using UnityEngine;

public class SmartTank : MonoBehaviour
{
    // Tank stats
    public float TankCurrentHealth = 100f;
    public float TankMaxHealth = 100f;
    public float TankCurrentFuel = 100f;
    public float TankCurrentAmmo = 50f;

    public GameObject PlayerTank;

    // Visible objects
    public List<GameObject> VisibleConsumables = new List<GameObject>();
    public List<GameObject> VisibleEnemyBases = new List<GameObject>();
    public List<GameObject> VisibleEnemyTanks = new List<GameObject>();

    // Tank actions
    public void MoveToPosition(Vector3 target) { }
    public void ResupplyState() { }
    public void FollowPathToRandomWorldPoint() { }
    public void GenerateNewRandomWorldPoint() { }
    public void AttackTarget(GameObject target) { }
}
