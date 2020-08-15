using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage8_Dungeon : DungeonBase
{
	public GameObject characterLocation8;
	public GameObject characterLocation8_2;
	public GameObject stage1Location;
	public GameObject doorObjectForEnding;

	public GameObject hint2ObjForEnding;
	public GameObject lever4Object;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;
	private List<GameObject[]> backupMonsters;
	private List<GameObject> currentMonsters;

	protected override void Start()
	{
		init ();

		backupMonsters = new List<GameObject[]> ();
		currentMonsters = new List<GameObject> ();
		hint2ObjForEnding.SetActive (false);

		backupMonsters = SpawnDungeonMonsters ();
		foreach(GameObject[] monsters in backupMonsters)
		{
			foreach(GameObject m in monsters)
			{
				m.SetActive(false);
			}
		}
		CallOnTeleportNewSpawn ();
	}
	
	public override void Update ()
	{
		CheckLever4 ();

		if (missions.Count <= currentMission)
			return;
		if(!missions[currentMission].isInProgress)
		{
			timer += Time.deltaTime;
			if (timer >= missions[currentMission].startTime) 
			{
				timer = 0;
				missions[currentMission].start ();
			}
		}
		else // mission check.
		{
			if(missions[currentMission].checkMission())
				currentMission++;
		}
	}

	void CheckLever4()
	{
		if(lever4Object != null)
		if(lever4Object.GetComponent<LeverScript>().isLeverTriggered)
		{
			NotificationManager.GetInstance().toast("시스템 보안실 문이 열렸습니다. 어서 가보세요.");
			doorObjectForEnding.SetActive(false);
			lever4Object.SetActive(false);
		}
	}
	
	public override void init()
	{
		base.init ();
		missions = new List<MissionBase> ();
		missions.Add (new ScriptMission ("BabelScripts/Stage8/stage8_prologue", false, true));
		missions.Add (new OnEndScriptSpawnMission (this, GameManager.PlayerObject, characterLocation8));
		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- using System.Player.Mental; -");
	}

	public void CallOnWanderingScript()
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage8/stage8_dungeon", false, true, false));
	}

	public void CallOnCollideSystemEnding()
	{
		Destroy (lever4Object);
		IOManager.GetInstance().OpenInfinityMode();
		missions.Add (new ScriptMission ("BabelScripts/Stage8/stage8_ending", false, true, false));
		missions.Add (new PlayCinemaMission (CinemaManager.CINEMA.Stage8_ENDING));
		missions.Add (new OnEndScriptSpawnMission (this, GameManager.PlayerObject, characterLocation8_2));
		missions.Add (new ScriptMission ("BabelScripts/Stage8/stage8_trueEnding", false, true, false));
		missions.Add (new OnEndScriptSpawnMission (this, GameManager.PlayerObject, stage1Location));
		missions.Add (new ClearedMission (this));
		hint2ObjForEnding.SetActive (true);
	}

	// Respawn all
	public void CallOnTeleportNewSpawn()
	{
		// Remove current monsters.
		for(int i = 0; i < currentMonsters.Count; i++)
		{
			if(currentMonsters[i] != null)
				Destroy(currentMonsters[i]);
		}

		// Respawn from backupMonsters
		foreach(GameObject[] monsters in backupMonsters)
		{
			foreach(GameObject m in monsters)
			{
				GameObject copiedNewMonster = (GameObject)Instantiate(m);
				copiedNewMonster.SetActive(true);
				currentMonsters.Add(copiedNewMonster);
			}
		}
	}

	public void CallOnLeverFinished()
	{
		doorObjectForEnding.SetActive (false);
	}

	public void CallOnHint(string hintScriptFileName)
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage8/"+hintScriptFileName, false, true, false));
	}

}

