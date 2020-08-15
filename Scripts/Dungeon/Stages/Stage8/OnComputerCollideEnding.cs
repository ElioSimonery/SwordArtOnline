using UnityEngine;

public class OnComputerCollideEnding : MonoBehaviour
{
	public Stage8_Dungeon ref_dungeon;

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			ref_dungeon.CallOnCollideSystemEnding();
			Destroy(GetComponent<OnComputerCollideEnding>());
		}
	}
}

