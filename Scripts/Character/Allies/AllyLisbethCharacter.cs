using UnityEngine;
using System;

public class AllyLisbethCharacter : AllyMeleeCharacterBase
{
	protected override void Start()
	{
		base.Start ();
		GameObject lisbeth_weapon = GameManager.World.GetComponent<ItemFactory>().GetSingleRandomWeapon(1.0f, ItemWeaponsTable.SHORT_SWORD, gameObject);
		mModel.equipWeapon (ref lisbeth_weapon.GetComponent<ItemCube>().weapon);
		lisbeth_weapon.SetActive (false);
	}
}