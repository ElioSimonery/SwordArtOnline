using UnityEngine;
using System.Collections;

public class HolyState : SufferedState
{
	float damage = 1.0f;
	GameObject effect;
	public HolyState(GameObject obj, float holy_damage, float duration)
		: base(obj, duration)
	{
		this.stateName = "Holy";
		damage = holy_damage;
	}
	
	public override bool OnSufferedStart ()
	{
		if (!base.OnSufferedStart ())
			return false;
		effect = GameManager.Instantiate (Resources.Load("effects/HolyEffectPrefab")) as GameObject;
		effect.transform.parent = targetObj.transform;
		effect.transform.GetChild(0).transform.localPosition = BabelUtils.GetRandomVector2(1f);
		effect.transform.position = targetObj.transform.position;

		targetObj.GetComponent<MonsterControlHelper>().m.hit ((int)damage);
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
		//nothing.
		GameManager.Destroy (effect);
	}
}
