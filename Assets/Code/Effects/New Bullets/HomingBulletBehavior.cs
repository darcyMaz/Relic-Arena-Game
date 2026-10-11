using UnityEngine;

public class HomingBulletBehavior : BulletBehavior
{

    public float turnRate = 1f;

    
    public override void BulletSetup(RelicEffectSO relicSO, GameObject spawner, float listRotation)
    {
        base.BulletSetup(relicSO, spawner, listRotation);

        turnRate = relicEffectSO.bulletRotationRate;
    }
    protected override void Update()
    {
        base.Update();
        if (spawnerRef != null)
        {
            Vector3 desiredDiff = (spawnerRef.transform.position - transform.position) - moveDirection;
            moveDirection += desiredDiff * turnRate;
        }
    }
    protected override void BulletMove()
    {
        base.BulletMove();
    }
}
