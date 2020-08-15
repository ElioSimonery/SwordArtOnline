using UnityEngine;


public class Bomb2DMonster : Monster2DBase
{
	protected override bool tryAttack(GameObject target)
	{
		if(base.tryAttack(target))
		{
			OnDeadDestroy();
			target.GetComponent<Rigidbody>().AddExplosionForce(40000f, transform.position, 100);
			FireOnDead.OnFire(gameObject, true);
			return true;
		}
		return false;
	}
}

