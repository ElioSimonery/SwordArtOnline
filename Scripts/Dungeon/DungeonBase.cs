using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// <<View Class>>
public abstract class DungeonBase : MonoBehaviour
{
	/* In Unity Interface - Dungeon info setting */
	public int 			floor;
	public GameObject 	characterBase;
	public bool 		SafeSpawning;		// Character will be safe from initial spawning monsters.
	public GameObject[] Monsters;			// Normal Monsters objects
	public int[] 		MonsterSpawnNumber;	// Spawn Initial Normal Monsters.

	public GameObject[] RareMonsters;		// Rare		
	public int[] RareMonsterSpawnNumber;	// Rare

	public GameObject	Boss;	// Single Boss will be spawned.

	// Spawning normal monsters option at runtime.
	public GameObject runTimeSpawnLocation = null;
	public int 		runTimeArraySize = 999;		// Size 
	public bool[] 	runTimeSpawn;				// Normal monsters are spawned at runtime. check your SpawningSafe option.
	public int[] 	runTimeSpawnSize;			// Spawning number
	public float[]	runTimeSpawnTimeInterval; 	// Spawning a monster time interval 
	public int[] runTimeMonsterMaximumLimitSize;// When it reach the limit, stop spawning for a while.

	// Terrain data
	public Terrain terrain;
	public float dungeon_border = 5f;
	public float spawnHeight = 5f;
	/* End Unity Interface */

	protected int[] realtimeMonsterSize;		// 
	protected int[] remainedSpawnSize; 			// Spawn Max size
	protected float[] spawn_timer;
	
	protected abstract void Start ();
	public virtual void addMission(MissionBase m)
	{
		Debug.LogWarning ("[DungeonBase] addMission base has been called. override this method.");
	}

	public virtual void Update ()
	{
		for(int i = 0; i < runTimeArraySize; i++) if(runTimeSpawn[i])
		{
			spawn_timer[i] += Time.deltaTime;

			if(spawn_timer[i] > runTimeSpawnTimeInterval[i])
			{
				spawn_timer[i] = 0f;
				if(remainedSpawnSize[i] == 0) continue;

				if(realtimeMonsterSize[i] < runTimeMonsterMaximumLimitSize[i])
				{
					// Spawn a monster.
					SpawnMonster(Monsters[i], runTimeSpawnLocation);
					realtimeMonsterSize[i]++;
					remainedSpawnSize[i]--;
				}
			}
		}
	}

	// Default Difficulty Policy 
	// Increases 10% per stage level
	// 1. Increases runtime spawn size
	// 2. Increases monster's attack damage & hp
	// 3. Increases monster's speed
	public virtual void DifficultyPolicy(int dungeon_level)
	{
		float difficulty = 1f + (float)dungeon_level * 0.1f;
		for (int i = 0; i < runTimeSpawnSize.Length; i++)
			runTimeSpawnSize [i]++;
		foreach(GameObject m in Monsters)
			m.GetComponent<IMonsterBase>().upgrade(difficulty);
	}

	public virtual void init()
	{
		realtimeMonsterSize = new int[Monsters.Length + RareMonsters.Length + 1];
		remainedSpawnSize = new int[runTimeArraySize];
		for (int i = 0; i < runTimeSpawnSize.Length; i++)
			remainedSpawnSize[i] = runTimeSpawnSize [i];
		spawn_timer = new float[runTimeArraySize];
	}

	/* Initial set dungeon functions */
	public List<GameObject[]> SpawnDungeonMonsters()
	{
		List<GameObject[]> ret = new List<GameObject[]> ();

		for(int i = 0; i < Monsters.Length; i++)
		{
			ret.Add(new GameObject[MonsterSpawnNumber[i]]);
			for(int j = 0; j < MonsterSpawnNumber[i]; j++)
			{
				// Spawn
				ret[i][j] = SpawnMonster(Monsters[i]);
				if(realtimeMonsterSize != null)
					realtimeMonsterSize[i]++;
			}
			Monsters[i].SetActive(false); // original monster hide
		}
		return ret;
	}

	public void InitializeMonsterCounter()
	{
		for (int i = 0; i < Monsters.Length + RareMonsters.Length + 1; i++)
			realtimeMonsterSize [i] = 0;
	}

	public int SpawnRareMonsters()
	{
		int count = 0;
		for(int i = 0; i < RareMonsters.Length; i++)
		{
			for(int j = 0; j < RareMonsterSpawnNumber[i]; j++)
			{
				// Spawn
				SpawnMonster(RareMonsters[i]);
				count++;
			}
			RareMonsters[i].SetActive(false); // original monster hide
		}
		return count;
	}

	public void SpawnCharacter()
	{
		Vector3 v = new Vector3 ();
		Vector3 tSize = terrain.terrainData.size; 
		Debug.Log ("Dungeon size : "+tSize);
		v.x = Random.Range(dungeon_border, tSize.x - dungeon_border);
		v.y = spawnHeight;
		v.z = Random.Range(dungeon_border, tSize.z - dungeon_border);
		Debug.Log ("spawned character to " + v);
		characterBase.transform.position = v;
	}

	/* Sugar */
	public GameObject SpawnMonster(GameObject monster, GameObject location = null)
	{
		// Spawn a monster.
		GameObject newMonster = (GameObject)Instantiate(monster);
		newMonster.SetActive (true);
		int cnt = 0;
		while(true)
		{
			if(location == null)
				RandomizePosition(newMonster);
			else 
			{
				newMonster.transform.position = location.transform.position;
				break;
			}

			if(!SafeSpawning) break;
			if(GameManager.IsSafePosition(characterBase, newMonster))
				break;
			cnt++;
			
			if(cnt > 10) 
			{
				Debug.LogWarning("Not enough to spawn. Character is not safe anymore.");
				break;
			}
		}
		Vector3 pos = newMonster.transform.position;
		pos.y += 2f; // resolve colliding.
		newMonster.transform.position = pos;

		return newMonster;
	}
	
	public void RandomizePosition(GameObject obj)
	{
		Vector3 v = new Vector3 ();
		Vector3 tSize = terrain.terrainData.size; 


		v.x = Random.Range(dungeon_border, tSize.x - dungeon_border);
		float height = spawnHeight;
		if (characterBase.transform.position.y < spawnHeight)
			height = characterBase.transform.position.y;
		v.y = height;
		v.z = Random.Range(dungeon_border, tSize.z - dungeon_border);

		v += terrain.gameObject.transform.position;

		obj.transform.position = v;
		Debug.Log ("[DungeonBase] "+obj+" randomized Pos = " + v);
	}

	public void SpawnAt(GameObject target, Vector3 pos)
	{
		if(target.GetComponent<Rigidbody>() != null)
			target.GetComponent<Rigidbody>().velocity = Vector3.zero;

		target.SetActive (true);
		target.transform.position = pos;
	}
}
