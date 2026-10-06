using UnityEngine;

public class HomingBulletBehavior : BulletBehavior
{

    public float turnRate = 1f;

    
    public override void BulletSetup(RelicEffectSO relicSO, GameObject spawner)
    {
        base.BulletSetup(relicSO, spawner);

        turnRate = relicEffectSO.bulletRotationRate;
    }
    protected override void Update()
    {
        Vector3 desiredDiff = (spawnerRef.transform.position - transform.position) - moveDirection;
        moveDirection += desiredDiff * turnRate;
        base.Update();
    }
    protected override void BulletMove()
    {
        base.BulletMove();
    }
}
