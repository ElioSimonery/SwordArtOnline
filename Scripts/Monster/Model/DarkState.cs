using UnityEngine;
using System.Collections;

public class DarkState : SufferedState
{
	float rate = 0f;
	private int initial_hp;
	GameObject effect;

	public DarkState(GameObject obj, float heal_rate, float duration)
		: base(obj, duration)
	{
		this.stateName = "Dark";
		rate = heal_rate;
	}
	
	public override bool OnSufferedStart ()
	{
		if (!base.OnSufferedStart ())
			return false;
		effect = GameManager.Instantiate (Resources.Load("effects/DarkEffectPrefab")) as GameObject;
		effect.transform.parent = targetObj.transform;
		effect.transform.GetChild(0).transform.localPosition = BabelUtils.GetRandomVector2(1f);
		effect.transform.position = targetObj.transform.position;
		initial_hp = targetObj.GetComponent<MonsterControlHelper> ().m.viewHP ();
		return true;
	}
	
	protected override void OnSufferedFinish ()
	{
		GameManager.Destroy (effect);
		//create heal addon effect.
		int heal = (int)((initial_hp - targetObj.GetComponent<MonsterControlHelper> ().m.viewHP())*rate);
		if (heal < 1) heal = 1;
		if (GameManager.PlayerObject)
		{
			GameObject stealer = GameManager.Instantiate(Resources.Load("effects/LifeStealerPrefab") as GameObject) as GameObject;
			stealer.transform.position = targetObj.transform.position;
			GameManager.PlayerObject.GetComponent<ICCharacterBase> ().hit (-heal);
		}
	}
}
