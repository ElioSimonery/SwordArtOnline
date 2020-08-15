using UnityEngine;
using System;

public class AllyAsunaCharacter : AllyMeleeCharacterBase
{
	protected override void Start()
	{
		base.Start ();
		GameObject asuna_weapon = GameManager.World.GetComponent<ItemFactory>().GetSingleRandomWeapon(1.0f, ItemWeaponsTable.SHORT_SWORD, gameObject);
		mModel.equipWeapon (ref asuna_weapon.GetComponent<ItemCube>().weapon);
		asuna_weapon.SetActive (false);
		mModel.addDamage (1000);
	}
}