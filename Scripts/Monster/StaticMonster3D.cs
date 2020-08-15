using UnityEngine;
using System.Collections;

public class StaticMonster3D : IMonsterBase {
	
	// Fixed drop position.y, monster pivot is floor. 
	public override GameObject[] dropRandomItem()
	{
		GameObject[] dropped = base.dropRandomItem ();
		foreach(GameObject item in dropped)
		{
			Vector3 pos = item.transform.position;
			pos.y += 0.5f;
			item.transform.position = pos;
		}
		return dropped;
	}
}
