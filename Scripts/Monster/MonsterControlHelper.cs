using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterControlHelper : MonoBehaviour {
	public IMonsterBase m;
	// Use this for initialization
	void Start () {
		if(m == null)
		{
			Debug.Log("[MonsterControlHelper] reference is null. auto finder has activated.");
			m = MonsterClassFinder();

			if(m == null)
			{
				Debug.LogError("[MonsterControlHelper] reference is null : " + gameObject.name);
			}

		}
	}

	public IMonsterBase MonsterClassFinder()
	{
		List<IMonsterBase> mons = new List<IMonsterBase> ();

		// Generic monsters
		mons.Add (GetComponent<Monster2DBase> ());
		mons.Add (GetComponent<Monster3DBase> ());
		mons.Add (GetComponent<StaticMonster3D>());

		// Specific monsters
		mons.Add (GetComponent<Cradile3DMonster>());
		mons.Add (GetComponent<Bomb2DMonster>());
		mons.Add (GetComponent<Mantis3D>());
		mons.Add (GetComponent<GrimReaper3DBoss> ());
		mons.Add (GetComponent<Boss>());

		foreach(IMonsterBase m in mons)
		{
			if(m != null)
				return m;
		}
		return null;
	}
}
