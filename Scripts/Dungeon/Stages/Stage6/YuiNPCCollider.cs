using UnityEngine;

public class YuiNPCCollider : MonoBehaviour
{
	public Stage6_Dungeon ref_dungeon;

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			ref_dungeon.OnCollideYuiEvent ();
			gameObject.SetActive (false);
		}
	}
}