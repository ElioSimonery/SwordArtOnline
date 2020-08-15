using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage4_Dungeon : DungeonBase {
	public GameObject characterLocation4_1;
	public GameObject characterLocation4_2;
	public GameObject characterLocation4_3;
	public GameObject characterLocation4_4;

	public GameObject dragonLocation;

	public GameObject lisbethCharacter;
	public GameObject lisbethMonster;

	public Monster3DBase dragonMonster;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;
	
	protected override void Start()
	{
		init ();
		SpawnDungeonMonsters ();
	}
	
	public override void init()
	{
		base.init ();
		missions = new List<MissionBase> ();
		missions.Add (new ScriptMission ("BabelScripts/Stage4/stage4_prologue", false, true));

		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation4_1));
		missions.Add (new KillLisbethMission (lisbethMonster));
		missions.Add (new ScriptMission ("BabelScripts/Stage4/stage4_Lisbeth_after", false, true));

		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation4_2));
		missions.Add (new GoDragonMission(dragonMonster, lisbethCharacter));
		missions.Add (new ScriptMission ("BabelScripts/Stage4/stage4_Dragon_before", false, true, false));

		missions.Add (new DragonPhaseMission(this, dragonMonster, lisbethCharacter, dragonLocation, characterLocation4_3));
		missions.Add (new ScriptMission ("BabelScripts/Stage4/stage4_Dragon_after", false, true, false));

		missions.Add (new DragonDeadMission(this, dragonMonster, lisbethCharacter, characterLocation4_4));
		missions.Add (new ScriptMission ("BabelScripts/Stage4/stage4_returnhome", false, true, false));
		missions.Add (new MessageMission());
		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- 수정 -");
	}

	public void OnDestination()
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage4/stage4_ending", false, true, false));
		missions.Add (new ClearedMission (this));
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

	public class KillLisbethMission : MissionBase
	{
		public DungeonBase ref_dungeon;
		public GameObject target;
		public KillLisbethMission(GameObject lisbethMonster)
			: base()
		{ target = lisbethMonster; }	

		public override void start()
		{
			base.start ();
			NotificationManager.GetInstance().toast("리즈벳을 제압하세요.");
		}
		
		public override bool checkMission()
		{ 
			if(target == null)
				return true; 
			return false;
		}
	}

	public class GoDragonMission : MissionBase
	{
		Monster3DBase dragon;
		GameObject lisbethCharacter;
		public GoDragonMission(Monster3DBase _dragon, GameObject _lisbethCharacter)
			: base()
		{
			dragon = _dragon; 
			lisbethCharacter = _lisbethCharacter;
		}

		public override void start()
		{
			base.start ();
			lisbethCharacter.SetActive(true);
		}
		
		public override bool checkMission()
		{
			if(dragon.anim.GetBool("isWake"))
			{
				dragon.gameObject.SetActive(false);
				return true; 
			}
			return false;
		}
	}

	public class DragonPhaseMission : MissionBase
	{
		Monster3DBase dragon;
		GameObject dragonLocation;
		GameObject characterLocation4_3;
		GameObject lisbeth;

		DungeonBase ref_dungeon;
		public DragonPhaseMission(DungeonBase d, Monster3DBase _dragon, GameObject _lisbeth, GameObject _dragonLocation, GameObject _characterLocation4_3)
			: base()
		{
			dragon = _dragon; 
			ref_dungeon = d;
			dragonLocation = _dragonLocation;
			lisbeth = _lisbeth;
			characterLocation4_3 = _characterLocation4_3;
		}
		
		public override bool checkMission()
		{
			if(SpeechController.GetInstance().noSpeech)
				dragon.gameObject.SetActive(true);

			if(dragon.getHPRate() < 0.9f)
			{
				ref_dungeon.SpawnAt(dragon.gameObject, dragonLocation.transform.position);
				ref_dungeon.SpawnAt(GameManager.PlayerObject, characterLocation4_3.transform.position);
				ref_dungeon.SpawnAt(lisbeth, characterLocation4_3.transform.position+3*Vector3.right);
				return true;
			}
			return false;
		}
	}

	public class DragonDeadMission : MissionBase
	{
		Monster3DBase dragon;
		GameObject characterLocation4_4;
		DungeonBase ref_dungeon;
		GameObject lisbeth;

		public DragonDeadMission(DungeonBase d, Monster3DBase _dragon, GameObject _lisbeth, GameObject _characterLocation4_4)
			: base()
		{ 
			ref_dungeon = d;
			dragon = _dragon; 
			lisbeth = _lisbeth;
			characterLocation4_4 = _characterLocation4_4;
		}
		
		public override bool checkMission()
		{
			if(dragon.getHPRate() < 0.2f)
			{
				dragon.gameObject.SetActive(false);
				ref_dungeon.SpawnAt(GameManager.PlayerObject, characterLocation4_4.transform.position);
				ref_dungeon.SpawnAt(lisbeth, characterLocation4_4.transform.position + 3*Vector3.right);
				return true;
			}
			return false;
		}
	}
	public class MessageMission : MissionBase
	{
		public override bool checkMission ()
		{
			if(SpeechController.GetInstance().noSpeech)
			{
				NotificationManager.GetInstance ().toast ("집으로 돌아가세요.");
				return true;
			}
			return true;
		}
	}
}
