using UnityEngine;

public abstract class RangedWeaponBase : WeaponBase
{
	public float baseBulletSpeed; // bullet speed
	public float baseFireRate;

	public GameObject msl_obj;

	// 해당 장비 착용 시 케릭터의 공격 데이터 누적 함수
	public sealed override void onEquipApplyData(ICCharacterBase character)
	{
		RangerCharacterBase ranger = (RangerCharacterBase)character;
		ranger.addDamage (baseDamage);
		ranger.setBaseFireRate (baseFireRate);

		applyData (ranger);
	}

	public sealed override void unEquipApplyData(ICCharacterBase character)
	{
		RangerCharacterBase ranger = (RangerCharacterBase)character;
		ranger.addDamage (-baseDamage);

		applyData (ranger);
	}

	protected void applyData(RangerCharacterBase ranger)
	{
		ranger.calculateData ();
	}

	public override bool fire(GameObject character, Vector3 dir)
	{
		fire_timer += Time.deltaTime;
		RangerCharacterBase c = (RangerCharacterBase)character.GetComponent<CharacterControlHelper> ().c;

		if(fire_timer >= c.getFireRate())
		{
			
			GameObject new_bltObj = GameManager.CopyObjects (msl_obj);
			addNewBltData(c, new_bltObj);
			new_bltObj.SetActive(true);
			Vector3 v = character.transform.position;
			v.y += character.transform.lossyScale.y;
			new_bltObj.transform.position = v;
			new_bltObj.GetComponent<Rigidbody>().velocity = dir.normalized*baseBulletSpeed + character.GetComponent<Rigidbody>().velocity*0.1f;

			fire_timer = 0.0f;
			lastBullet.obj = new_bltObj;
			lastBullet.data = new_bltObj.GetComponent<BulletBase>();

			base.fire (character, dir);
			return true;
		}
		return false;
	}

	public abstract void addNewBltData(RangerCharacterBase ranger, GameObject new_bltObj);

	public void addNewBltDataImp(RangerCharacterBase ranger, GameObject new_bltObj, float ttl, string sound)
	{
		new_bltObj.AddComponent<BulletBase>().set (ranger.getCurrentAttack().mResultDamage, baseBulletSpeed, ttl, BulletBase.BULLET);
		SoundEffectManager.GetInstance().play(sound);
	}

	// is colling? true : false
	// call if character didn't firing with the current weapon.
	public virtual bool coolingTimerBackground()
	{
		if(fire_timer < baseFireRate)
		{
			fire_timer += Time.deltaTime;
			return true; // now cooling...
		}
		else
		{
			fire_timer = 0.0f;
			return false; // cooling finished!
		}
	}
}

