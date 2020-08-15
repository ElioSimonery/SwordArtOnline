using UnityEngine;
using System.Collections;

public class ColdState : SufferedState
{
	float rate = 1.0f;
	float subtracted;
	GameObject effect;
	public ColdState(GameObject obj, float slow_rate, float duration)
		: base(obj, duration)
	{
		this.stateName = "Cold";
		rate = slow_rate;
	}

	public override bool OnSufferedStart ()
	{
		if (!base.OnSufferedStart ())
			return false;

		float spd = targetObj.GetComponent<MonsterControlHelper>().m.getMoveSpeed();
		spd -= rate * spd;
		targetObj.GetComponent<MonsterControlHelper>().m.setMoveSpeed(spd);
		subtracted = rate*spd;

		effect = GameManager.Instantiate (Resources.Load("effects/ColdEffectPrefab")) as GameObject;
		effect.transform.parent = targetObj.transform;
		effect.transform.GetChild(0).transform.localPosition = BabelUtils.GetRandomVector2(1f);
		effect.transform.position = targetObj.transform.position;

		return true;
	}
	
	public override bool SufferingUpdate ()
	{
		if (!base.SufferingUpdate ())
			return false;
		return true;
	}
	
	protected override void OnSufferedFinish ()
	{
		GameManager.Destroy (effect);
		float pre_spd = targetObj.GetComponent<MonsterControlHelper> ().m.getMoveSpeed ();
		pre_spd += subtracted;
		targetObj.GetComponent<MonsterControlHelper> ().m.setMoveSpeed (pre_spd);
	}
}
