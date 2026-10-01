using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    #region Bullet Variables

    //References
    [SerializeField] Rigidbody rb;
    [SerializeField] RelicEffectSO relicEffectSO;

    //Bullet Speed
    private float bSpeed = 1f;

    //Bullet Lifespan (in seconds)
    private float bLifespan = 5f;
    private float timer;

    //Bullet SpawnPoint
    private Vector3 spawnPoint;

    //Bullet Rotation
    [SerializeField] private float bRotateZ = 0f;

    #endregion

    private void Awake()
    {
        //Record SpawnPoint of Bullet
        spawnPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public void BulletSetup(RelicEffectSO relicSO)
    {
        //Call the Info from the SO
        relicEffectSO = relicSO;

        bSpeed = relicEffectSO.bulletSpeed;
        bLifespan = relicEffectSO.bulletLifespan;
        bRotateZ = relicEffectSO.bulletRotationZ;

    }

    private void Update()
    {
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

    private void BulletMove()
    {
        //Moves bullet (1f, 0f, 0f) * bullet speed * Time between frames
        //Turned by rotation of bullet (of which the warning sets)
        transform.Translate(Vector3.right * bSpeed * Time.deltaTime);
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
}
