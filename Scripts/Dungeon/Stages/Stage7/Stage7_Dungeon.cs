using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage7_Dungeon : DungeonBase 
{
	public GameObject warp4_2Obj;
	public GameObject soldierMonster;
	public int soldierMonsterSpawnSize;
	public Terrain terrain7_2;
	public Terrain terrain7_3;

	public GameObject characterLocation7_1;
	public GameObject characterLocation7_2;
	public GameObject characterLocation7_3;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;
	
	protected override void Start()
	{
		init ();

		for(int i = 0; i < soldierMonsterSpawnSize; i++)
			SpawnMonster (soldierMonster);
	}
	
	public override void Update ()
	{
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
		// 7_1
		missions.Add (new ScriptMission ("BabelScripts/Stage7/stage7_prologue", false, true));
		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation7_1));
		missions.Add (new SoldierAlliminationMission(soldierMonsterSpawnSize-10, "경비병 처치: ", warp4_2Obj));
		missions.Add (new ScriptMission ("BabelScripts/Stage7/stage7_soldier", false, true));
		// 7_2
		//missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation7_2));
		missions.Add (new Spawn7_2Mission (this, terrain7_2));

		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- AI -");
	}

	public void OnCollide7_2_mid()
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage7/stage7_dungeon_1", false, true, false));
	}

	public void OnCollide7_2_end()
	{
		// 7_3
		missions.Add (new ScriptMission ("BabelScripts/Stage7/stage7_dungeon_2", false, true, false));
		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation7_3));
		terrain = terrain7_3;
	}

	public void OnBossMaxHP()
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage7/stage7_yui", false, true, false));
		missions.Add (new SuccessOnYUIMission (Boss));
		missions.Add (new ScriptMission ("BabelScripts/Stage7/stage7_reaper", false, true, false));
		missions.Add (new OnEndScriptSpawnMission (this, GameManager.PlayerObject, characterLocation7_3));
		missions.Add (new PlayCinemaMission ());
		missions.Add (new ScriptMission ("BabelScripts/Stage7/stage7_ending", false, true, false));
		missions.Add (new ClearedMission(this));
	}

	public class SoldierAlliminationMission : MissionBase
	{
		private GameObject warp4_2Obj;
		private int count;
		private int szMonster;
		private string toast;
		
		public SoldierAlliminationMission(int _szMonster, string toastString, GameObject _warp4_2Obj)
		{
			szMonster = _szMonster;
			toast = toastString;
			warp4_2Obj = _warp4_2Obj;
			warp4_2Obj.SetActive(false);
		}
		
		public override void start()
		{
			base.start ();
			NotificationManager.GetInstance ().toast ("불량 경비병들을 섬멸하세요!");
			GameManager.AddGMComponent (new GMAccMonsterComponent ());
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
			NotificationManager.GetInstance ().toast ("미션 완료! 북쪽 입구로 들어가세요.");
			warp4_2Obj.SetActive (true);
			return true;
		}
	}

	public class Spawn7_2Mission : MissionBase
	{
		Stage7_Dungeon ref_dungeon;
		Terrain terrain7_2;

		public Spawn7_2Mission(Stage7_Dungeon d, Terrain _terrain4_2)
		{
			ref_dungeon = d;
			terrain7_2 = _terrain4_2;
		}

		public override bool checkMission()
		{
			ref_dungeon.terrain = terrain7_2;
			ref_dungeon.SpawnDungeonMonsters ();
			return true;
		}
	}

	public class SuccessOnYUIMission : MissionBase
	{
		private GameObject boss;
		public SuccessOnYUIMission(GameObject _bossObj)
		{
			boss = _bossObj;
		}
		public override bool checkMission()
		{
			if(GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.viewHPRate() < 0.2f)
			{
				boss.SetActive(false);
				return true;
			}
			return false;
		}
	}

	public class PlayCinemaMission : MissionBase
	{
		public override bool checkMission()
		{
			CinemaManager.PlayCinema (CinemaManager.CINEMA.Stage7_YUI);
			return true;
		}
	}
}
