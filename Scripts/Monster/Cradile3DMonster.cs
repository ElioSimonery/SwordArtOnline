using UnityEngine;

public class Cradile3DMonster : Monster3DBase
{
	public bool applyHalfDamage = true;
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
}
