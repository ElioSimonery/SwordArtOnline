using UnityEngine;
using System.Collections;

public class ChestEvent : MonoBehaviour {
	public Stage2_Dungeon dungeon;
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			if(dungeon.OnSpawnTriggerStart())
				Destroy (this); // disable calling multiple script
		}
	}
}
