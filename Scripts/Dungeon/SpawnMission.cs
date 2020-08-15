using UnityEngine;

public class SpawnMission : MissionBase
{
	public GameObject[] spawnedMonsters;
	private GameObject targetObj;
	private int sz;
	public SpawnMission(DungeonBase d, GameObject _monsterObj, int spawnSize)
		: base()
	{
		targetObj = _monsterObj;
		sz = spawnSize;
		spawnedMonsters = new GameObject[sz];
		for(int i = 0; i < sz; i++)
			spawnedMonsters[i] = d.SpawnMonster(targetObj);
	}
	public override bool checkMission() { return true; }
}
