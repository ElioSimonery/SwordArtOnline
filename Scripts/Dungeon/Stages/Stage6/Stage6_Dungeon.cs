using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage6_Dungeon : DungeonBase 
{
	public Terrain terrain_4_2;
	public GameObject characterLocation6_1;
	public GameObject characterLocation6_2;
	public GameObject yuiObj;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;
	private bool location4_2runtimeSpawnStart = false;
	
	protected override void Start()
	{
		init ();
		SpawnDungeonMonsters ();
	}
	
	public override void Update ()
	{
		if (location4_2runtimeSpawnStart)
			base.Update ();

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
	
	public override void init()
	{
		base.init ();
		missions = new List<MissionBase> ();
		missions.Add (new ScriptMission ("BabelScripts/Stage6/stage6_prologue", false, true, false));
		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation6_1));
		
		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- 유이 -");
	}

	public void OnCollideYuiEvent()
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage6/stage6_yui", false, true, false));
	}

	public void OnCollideHouseEvent()
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage6/stage6_yui2", false, true, false));
		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation6_2));
		missions.Add (new DefenseMission (this, 20, "수집할 식량 : ", terrain_4_2));
		missions.Add (new ScriptMission ("BabelScripts/Stage6/stage6_ending", false, true, false));
		missions.Add (new ClearedMission (this));
	}

	public class DefenseMission : MissionBase
	{
		private Stage6_Dungeon ref_dungeon;
		private int count;
		private int szMonster;
		private string toast;
		private Terrain stage6_2Terrain;

		public DefenseMission(Stage6_Dungeon d, int _szMonster, string toastString, Terrain _stage6_2Terrain)
		{
			ref_dungeon = d;
			szMonster = _szMonster;
			toast = toastString;
			stage6_2Terrain = _stage6_2Terrain;
		}

		public override void start()
		{
			base.start ();
			NotificationManager.GetInstance ().toast ("20마리 이상 처치하여 식량 20을 수집하세요.");
			GameManager.AddGMComponent (new GMAccMonsterComponent ());
			ref_dungeon.terrain = stage6_2Terrain;
			ref_dungeon.location4_2runtimeSpawnStart = true;
			ref_dungeon.InitializeMonsterCounter ();
			ref_dungeon.SafeSpawning = false;
		}
		
		public override bool checkMission()
		{
			GMAccMonsterComponent counter = (GMAccMonsterComponent)GameManager.GetGMComponent (GMAccMonsterComponent.COMPONENT_NAME);
			count = szMonster - counter.getCount ();

			if(count > 0)
			{
				if(!NotificationManager.GetInstance().isToasting())
					NotificationManager.GetInstance ().toast (toast + " : " + count);
				return false;
			}

			return true;
		}
	}
}