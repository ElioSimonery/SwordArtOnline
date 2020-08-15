using UnityEngine;

public class ClearedMission : MissionBase
{
	public DungeonBase ref_dungeon;
	public GameObject characterLocation;
	public ClearedMission(DungeonBase d)
		: base()
	{ ref_dungeon = d; }
	
	public override bool checkMission()
	{ 
		if(SpeechController.GetInstance().noSpeech)
		{
			GameManager.missionCleared = true;
			return true; 
		}
		return false;
	}
}