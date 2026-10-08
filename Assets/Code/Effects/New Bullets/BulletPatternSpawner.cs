using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletPatternSpawner : MonoBehaviour
{
    #region Bullet Spawn Variables

    //Relic Effect References
    [SerializeField] private RelicEffectSO relicEffectSO;
    [SerializeField] private GameObject bulletWarning;
    private BulletWarning bulletWarningScript;

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

    //Number of Bullets in a Pattern
    private int bAmount = 2;
    private int currentBullet = 1;

    //Spawned Bullet Warning
    private GameObject spawnedWarnBullet;

    //Bullet Spawn Point Vector
    private Vector3 spawnPoint;

    //Bullet Spawn cooldown (Bullet to Bullet) (Pattern to Pattern)
    private float b2bCooldown = 1f;
    private float p2pCooldown = 3f;
    private bool onCooldown = false;
    private float cooldownWaittime = 0f;
    private float timer;

    //Bullet Manager ready
    private bool bManagerReady = false;

    //Bullet Kill event
    //public event Action OnPattSpawnerDeath;

    #endregion

    //Awake was here to test PatternSetup(relicEffectSO)
    //private void Awake()
    //{
    //    PatternSetup(relicEffectSO);
    //}

    private void Update()
    {
        Timer();
        BulletPatternManger();
    }

    //Set up spawner with SO values
    public void PatternSetup(RelicEffectSO relicType)
    {
        relicEffectSO = relicType;

        bAmount = relicEffectSO.numberOfBullets;
        spawnType = relicEffectSO.spawnType;
        spawnLocationList = relicEffectSO.spawnLocationList;
        spawnHalfX = relicEffectSO.spawnAOEHalfX;
        spawnHalfY = relicEffectSO.spawnAOEHalfY;
        b2bCooldown = relicEffectSO.b2bCooldown;
        p2pCooldown = relicEffectSO.p2pCooldown;

        bManagerReady = true;
    }
    private void BulletPatternManger()
    {
        if (bManagerReady)
        {
            if (!onCooldown)
            {
                //Set Spawn Location
                SpawningSpot();
                //Spawn and Setup Bullet Warning
                SpawnBulletWarning();

                //Play correct Cooldown depending on bullet count
                if (currentBullet >= bAmount)
                {
                    //Play pattern to pattern cooldown
                    onCooldown = true;
                    timer = 0f;
                    cooldownWaittime = p2pCooldown;
                    currentBullet = 1;
                }
                else
                {
                    //Play bullet to bullet cooldown
                    onCooldown = true;
                    timer = 0f;
                    cooldownWaittime = b2bCooldown;
                    currentBullet += 1;
                }
            }
        }
        
    }

    private void SpawningSpot()
    {
        //Get Player position
        playerPos = this.gameObject.transform.position;

        //If: Spawn Locations
        if (spawnType == 0)
        {
            //Set a random spawn from selected spots
        }

        //If: Spawn Spot in AOE
        else if(spawnType == 1)
        {
            SpawningAOESetup();
            spawnPoint = new Vector3(UnityEngine.Random.Range(minX,maxX), UnityEngine.Random.Range(minY, maxY), playerPos.z);
        }
    }
    private void SpawnBulletWarning()
    {
        spawnedWarnBullet = Instantiate(bulletWarning, spawnPoint, Quaternion.identity);
        bulletWarningScript = spawnedWarnBullet.GetComponent<BulletWarning>();
        bulletWarningScript.WarnSetup(relicEffectSO, this.gameObject);

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
        if (timer >= cooldownWaittime)
        {
            onCooldown = false;
        }
    }

    public void RemovePassiveEffect()
    {
        Destroy(this.gameObject);
    }
}
