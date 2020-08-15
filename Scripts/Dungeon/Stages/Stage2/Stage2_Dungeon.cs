using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage2_Dungeon : DungeonBase {
	public GameObject characterLocation2_1Obj;
	public GameObject characterLocation2_2Obj;
	public GameObject treasureObj;
	public GameObject mantisObj;
	public GameObject mantisLocationObj;
	public GameObject golemObj;
	public GameObject nextObj;
	public int golemSpawnSize = 20;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;
	private bool golemSpawn = false;

	
	protected override void Start()
	{
		init ();
		SpawnAt (GameManager.PlayerObject, characterLocation2_1Obj.transform.position);
		GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.setLock(true);
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
		missions.Add (new ScriptMission ("BabelScripts/Stage2/stage2_prologue", false, true));
		missions.Add (new MantisMission (this, mantisObj, mantisLocationObj));
		missions.Add (new MantisClearedMission (mantisObj));
		missions.Add (new ScriptMission ("BabelScripts/Stage2/stage2_mantis_after", false, true));
		missions.Add (new Next2_2Mission(this, characterLocation2_2Obj));
		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- 죽음의 미궁 -");
	}

	public class MantisMission : MissionBase
	{
		private Stage2_Dungeon ref_dungeon;
		private GameObject mantisObj;
		private GameObject mantisLocationObj;
		public MantisMission(Stage2_Dungeon d, GameObject _mantisObj, GameObject _mantisLocationObj)
			: base()
		{
			ref_dungeon = d;
			mantisObj = _mantisObj;
			mantisLocationObj = _mantisLocationObj;
		}
		
		public override bool checkMission()
		{
			if (SpeechController.GetInstance().noSpeech) 
			{
				ref_dungeon.SpawnAt (mantisObj, mantisLocationObj.transform.position);
				GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.setLock(false);
				return true;
			}
			return false;
		}
	}

	public class MantisClearedMission : MissionBase
	{
		private GameObject mantisObj;
		public MantisClearedMission(GameObject _mantisObj) : base()
		{
			mantisObj = _mantisObj;
		}

		public override bool checkMission()
		{
			if(mantisObj == null) // dead
				return true;
			return false;
		}
	}

	public class Next2_2Mission : MissionBase
	{
		private Stage2_Dungeon ref_dungeon;
		private GameObject pos2_2;
		public Next2_2Mission(Stage2_Dungeon d, GameObject new2_2Location) : base()
		{
			ref_dungeon = d;
			pos2_2 = new2_2Location;

		}

		public override bool checkMission() 
		{ 
			ref_dungeon.SpawnAt(GameManager.PlayerObject, pos2_2.transform.position); 
			return true; 
		}
	}

	public bool OnSpawnTriggerStart()
	{
		if(golemSpawn == false)
		{
			missions.Add(new SpawnMission(this, golemObj, golemSpawnSize));
			NotificationManager.GetInstance ().toast ("죽지 않고 입구로 도망치세요!");
			nextObj.SetActive(true);
		}
		return golemSpawn = true; 
	}
	public bool OnEndingScriptStart()
	{
		if(golemSpawn)
		{
			missions.Add(new ScriptMission("BabelScripts/Stage2/stage2_sachi_dead", true, true, false));
			missions.Add (new NextStageMission ());
			return true;
		}
		return false;
	}

	public class NextStageMission : MissionBase
	{
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
}
