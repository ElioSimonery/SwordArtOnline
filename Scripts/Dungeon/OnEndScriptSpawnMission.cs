using UnityEngine;

public class OnEndScriptSpawnMission : MissionBase
{
	public DungeonBase ref_dungeon;
	public GameObject target;
	public GameObject location;
	public OnEndScriptSpawnMission(DungeonBase d, GameObject targetObj, GameObject locationObj)
		: base()
	{
		ref_dungeon = d;
		target = targetObj;
		location = locationObj;
	}
	
	public override bool checkMission()
	{ 
		if(SpeechController.GetInstance().noSpeech)
		{
			ref_dungeon.SpawnAt(target, location.transform.position);
			return true; 
		}
		return false;
	}

}