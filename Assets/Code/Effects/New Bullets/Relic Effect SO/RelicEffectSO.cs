using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicEffectSO", menuName = "Scriptable Objects/RelicEffectSO")]
public class RelicEffectSO : ScriptableObject
{
    [Header("Pattern Spawner")]
    public int numberOfBullets = 2;
    [Space]
    public int spawnType = 0;
    #region Spawn Type Legend
    // 0 = Set Spawn Locations List
    // 1 = Spawning AOE
    #endregion
    public List<Vector3> spawnLocationList;
    public float spawnAOEHalfX = 2;
    public float spawnAOEHalfY = 2;
    [Space]
    public float b2bCooldown = 1f;
    public float p2pCooldown = 2f;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 1f;
    public float bulletLifespan = 5f;
    [Space]
    public float bulletScaleX = 1f;
    public float bulletScaleY = 1f;
    [Space]
    public int rotationType = 1;
    #region Rotation Type Legend
    // 0 = Set Rotation for all Bullets
    // 1 = Rotation follow Player when aiming
    // 2 = Rotation follow Player constant
    // 3 = Set Rotation per set spawnLocation
    #endregion
    public float bulletRotationZ = 0f;
    public float bulletRotationRate = 1f;

    [Header("Bullet Warning")]
    public float warnBulletLifespan = 2f;
}
