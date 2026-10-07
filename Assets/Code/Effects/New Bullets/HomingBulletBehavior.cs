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
        base.Update();
        Vector3 desiredDiff = (spawnerRef.transform.position - transform.position) - moveDirection;
        moveDirection += desiredDiff * turnRate;
    }
    protected override void BulletMove()
    {
        base.BulletMove();
    }
}
