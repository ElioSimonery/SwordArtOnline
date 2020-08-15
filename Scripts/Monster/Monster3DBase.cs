using UnityEngine;
using System.Collections;

public class Monster3DBase : IMonsterBase {
	
	protected override bool tryMove()
	{
		if(base.tryMove ())
		{
			view.gazePlayer3D();
			return true;
		}
		return false;
	}

	protected override bool tryAttack(GameObject target)
	{
		if(base.tryAttack (target))
		{
			view.gazePlayer3D();
			return true;
		}
		return false;
	}
	// Fixed drop position.y, monster pivot is floor. 
	public override GameObject[] dropRandomItem()
	{
		GameObject[] dropped = base.dropRandomItem ();
		foreach(GameObject item in dropped)
		{
			if(item == null) continue;
			Vector3 pos = item.transform.position;
			pos.y += 0.5f;
			item.transform.position = pos;
		}
		return dropped;
	}
}
