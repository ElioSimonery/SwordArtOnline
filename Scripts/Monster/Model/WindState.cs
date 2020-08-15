using UnityEngine;
using System.Collections;

public class WindState : SufferedState
{
	float rate = 1.0f;
	GameObject effect;
	public WindState(GameObject obj, float knockback_rate, float duration)
		: base(obj, duration)
	{
		this.stateName = "Wind";
		rate = knockback_rate;
	}

	public override bool OnSufferedStart ()
	{
		if (!base.OnSufferedStart ())
			return false;

		Vector3 nv = targetObj.transform.position - GameManager.PlayerObject.transform.position;
		nv.Normalize ();
		targetObj.GetComponent<Rigidbody>().AddForce (nv*rate, ForceMode.Impulse);
		targetObj.GetComponent<MonsterControlHelper>().m.initState ();
		effect = GameManager.Instantiate (Resources.Load("effects/WindEffectPrefab")) as GameObject;
		effect.transform.parent = targetObj.transform;
		effect.transform.GetChild(0).transform.localPosition = BabelUtils.GetRandomVector2(1f);
		effect.transform.position = targetObj.transform.position;

		return true;
	}
	
	public override bool SufferingUpdate ()
	{
		if (!base.SufferingUpdate ())
			return false;

		// indirective stun effect.
		targetObj.GetComponent<MonsterControlHelper>().m.setWakeState(false);

		return true;
	}
	
	protected override void OnSufferedFinish ()
	{
		GameManager.Destroy (effect);
	}
}
