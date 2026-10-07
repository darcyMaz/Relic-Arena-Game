using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    #region Bullet Variables

    //References
    [SerializeField] Rigidbody rb;
    [SerializeField] public RelicEffectSO relicEffectSO;
    public GameObject spawnerRef;

    private bool bSetup = false;

    //Bullet Speed
    private float bSpeed = 1f;

    //Bullet Lifespan (in seconds)
    private float bLifespan = 5f;
    private float timer;

    //Bullet SpawnPoint
    private Vector3 spawnPoint;

    //Bullet Rotation
    private float bRotateZ = 0f;
    public Vector3 moveDirection = Vector3.right;

    //Bullet Scale
    private float bScaleX = 1f;
    private float bScaleY = 1f;

    #endregion

    private void Awake()
    {
        //Record SpawnPoint of Bullet
        spawnPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public virtual void BulletSetup(RelicEffectSO relicSO, GameObject spawner)
    {
        //Call the Info from the SO
        relicEffectSO = relicSO;
        spawnerRef = spawner;

        bSpeed = relicEffectSO.bulletSpeed;
        bLifespan = relicEffectSO.bulletLifespan;
        bRotateZ = relicEffectSO.bulletRotationZ;

        bScaleX = relicEffectSO.bulletScaleX;
        bScaleY = relicEffectSO.bulletScaleY;
        transform.localScale = new Vector3(bScaleX, bScaleY, 1f);

        //Bullet is setup
        bSetup = true;
    }

    protected virtual void Update()
    {
        LoseRelicDestoryBullet();
        BulletTimer();
        BulletMove();
    }

    private void BulletTimer()
    {
        //Destroy Bullet when Lifespan exceeded
        if (timer > bLifespan)
        {
            Destroy(this.gameObject);
        }
        //Add to timer
        timer += Time.deltaTime;
    }

    protected virtual void BulletMove()
    {
        //Moves bullet (1f, 0f, 0f) * bullet speed * Time between frames
        //Turned by rotation of bullet (of which the warning sets)
        transform.Translate(moveDirection.normalized * bSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player;
        if (other.TryGetComponent(out player))
        {
            // the other collider has a player component on it
            Debug.Log("Hit Player");
        }
        else
        {
            // the other collider does NOT have a player component on it
            Debug.Log("Hit not a Player");
        }
    }

    private void LoseRelicDestoryBullet()
    {
        if (bSetup)
        {
            if (spawnerRef == null)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
