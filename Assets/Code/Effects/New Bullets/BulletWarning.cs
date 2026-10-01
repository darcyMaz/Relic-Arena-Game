using UnityEngine;

public class BulletWarning : MonoBehaviour
{
    
    [SerializeField] private RelicEffectSO relicEffectSO;
    [SerializeField] private GameObject warnArrow;
    [SerializeField] private GameObject bullet;

    private Vector3 warnPos;

    private GameObject spawnedBullet;
    private BulletBehavior bulletBehaviorScript;

    private float bSpeed;
    private float bRotation;

    private float bWarnLifespan = 5f;
    private float timer = 0f;

    private void Update()
    {
        WarningTimer();
    }

    public void WarnSetup(RelicEffectSO relicType)
    {
        relicEffectSO = relicType;

        bSpeed = relicEffectSO.bulletSpeed;
        bRotation = relicEffectSO.bulletRotationZ;
        bWarnLifespan = relicEffectSO.warnBulletLifespan;

        warnPos = this.transform.position;
        WarningArrowRotation();
    }

    private void WarningArrowRotation()
    {
        if (bSpeed == 0)
        {
            warnArrow.SetActive(false);
        }
        transform.rotation = Quaternion.Euler(0f, 0f, bRotation);
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
        GameObject spawnedBullet = Instantiate(bullet, warnPos, Quaternion.Euler(0f, 0f, bRotation));
        bulletBehaviorScript = spawnedBullet.GetComponent<BulletBehavior>();
        bulletBehaviorScript.BulletSetup(relicEffectSO);
        
        Destroy(this.gameObject);
    }
}
