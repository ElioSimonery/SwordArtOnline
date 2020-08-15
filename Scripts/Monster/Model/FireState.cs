using UnityEngine;
using System.Collections;

public class FireState : SufferedState
{
	float rate = 1.0f;
	int fire_counter = 1;
	float subtracted;
	GameObject effect;
	public FireState(GameObject obj, float fire_rate, float duration)
		: base(obj, duration)
	{
		this.stateName = "Fire";
		rate = fire_rate;
	}

	public override bool OnSufferedStart ()
	{
		if (!base.OnSufferedStart ())
			return false;
		effect = GameManager.Instantiate (Resources.Load("effects/FireEffectPrefab")) as GameObject;
		effect.transform.parent = targetObj.transform;
		effect.transform.GetChild(0).transform.localPosition = BabelUtils.GetRandomVector2(1f);
		effect.transform.position = targetObj.transform.position;
	
		return true;
	}
	
	public override bool SufferingUpdate ()
	{
		if (!base.SufferingUpdate ())
			return false;

		if (currentTimer > fire_counter)
		{
			if(targetObj == null) return true;
			fire_counter++;
			int max_hp = targetObj.GetComponent<MonsterControlHelper>().m.getMaxHP();
			int hp = targetObj.GetComponent<MonsterControlHelper>().m.viewHP();
			subtracted = rate*max_hp;
		   	if(hp > (int)subtracted)
				targetObj.GetComponent<MonsterControlHelper>().m.hit((int)subtracted);
		}
		return true;
	}
	
	protected override void OnSufferedFinish ()
	{
		//nothing.
		GameManager.Destroy (effect);
	}
}
