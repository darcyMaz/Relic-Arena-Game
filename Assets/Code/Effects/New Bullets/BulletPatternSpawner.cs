using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternSpawner : MonoBehaviour
{
    #region Bullet Spawn Variables

    //Player Position
    private Vector3 playerPos;

    //Spawn Locations or AOE
    private List<Vector3> spawnLocationList;
    private float spawnAOEX;
    private float spawnAOEY;

    private int spawnType;
    #region Spawn Type Legend
    // 0 = Set Spawn Locations List
    // 1 = Spawning AOE
    #endregion

    //Number of Bullets in a Pattern
    private int bAmount = 2;
    private int currentBullet = 1;

    //Spawned Bullet
    private GameObject recentBullet;

    //Bullet Spawn Point Vector
    private Vector3 spawnPoint;

    //Bullet Spawn cooldown (Bullet to Bullet) (Pattern to Pattern)
    private float b2bCooldown = 1f;
    private float p2pCooldown = 3f;

    private float timer;

    #endregion

    private void Awake()
    {
        
    }

    private void Update()
    {
        
    }

    private void BulletPattern()
    {

    }

    private void Timer()
    {
        timer += Time.deltaTime;
    }
}
