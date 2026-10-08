using UnityEngine;

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

    private float bWarnLifespan = 5f;
    private float timer = 0f;

    private float bScaleX = 1f;
    private float bScaleY = 1f;

    private void Update()
    {
        WarningTimer();
        WarningArrowRotation();
    }

    public void WarnSetup(RelicEffectSO relicType, GameObject spawner)
    {
        relicEffectSO = relicType;
        spawnerRef = spawner;

        bSpeed = relicEffectSO.bulletSpeed;
        bRotation = relicEffectSO.bulletRotationZ;
        bRotateType = relicEffectSO.rotationType;
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
        if (warnSetup)
        {
            if (bRotateType == 0)
            {
                //Warning Bullets points 1 direction
                transform.rotation = Quaternion.Euler(0f, 0f, bRotation);
            }
            else if (bRotateType == 1 || bRotateType == 2)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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
        bulletBehaviorScript.BulletSetup(relicEffectSO, spawnerRef);
        
        Destroy(this.gameObject);
    }
}
