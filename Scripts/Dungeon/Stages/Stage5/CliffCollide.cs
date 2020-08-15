using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CliffCollide : MonoBehaviour
{
	public Stage5_Dungeon ref_dungeon;
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			ref_dungeon.OnCollideCliffEvent();
			gameObject.SetActive(false);
		}
	}
}