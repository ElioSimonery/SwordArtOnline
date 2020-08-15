using UnityEngine;

public class OnCollideHint : MonoBehaviour
{
	public Stage8_Dungeon ref_dungeon;
	public string hintScriptFileName = "stage8_hint1";
	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			ref_dungeon.CallOnHint(hintScriptFileName);
			gameObject.SetActive(false);
		}
	}
}