using UnityEngine;
using System.Collections;

public class ShortSword : MeleeWeaponBase {

	protected void Start ()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( true,
		                                                                         ItemWeaponsTable.SHORT_SWORD, gameObject);
		if(result)
			gameObject.SetActive (false);
		else
			GameManager.garbage.push (gameObject);
		
		init();
	}

}
