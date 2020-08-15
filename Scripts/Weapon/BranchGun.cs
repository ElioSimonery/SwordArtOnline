using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BranchGun : RangedWeaponBase {
	public static string BULLET = "BranchGunBullet";
	public string BULLET_SOUND = "branch_arrow";
	public static float BULLET_TTL = 2.0f;

	void Start()
	{
		if(!SoundEffectManager.GetInstance ().contains(BULLET_SOUND))
			SoundEffectManager.GetInstance ().register (BULLET_SOUND, GetComponent<AudioSource> ());
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( true,
		                                                                         ItemWeaponsTable.BRANCH_GUN, gameObject);
		if(result)
			gameObject.SetActive (false);
		else
			GameManager.garbage.push (gameObject);
		init ();
	}

	public override void init()
	{
		if(msl_obj == null)
			msl_obj = GameObject.Find(BULLET);
	}

	public override void initData(ItemRank rank)
	{
		init ();
		base.initData (rank);

		baseDamage += (int)rank;
		baseBulletSpeed += (int)rank;
	}
	
	public override void addNewBltData (RangerCharacterBase ranger, GameObject new_bltObj)
	{
		addNewBltDataImp (ranger, new_bltObj, BULLET_TTL, BULLET_SOUND);
	}

	// is colling? true : false
	// call if character didn't firing current weapon.
	public override bool coolingTimerBackground()
	{
		return base.coolingTimerBackground ();
	}
}
