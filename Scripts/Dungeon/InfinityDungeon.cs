using UnityEngine;
using System.Collections;

public class InfinityDungeon : DungeonBase {

	protected override void Start()
	{
		init ();
		// Reposition Character
		SpawnCharacter ();
		// Spawn Normal Monsters
		SpawnDungeonMonsters ();
		// Spawn Rare Monsters
		SpawnRareMonsters ();
		// Spawn Boss
		if(Boss)
		{
			RandomizePosition (Boss);
		}
	}

	public override void init()
	{
		base.init ();
		if(GameManager.ioManager.currentStageNumber > 0)
			DifficultyPolicy (GameManager.ioManager.currentStageNumber);

		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("- " + GameManager.ioManager.currentStageNumber + " floor -");
	}

	public override void DifficultyPolicy(int dungeon_level)
	{
		float difficulty = 1f;

		for(int i = 0 ; i < dungeon_level; i++)
			difficulty *= 1.1f;

		for (int i = 0; i < runTimeSpawnSize.Length; i++)
			runTimeSpawnSize [i]++;
		foreach(GameObject m in Monsters)
			m.GetComponent<IMonsterBase>().upgrade(difficulty);
	}
}
