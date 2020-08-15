using UnityEngine;
using System.Collections;

public class InvenSlotController : MonoBehaviour {
	public float equipByTapTime = 1.2f;
	private UIButtonMessage viewOn;
	private UIButtonMessage viewOff;
	private bool initialized = false;
	private bool tapping = false;
	private float tapTimer;

	// Use this for initialization
	void Awake () {
		if(!initialized)
			initialize ();
		gameObject.SetActive (false);

	}

	void Update()
	{
		if(tapping)
		{
			if(tapTimer >= equipByTapTime)
			{
				GameManager.inventoryObject.
					GetComponent<InventoryScript>().OnTouched(gameObject);

				tapTimer = 0f;
				tapping = false;
			}
			EquipProgressController.getInstance().gameObject.SetActive(true);
			EquipProgressController.getInstance().setProgress(tapTimer / equipByTapTime);			

			tapTimer += Time.deltaTime;
		}
		else tapTimer = 0f;
	}
	
	private void initialize()
	{
		viewOn = gameObject.AddComponent<UIButtonMessage> ();
		viewOn.target = gameObject;
		viewOn.trigger = UIButtonMessage.Trigger.OnPress;
		viewOn.functionName = "TapOn";
		
		viewOff = gameObject.AddComponent<UIButtonMessage> ();
		viewOff.target = gameObject;
		viewOff.trigger = UIButtonMessage.Trigger.OnRelease;
		viewOff.functionName = "TapOff";
		
		initialized = true;
	}

	public void init(GameObject inventoryObj)
	{
		if (viewOn == null)
			initialize ();
	}

	void TapOn(GameObject spriteObj)
	{
		tapping = true;
		GameManager.descriptor.displayItem (gameObject);
		GameManager.descriptorObject.SetActive (true);

		WeaponBase inven_weapon = spriteObj.GetComponent<SecondParent> ().virtualParent.GetComponent<ItemCube> ().weapon;

		if (checkStashItem (spriteObj))
			return;

		if (inven_weapon != null)
		{
			// User Trying unequip
			if (inven_weapon == GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.getCurrentWeapon ()) 
			{
				if(LabelChange.getInstance().mode != LabelChange.DISCARD)
					EquipProgressController.getInstance ().setMode (EquipProgressController.MODE.UnEquip, WeaponBase.WeaponType.NULL);
				EquipProgressController.getInstance ().playSound ();
				return;
			}
			EquipProgressController.getInstance ().setMode (EquipProgressController.MODE.Equip, inven_weapon.type);
		}
		else
			EquipProgressController.getInstance ().setMode (EquipProgressController.MODE.Equip, WeaponBase.WeaponType.NULL);

		if(LabelChange.getInstance().mode == LabelChange.DISCARD)
			EquipProgressController.getInstance ().setMode (EquipProgressController.MODE.Discard, WeaponBase.WeaponType.NULL);

		EquipProgressController.getInstance ().playSound ();
	}
	
	void TapOff()
	{
		tapping = false;
		EquipProgressController.getInstance ().stopSound();
		EquipProgressController.getInstance ().gameObject.SetActive (false);
	}

	private UISprite checkSprite = null;
	private bool checkStashItem(GameObject spriteObj)
	{
		// stash item -> check item and return.
		ItemCube item = spriteObj.GetComponent<SecondParent> ().virtualParent.GetComponent<ItemCube> ();
		if (Application.loadedLevelName != Stash.GetInstance().SceneName)
			return false;

		if(item.isStash())
		{
			if(item.isPicked()) 
			{
				item.setStashPickItem(false);
				Destroy (checkSprite);
				checkSprite = null;
				StashDAO.GetInstance().pickCount--;
			}
			else
			{
				if(StashDAO.GetInstance().maxPickCount > StashDAO.GetInstance().pickCount)
				{
					item.setStashPickItem(true);
					StashDAO.GetInstance().pickCount++;
					if(checkSprite == null)
					{
						checkSprite = gameObject.AddComponent<UISprite>();
						checkSprite.atlas = (Resources.Load("Resources/ui/OnItemCheck") as GameObject).GetComponent<UISprite>().atlas;
						checkSprite.depth = 43;
						checkSprite.spriteName = "checked";
					}
				}
				else
					NotificationManager.GetInstance().toast("동시에 최대 "+StashDAO.GetInstance().maxPickCount+"개 가져갈 수 있습니다.");
			}
			return true;
		}
		return false;
	}
}
