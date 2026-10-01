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
    public List<Vector3> spawnLocationList;
    public float spawnAOEHalfX = 2;
    public float spawnAOEHalfY = 2;
    public float spawnAOEOffsetX = 0;
    public float spawnAOEOffsetY = 0;
    [Space]
    public float b2bCooldown = 1f;
    public float p2pCooldown = 2f;

    [Header("Bullet")]
    public float bulletSpeed = 1f;
    public float bulletLifespan = 5f;
    [Space]
    public float bulletScaleX = 1f;
    public float bulletScaleY = 1f;
    [Space]
    public float bulletRotationZ = 0f;

    [Header("Bullet Warning")]
    public float warnBulletLifespan = 2f;
}
