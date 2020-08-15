using UnityEngine;

public class DestLocation : MonoBehaviour
{
	public Stage4_Dungeon dungeon;
	private bool triggered = false;

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			if(triggered) return;

			if(dungeon.dragonMonster == null || !dungeon.dragonMonster.gameObject.activeInHierarchy)
			{
				dungeon.OnDestination ();
				triggered = true;
			}
		}
	}
}

