using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// <<View Control Class>> Stage1.
public class Stage1_Dungeon : DungeonBase
{
	public GameObject	characterLocationObj;
	public GameObject	bossLocationObj;
	public GameObject	stairObj;
	public GameObject	doorObj;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;

	protected override void Start()
	{
		init ();
		SpawnAt (GameManager.PlayerObject, characterLocationObj.transform.position);
		SpawnAt (Boss, bossLocationObj.transform.position);
		// Spawn Normal Monsters
		SpawnDungeonMonsters ();
		// Spawn Rare Monsters
		SpawnRareMonsters ();
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

		missions.Add (new ScriptMission ("BabelScripts/Stage1/stage1_prologue", false, true));
		missions.Add (new BossHPMission (Boss));
		missions.Add (new ScriptMission ("BabelScripts/Stage1/stage1_boss_before", true, true, false));
		missions.Add (new BossDeadMission(Boss, stairObj));
		missions.Add (new ScriptMission ("BabelScripts/Stage1/stage1_ending", true, true));
		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- Aincrad 1st floor -");
		stairObj.SetActive (false);
		doorObj.GetComponent<Animator> ().SetBool ("Open", false);
	}

	public override void addMission(MissionBase m)
	{
		missions.Add(m);
	}

	public class BossHPMission : MissionBase
	{
		private GameObject boss;
		private int max_hp;
		public BossHPMission(GameObject bossObj) 
		{
			boss = bossObj; 
		}
		public override void start()
		{
			base.start ();
			max_hp = boss.GetComponent<Boss> ().getMaxHP();

		}
		public override bool checkMission()
		{
			if (boss == null)
				return false; // not spawned yet.

			int hp = boss.GetComponent<Boss>().viewHP();
			if(((float)hp/(float)max_hp) <= 0.65)
			{
				GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.setLock(true);
				return true;
			}
			return false;
		}
	}

	public class BossDeadMission : MissionBase
	{
		public GameObject boss;
		public GameObject stair;
		public BossDeadMission(GameObject bossObj, GameObject stairObj){ boss = bossObj; stair = stairObj; }

		public override void start()
		{
			base.start ();
		}

		public override bool checkMission()
		{
			if (SpeechController.GetInstance().noSpeech) 
				GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.setLock(false);
			// Dead boss
			if(boss == null)
			{
				GameManager.missionCleared = true;
				NotificationManager.GetInstance().toast("보스가 죽었습니다. 다음 계단으로 이동할 수 있습니다.");
				stair.SetActive(true);
				return true;
			}
			return false;
		}
	}
}