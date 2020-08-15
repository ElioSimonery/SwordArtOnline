using UnityEngine;
using System.Collections;

public class GrimReaper3DBoss : Monster3DBase {
	public GameObject spawningBombMonster; // Bomb Monster
	public Stage7_Dungeon ref_dungeon;

	private int bossPhase;
	private float HPphaser = 0.8f;
	private bool applyHalfDamage = false;

	private bool scriptProcessed = false;

	public override bool hit (int damage)
	{

		bool ret = base.hit (damage);

		if(!model.isDead)
		{
			if (getHPRate () < HPphaser) 
			{
				SpawnOnPhase(++bossPhase);
				HPphaser -= 0.2f;
			}
		}

		if (getHPRate() < 0.1f) 
		{
			model.setHP(model.MAX_HP);
			applyHalfDamage = true;

			if (ref_dungeon == null) return ret;
			if(scriptProcessed) return ret;
			ref_dungeon.OnBossMaxHP();
			scriptProcessed = true;
		}

		return ret;
	}

	protected override bool tryAttack(GameObject target)
	{
		if(base.tryAttack(target))
		{
			if(!applyHalfDamage) return true;
			ICCharacterBase player = GameManager.PlayerObject.GetComponent<ICCharacterBase> ();
			if(player != null)
			{ 
				int halfDamage = player.viewHP() / 2;
				GameManager.PlayerObject.GetComponent<ICCharacterBase> ().hit (halfDamage);
			}
			return true;
		}
		return false;
	}

	void SpawnOnPhase(int phase)
	{
		if (ref_dungeon == null) return;
		if (phase > 4)	return;

		for(int i = 0; i < phase*2+3; i++)
		{
			GameObject mon = ref_dungeon.SpawnMonster (spawningBombMonster);
			mon.GetComponent<Bomb2DMonster> ().forceToWakeUp ();
		}
		Debug.Log ("[GrimReaper] boss Spawned monsters at phase " + phase);
	}

}
