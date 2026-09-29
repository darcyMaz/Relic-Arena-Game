using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    #region Bullet Properties

    //References
    [SerializeField] Rigidbody rb;

    //Bullet Speed
    private float bSpeed = 1f;

    //Bullet Lifespan (in seconds)
    private float bLifespan = 5f;
    private float timer;

    //Bullet SpawnPoint
    private Vector3 spawnPoint;

    //Bullet Movement
    private Vector3 movement;

    //Bullet Rotation
    [SerializeField] private float bRotateZ = 0f;
    private Quaternion rotation;

    #endregion

    
    private void Awake()
    {
        //Record SpawnPoint of Bullet
        spawnPoint = new Vector3(transform.position.x,transform.position.y,transform.position.z);

        //Define Bullet Trajectory Direction
        rotation = Quaternion.Euler(0, 0, bRotateZ);
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
        //Record movement of bullet based on Time * Speed relative to SpawnPoint
        movement = new Vector3(spawnPoint.x + timer * bSpeed, spawnPoint.y, spawnPoint.z);

        //Rotate calculated Vector to direct Bullet
        Vector3 rotatedVector = rotation * movement;

        //Apply transformation
        rb.transform.position = rotatedVector;
    }
}
