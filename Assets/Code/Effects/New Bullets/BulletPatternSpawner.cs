using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletPatternSpawner : MonoBehaviour
{
    #region Bullet Spawn Variables

    //Relic Effect Scriptable Object
    [SerializeField] RelicEffectSO relicEffectSO;

    //Player Position
    private Vector3 playerPos;

    //Spawn Locations or AOE

    private int spawnType;
    #region Spawn Type Legend
    // 0 = Set Spawn Locations List
    // 1 = Spawning AOE
    #endregion

    private List<Vector3> spawnLocationList;

    private float spawnHalfX;
    private float spawnHalfY;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float spawnOffsetX;
    private float spawnOffsetY;

    //Number of Bullets in a Pattern
    private int bAmount = 2;
    private int currentBullet = 1;

    //Spawned Bullet Warning
    private GameObject recentWarnBullet;

    //Bullet Spawn Point Vector
    private Vector3 spawnPoint;

    //Bullet Spawn cooldown (Bullet to Bullet) (Pattern to Pattern)
    private float b2bCooldown = 1f;
    private float p2pCooldown = 3f;

    private float timer;

    #endregion

    private void Awake()
    {
        //TEMPORARY, to be moved into ieffectable
        PatternSetup();
    }

    private void Update()
    {
        
    }

    //Set up spawner with SO values
    private void PatternSetup()
    {
        bAmount = relicEffectSO.numberOfBullets;
        spawnType = relicEffectSO.spawnType;
        spawnLocationList = relicEffectSO.spawnLocationList;
        spawnHalfX = relicEffectSO.spawnAOEHalfX;
        spawnHalfY = relicEffectSO.spawnAOEHalfY;
        spawnOffsetX = relicEffectSO.spawnAOEOffsetX;
        spawnOffsetY = relicEffectSO.spawnAOEOffsetY;
        b2bCooldown = relicEffectSO.b2bCooldown;
        p2pCooldown = relicEffectSO.p2pCooldown;
        
        SpawningSpot();
    }

    private void SpawningSpot()
    {
        //Get Player position
        playerPos = this.gameObject.transform.position;

        //If: Spawn Locations
        if (spawnType == 0)
        {

        }

        //If: Spawn Spot in AOE
        else if(spawnType == 1)
        {
            SpawningAOESetup();
            spawnPoint = new Vector3(Random.Range(minX,maxX), Random.Range(minY, maxY), playerPos.z);
        }
    }

    private void SpawningAOESetup()
    {
        //Setting up Range for possible spawns
        minX = playerPos.x - spawnHalfX;
        maxX = playerPos.x + spawnHalfX;
        minY = playerPos.y - spawnHalfY;
        maxY = playerPos.y + spawnHalfY;
    }

    private void Timer()
    {
        timer += Time.deltaTime;
    }
}
