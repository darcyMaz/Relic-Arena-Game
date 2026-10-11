using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

public class BulletWarning : MonoBehaviour
{
    
    [SerializeField] private RelicEffectSO relicEffectSO;
    [SerializeField] private GameObject warnArrow;
    private GameObject spawnerRef;

    private Vector3 warnPos;
    private bool warnSetup = false;

    private GameObject spawnedBullet;
    private BulletBehavior bulletBehaviorScript;

    private float bSpeed;
    private float bRotation;
    private int bRotateType;
    private Vector3 aimDirection;
    private int chosenListRotate;

    private float bWarnLifespan = 5f;
    private float timer = 0f;

    private float bScaleX = 1f;
    private float bScaleY = 1f;

    private void Update()
    {
        LoseRelicDestoryWarnBullet();
        WarningTimer();
        WarningArrowRotation();
    }

    public void WarnSetup(RelicEffectSO relicType, GameObject spawner, int listSpawn)
    {
        relicEffectSO = relicType;
        spawnerRef = spawner;

        bSpeed = relicEffectSO.bulletSpeed;
        bRotation = relicEffectSO.bulletRotationZ;
        bRotateType = relicEffectSO.rotationType;
        chosenListRotate = listSpawn;
        bWarnLifespan = relicEffectSO.warnBulletLifespan;

        bScaleX = relicEffectSO.bulletScaleX;
        bScaleY = relicEffectSO.bulletScaleY;
        transform.localScale = new Vector3(bScaleX, bScaleY, 1f);

        warnPos = this.transform.position;
        WarningArrow();
        warnSetup = true;
    }

    private void WarningArrow()
    {
        if (bSpeed == 0)
        {
            warnArrow.SetActive(false);
        }
        else
        {
            warnArrow.SetActive(true);
        }
    }
    private void WarningArrowRotation()
    {
        if (warnSetup && spawnerRef != null)
        {
            //Warning Bullet points 1 direction
            if (bRotateType == 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, bRotation);
            }
            //Warning Bullet points towards Player
            else if (bRotateType == 1 || bRotateType == 2)
            {
                aimDirection = spawnerRef.transform.position - warnPos;
                if (aimDirection.y >= 0)
                {
                    // If y is positive, then calculate the angle between (1,0) and the direction.
                    bRotation = Vector2.Angle(new Vector3(1f, 0f, 0f), aimDirection);
                }
                // If y is negative, then calculate the angle between (-1,0) and the direction and add 180.
                else
                {
                    // Otherwise, set the current position and return it.
                    bRotation = Vector2.Angle(new Vector3(-1f, 0f, 0), aimDirection) + 180;
                }
                transform.rotation = Quaternion.Euler(0f, 0f, bRotation);
            }
            //Warning Bullet point direction depending on spawn location
            else if (bRotateType == 3)
            {
                bRotation = relicEffectSO.spawnLocationRoationList[chosenListRotate];
                transform.rotation = Quaternion.Euler(0f, 0f, bRotation);
            }
            else
            {
                Debug.Log("rotation type not set properly");
            }
        }
    }

    private void WarningTimer()
    {
        //Destroy Warning when Lifespan exceeded
        if (timer > bWarnLifespan)
        {
            SpawnBullet();
        }
        //Add to timer
        timer += Time.deltaTime;
    }

    private void SpawnBullet()
    {
        //1. Spawns Bullet, 2. Executes Bullet Setup, 3. Explodes
        GameObject spawnedBullet = Instantiate(relicEffectSO.bulletPrefab, warnPos, Quaternion.identity);
        bulletBehaviorScript = spawnedBullet.GetComponent<BulletBehavior>();
        bulletBehaviorScript.BulletSetup(relicEffectSO, spawnerRef, bRotation);
        
        Destroy(this.gameObject);
    }

    private void LoseRelicDestoryWarnBullet()
    {
        if (warnSetup)
        {
            if (spawnerRef == null)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
