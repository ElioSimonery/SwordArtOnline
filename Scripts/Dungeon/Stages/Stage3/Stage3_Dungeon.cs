using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage3_Dungeon : DungeonBase {
	public GameObject characterLocation3_1;
	public GameObject characterLocation3_2;
	public GameObject characterLocation3_3;
	public GameObject gorila;
	public GameObject bunker;
	public int spawnSize = 8;
	protected int gorilaCount;
	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;
	private bool runtimeSpawnStart = false;

	SpawnMission gorilaSpawningMission;
	protected override void Start()
	{
		init ();
	}

	public override void init()
	{
		base.init ();
		missions = new List<MissionBase> ();
		missions.Add (new ScriptMission ("BabelScripts/Stage3/stage3_Prologue", false, true));
		missions.Add (new OnEndScriptSpawnMission (this, GameManager.PlayerObject, characterLocation3_1));
		missions.Add (gorilaSpawningMission = new SpawnMission (this, gorila, spawnSize));
		missions.Add (new GorilaKillMission(this));

		missions.Add (new ScriptMission ("BabelScripts/Stage3/stage3_Gorila_after", false, true));
		missions.Add (new OnEndScriptSpawnMission (this, GameManager.PlayerObject, characterLocation3_2));
		missions.Add (new FlowerKillMission(this));

		missions.Add (new ScriptMission ("BabelScripts/Stage3/stage3_Flower_after", false, true));
		missions.Add (new OnEndScriptSpawnMission (this, GameManager.PlayerObject, characterLocation3_3));
		missions.Add (new BunkerMission(this));

		missions.Add (new ScriptMission ("BabelScripts/Stage3/stage3_ending", false, true));
		missions.Add (new ClearedMission(this));
		gorilaCount = spawnSize;
		runtimeSpawnStart = false;
		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- 소생의 꽃 -");
	}

	public override void Update ()
	{
		int cnt = 0;
		foreach(GameObject g in gorilaSpawningMission.spawnedMonsters)
			if(g != null) cnt++;
		gorilaCount = cnt;
		if (runtimeSpawnStart)
		{
			base.Update ();
		}

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

	public class GorilaKillMission : MissionBase
	{
		public Stage3_Dungeon ref_dungeon;

		public GorilaKillMission(Stage3_Dungeon d)
			: base()
		{ ref_dungeon = d; }

		public override void start ()
		{
			base.start ();

		}

		public override bool checkMission()
		{ 
			NotificationManager.GetInstance ().toast ("고릴라 섬멸 : "+ ref_dungeon.gorilaCount);
			if(ref_dungeon.gorilaCount <= 0)
				return true; 
			return false;
		}
	}

	public class FlowerKillMission : MissionBase
	{
		public Stage3_Dungeon ref_dungeon;

		public FlowerKillMission(Stage3_Dungeon d)
			: base()
		{ ref_dungeon = d; }

		public override bool checkMission()
		{ 
			if(ref_dungeon.Boss == null)
				return true; 
			return false;
		}
	}

	public class BunkerMission : MissionBase
	{
		public Stage3_Dungeon ref_dungeon;

		public BunkerMission(Stage3_Dungeon d)
			: base()
		{ ref_dungeon = d; }
		
		public override void start ()
		{
			base.start ();
			NotificationManager.GetInstance ().toast ("꽃을 보호하고 적의 기지를 부수세요.");
			ref_dungeon.runtimeSpawnStart = true;
		}
		
		public override bool checkMission()
		{ 
			if(ref_dungeon.bunker == null)
				return true; 
			return false;
		}
	}
}
