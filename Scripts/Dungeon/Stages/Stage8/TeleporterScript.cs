using UnityEngine;


public class TeleporterScript : WarpScript
{
	public static int teleport_count;
	public Stage8_Dungeon ref_dungeon;
	private static bool hintPushed = false;
	protected virtual void OnTriggerEnter(Collider col)
	{
		base.OnTriggerEnter (col);
		if(col.tag == Common.TAG_PLAYER)
		{
			teleport_count++;
			ref_dungeon.CallOnTeleportNewSpawn();
		}

		if (teleport_count == 4) 
		{
			if(!hintPushed)
				ref_dungeon.CallOnWanderingScript();
			hintPushed = true;
		}
	}
}

