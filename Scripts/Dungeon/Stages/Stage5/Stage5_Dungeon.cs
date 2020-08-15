using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage5_Dungeon : DungeonBase {
	public GameObject characterLocation4_1;
	public GameObject characterLocation4_2;
	public GameObject asunaAlly;
	public GameObject cradileMonster;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission = 0;
	
	protected override void Start()
	{
		init ();
		SpawnDungeonMonsters ();
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
		missions.Add (new ScriptMission ("BabelScripts/Stage5/stage5_prologue", false, true));
		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation4_1));

		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- 배신 -");
	}

	public void OnCollideCliffEvent()
	{
		missions.Add (new ScriptMission ("BabelScripts/Stage5/stage5_cradile_kirito", false, true, false));
		missions.Add (new OnEndScriptSpawnMission(this, GameManager.PlayerObject, characterLocation4_2));
		missions.Add (new BetrayMission());
		missions.Add (new ScriptMission ("BabelScripts/Stage5/stage5_cradile_asuna", false, true, false));
		missions.Add (new AsunaAllyMission(asunaAlly, cradileMonster));
		missions.Add (new ScriptMission ("BabelScripts/Stage5/stage5_ending", false, true, false));
		missions.Add (new ClearedMission(this));
	}

	public class BetrayMission : MissionBase
	{
		
		public override void start()
		{
			base.start ();
		}
		
		public override bool checkMission()
		{
			if(GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.viewHPRate() < 0.2f)
				return true;
			
			return false;
		}
	}

	public class AsunaAllyMission : MissionBase
	{
		GameObject asuna;
		GameObject cradile;
		public AsunaAllyMission(GameObject asunaObj, GameObject cradileObj)
		{
			asuna = asunaObj;
			cradile = cradileObj;
		}

		public override void start()
		{
			base.start ();
			ICCharacterBase c = GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c;
			c.setHP (c.viewMaxHP());
			cradile.GetComponent<Cradile3DMonster> ().applyHalfDamage = false;
		}
		
		public override bool checkMission() {
			if(SpeechController.GetInstance().noSpeech)
				asuna.SetActive (true);

			if(cradile == null)
				return true;
			return false;
		}
	}

}
