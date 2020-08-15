using UnityEngine;
using System.Collections;

public class OnCollide4_2_end : MonoBehaviour {
	public Stage7_Dungeon ref_dungeon;

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			ref_dungeon.OnCollide7_2_end();
			gameObject.SetActive(false);
		}
	}
}
