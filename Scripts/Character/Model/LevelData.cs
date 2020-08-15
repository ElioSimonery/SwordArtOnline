using UnityEngine;
using System.Collections;

// <<Entity Class>>
public class LevelData
{
	int level;
	int exp;
	PairData health;
	PairData mana;
	LevelUpTable t;
	public int Level{ get{return level; } }

	public void getExp(int _exp)
	{

	}

	private int damage;
	public LevelData ()
	{

	}

	public class PairData
	{
		int max_point;
		int regeneration;
	}

}

