using UnityEngine;
using System.Collections;

// <<Control class>>
public abstract class MeleeWeaponBase : WeaponBase {
	public GameObject[] meleeColliderObject;
	public float[] validDuration;
	public AudioClip[] swingEffect;

	public override void init ()
	{
		foreach(AudioClip s in swingEffect)
		{
			SoundEffectManager.GetInstance ().register (s.name, s);
		}
		return;
	}

	// 해당 장비 착용 시 케릭터의 공격 데이터 산출 함수
	public sealed override void onEquipApplyData(ICCharacterBase character)
	{
		character.addDamage (baseDamage);
		character.calculateData ();
		//melee.increaseDamage ... etc.
	}
	
	public sealed override void unEquipApplyData(ICCharacterBase character)
	{
		character.addDamage(-baseDamage);
		//melee.increaseDamage
		character.calculateData ();
	}

	// Current Weapon Attack Policy. override and call it if u wanna specialize.
	public virtual void AttackPolicy(GameObject characterObj, MeleeCharacterBase c, int damage, Vector3 dir)
	{
		// any custom attack policy for this gameobject.
		GameObject colliderObj = Instantiate (meleeColliderObject [c.getCurrentCombo()]) as GameObject;
		colliderObj.SetActive (true);

		Vector3 pos = characterObj.transform.position + dir.normalized*1f;
		pos.y += characterObj.transform.lossyScale.y;
		colliderObj.transform.position = pos;
		colliderObj.transform.rotation = GameManager.PlayerObject.transform.rotation;
		colliderObj.transform.GetChild(0).gameObject.AddComponent<BulletBase>().set (damage, 0, validDuration[c.getCurrentCombo()], BulletBase.MELEE);

		lastBullet.obj = colliderObj;
		lastBullet.data = colliderObj.transform.GetChild(0).gameObject.GetComponent<BulletBase>();
		SoundEffectManager.GetInstance().play(swingEffect[c.getCurrentCombo()].name);
	}

	// Common Melee weapon attack Algorithm.
	public override bool fire(GameObject character, Vector3 dir)
	{
		MeleeCharacterBase c = (MeleeCharacterBase)character.GetComponent<CharacterControlHelper> ().c;
		bool ret = false;
		// PreDelay Check
		if (c.getCurrentAttack().preDelayFinished()) 
		{
			// Attack.
			if(!c.getCurrentAttack().hit)
			{
				AttackPolicy(character, c, c.getCurrentAttack().attack(), dir);
				base.fire(character, dir); // call socket item effect callbacks
				//c.comboTimer = 0f; // attacked -> initialize combo timer
				ret = true; 
			}
		}
		// PostDelay
		if(c.getCurrentAttack().postDelayFinished())
		{
			c.getCurrentAttack().init(); // finished this attack. so initialize internal data.
			c.prepareNextAttack ();
		}
		coolingDelay (c);
		return ret;
	}

	/* these functions must be called only once at the frame.*/
	public void coolingDelay(GameObject character)
	{
		MeleeCharacterBase c = (MeleeCharacterBase)character.GetComponent<CharacterControlHelper> ().c;
		coolingDelay (c);
	}

	public void coolingDelay(MeleeCharacterBase c) // call once.
	{
		c.getCurrentAttack().timer += Time.fixedDeltaTime;
	}
}
