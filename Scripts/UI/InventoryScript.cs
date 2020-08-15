using UnityEngine;
using System.Collections;

public class InventoryScript : MonoBehaviour {
	public static int RELIC_SLOT = 0;
	public static int MAGIC_SLOT = 1;
	public static int MECHA_SLOT = 2;
	public GameObject windowUIObject;
	public GameObject weaponUIObject;
	public GameObject descriptorObj;
	public GameObject verifyObj;
	public GameObject labelChangeObject;

	protected readonly string GET_ITEM = "\n아이템을 영구 보존합니까?";
	protected bool isOpenState = false;

	public GameObject equipSlotObj;
	public GameObject[] voidPartSlotObj;
	public GameObject voidSlotObj;

	//public float slot_size;
	public float inven_slot_margin = 15;

	protected GameObject[,] slotObjs;
	private GameObject[,] weaponSlotObjs = new GameObject[3, 3];

	private WeaponBase prev_weapon_toClear = null;

	protected virtual void Awake()
	{
		slotObjs = new GameObject[Inventory.MAX_HEIGHT, Inventory.MAX_WIDTH];
		//init ui
		initializeSlots (Inventory.MAX_HEIGHT, Inventory.MAX_WIDTH);
		// init weapon part slots
		initializeWeaponInfo ();
	}

	protected void initializeSlots(int height, int width)
	{
		for(int i = 0; i < height; i++)
		{
			for(int j = 0; j < width; j++)
			{
				GameObject slot = (GameObject)Instantiate(voidSlotObj);
				Vector3 pos = slot.transform.position;
				Vector3 scale = slot.transform.lossyScale;
				
				slot.transform.parent = gameObject.transform;
				pos.x += (float)j*(inven_slot_margin+voidSlotObj.transform.localScale.x);
				pos.y -= (float)i*(inven_slot_margin+voidSlotObj.transform.localScale.y);
				
				slot.transform.localPosition = pos;
				slot.transform.localScale = scale;
				slotObjs[i, j] = slot;
			}
		}
	}

	protected void initializeWeaponInfo()
	{
		for(int types = 0; types < 3; types++)
		{
			for(int i = 0; i < 3; i++)
			{
				GameObject slot =(GameObject) Instantiate(voidPartSlotObj[types]);
				Vector3 pos = slot.transform.position;
				Vector3 scale = slot.transform.lossyScale;
				
				slot.transform.parent = weaponUIObject.transform;
				pos.x += (float)i*(inven_slot_margin+voidPartSlotObj[types].transform.localScale.x);
				slot.transform.localPosition = pos;
				slot.transform.localScale = scale;
				weaponSlotObjs[types,i] = slot;
				slot.SetActive(false);
			}
		}
		//set false original data.
		for(int i = 0; i < 3; i++)
			voidPartSlotObj[i].SetActive(false); 
		voidSlotObj.SetActive(false);
	}

	// OnProcess
	public void OnTouched(GameObject spriteObj)
	{
		descriptorObj.SetActive(false);
		switch(labelChangeObject.GetComponent<LabelChange>().mode)
		{
		case LabelChange.EQUIP:
			OnEquip(spriteObj);
			break;
		case LabelChange.DISCARD:
			DiscardItem(spriteObj);
			break;
		case LabelChange.ON_TAKE:
			OnTakeItem(spriteObj);
			break;
		}
	}

	protected void DiscardItem(GameObject linkSprite)
	{
		GameObject realObject = linkSprite.GetComponent<SecondParent> ().virtualParent;
		if(Inventory.getInstance ().hasItem (realObject))
		{
			Inventory.getInstance ().removeItem (realObject);
			realObject.GetComponent<ItemCube>().itemSpriteObj.SetActive(false);
			Destroy (realObject.GetComponent<ItemCube>().itemSpriteObj);
			Destroy (realObject);
		}
		OnUpdateInventory ();
	}

	public void OnEquip(GameObject linkSprite)
	{
		WeaponBase inven_weapon = linkSprite.GetComponent<SecondParent> ().virtualParent.GetComponent<ItemCube> ().weapon;
		IPartItemBase inven_part = linkSprite.GetComponent<SecondParent> ().virtualParent.GetComponent<ItemCube> ().part;
		ICCharacterBase c = GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c;


		if(inven_weapon != null)
		{
			if(!c.canEquip(inven_weapon)) return;

			// If Trying UnEquip then,
			if(inven_weapon == GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.getCurrentWeapon())
			{
				Inventory.getInstance().insertItem(inven_weapon.gameObject);
				c.unequipWeapon();
			}
			// Or Equip Weapon to the current.
			else 
			{
				WeaponBase swapped = c.equipWeapon(inven_weapon);
				Inventory.getInstance().removeItem(inven_weapon.gameObject);
				if(swapped != null)
					Inventory.getInstance().insertItem(swapped.gameObject);
			}
		}
		else if (inven_part != null)
		{
			WeaponBase w = c.getCurrentWeapon();
			if(w != null)
			{
				int idx = w.installPart(inven_part);
				if(idx != WeaponBase.INSTALL_FALSE)
					Inventory.getInstance().removeItem(inven_part.gameObject);
			}
			else
				Debug.Log("If you wanna install that part, then equip your weapon first. :)");
		}
		OnUpdateWeaponInfo ();
		OnUpdateInventory();
	}

	public void ClearWeaponInfo()
	{
		// parts
		for(int i = 0; i < 3; i++)
			for(int j = 0; j < 3; j++)
				weaponSlotObjs[i,j].SetActive(false);

		WeaponBase prev = prev_weapon_toClear;
		if(prev == null) return;

		// relic update
		for(int i = 0; i < prev.relic_parts.max_size; i++)
			if(prev.relic_parts.list[i] != null)
			{
				GameObject itemSpriteObj = prev.relic_parts.list[i].GetComponent<ItemCube>().itemSpriteObj;
				itemSpriteObj.SetActive(false);
			}
		// magic update
		for(int i = 0; i < prev.magic_parts.max_size; i++)
			if(prev.magic_parts.list[i] != null)
			{
				GameObject itemSpriteObj = prev.magic_parts.list[i].GetComponent<ItemCube>().itemSpriteObj;
				itemSpriteObj.SetActive(false);
			}	
		// Mechanic update
		for(int i = 0; i < prev.mecha_parts.max_size; i++)
			if(prev.mecha_parts.list[i] != null)
			{
				GameObject itemSpriteObj = prev.mecha_parts.list[i].GetComponent<ItemCube>().itemSpriteObj;
				itemSpriteObj.SetActive(false);
			}
	}

	public void OnUpdateWeaponInfo()
	{
		WeaponBase w = GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.getCurrentWeapon ();
		ClearWeaponInfo ();

		if (w == null)
			return;

		prev_weapon_toClear = w;
		GameObject weaponSpriteObj = w.gameObject.GetComponent<ItemCube>().itemSpriteObj;
		weaponSpriteObj.transform.parent = this.gameObject.transform;
		weaponSpriteObj.SetActive (true);
		//weapon
		for(int i = 0; i < weaponSpriteObj.transform.childCount; i++)
		{
			weaponSpriteObj.transform.GetChild(i).position = equipSlotObj.transform.position;
			weaponSpriteObj.transform.GetChild(i).localScale = equipSlotObj.transform.localScale;
		}
		// clear WeaponInfo
		// parts
		for(int i = 0; i < 3; i++)
			for(int j = 0; j < 3; j++)
				weaponSlotObjs[i,j].SetActive(false);

		// Relic update
		for(int i = 0; i < w.relic_parts.max_size; i++)
		{
			if(w.relic_parts.list[i] != null)
			{
				GameObject itemSpriteObj = w.relic_parts.list[i].GetComponent<ItemCube>().itemSpriteObj;
				itemSpriteObj.transform.parent = this.gameObject.transform;
				itemSpriteObj.transform.position = weaponSlotObjs[RELIC_SLOT,i].transform.position;
				itemSpriteObj.transform.GetChild(0).transform.localScale = weaponSlotObjs[RELIC_SLOT,i].transform.localScale;
				itemSpriteObj.transform.GetChild(1).transform.localScale = weaponSlotObjs[RELIC_SLOT,i].transform.localScale;
				itemSpriteObj.SetActive(true);
			}
			else
				weaponSlotObjs[RELIC_SLOT,i].SetActive(true);

		}
		// Magic update
		for(int i = 0; i < w.magic_parts.max_size; i++)
		{
			if(w.magic_parts.list[i] != null)
			{
				GameObject itemSpriteObj = w.magic_parts.list[i].GetComponent<ItemCube>().itemSpriteObj;
				itemSpriteObj.transform.parent = this.gameObject.transform;
				itemSpriteObj.transform.position = weaponSlotObjs[MAGIC_SLOT,i].transform.position;
				itemSpriteObj.transform.GetChild(0).transform.localScale = weaponSlotObjs[MAGIC_SLOT,i].transform.localScale;
				itemSpriteObj.transform.GetChild(1).transform.localScale = weaponSlotObjs[MAGIC_SLOT,i].transform.localScale;
				itemSpriteObj.SetActive(true);
			}
			else
				weaponSlotObjs[MAGIC_SLOT,i].SetActive(true);
		}
		// Mechanic update
		for(int i = 0; i < w.mecha_parts.max_size; i++)
		{
			if(w.mecha_parts.list[i] != null)
			{
				GameObject itemSpriteObj = w.mecha_parts.list[i].GetComponent<ItemCube>().itemSpriteObj;
				itemSpriteObj.transform.parent = this.gameObject.transform;
				itemSpriteObj.transform.position = weaponSlotObjs[MECHA_SLOT,i].transform.position;
				itemSpriteObj.transform.GetChild(0).transform.localScale = weaponSlotObjs[MECHA_SLOT,i].transform.localScale;
				itemSpriteObj.transform.GetChild(1).transform.localScale = weaponSlotObjs[MECHA_SLOT,i].transform.localScale;
				itemSpriteObj.SetActive(true);
			}
			else
				weaponSlotObjs[MECHA_SLOT,i].SetActive(true);
		}
	}

	public bool IsOpen() {return isOpenState;}
	public void OpenInventoryUI()
	{
		isOpenState = !isOpenState;
		windowUIObject.SetActive (isOpenState);
		if(isOpenState)
		{
			OnUpdateWeaponInfo ();
			OnUpdateInventory ();
		}
		else descriptorObj.SetActive(false);

		labelChangeObject.GetComponent<LabelChange> ().setMode(LabelChange.EQUIP);
	}

	protected void VerifyPickedItem(GameObject realParent, string itemName)
	{
		// 아이템을 보존하시겠습니까?
		verifyObj.SetActive (true);
		verifyObj.GetComponent<VerifyItem> ().itemCube = realParent;
		verifyObj.transform.FindChild ("verify_label").GetComponent<UILabel>().text ="<" + itemName + ">\n" + GET_ITEM;
	}

	protected void OnTakeItem(GameObject spriteObj)
	{
		WeaponBase inven_weapon = spriteObj.GetComponent<SecondParent> ().virtualParent.GetComponent<ItemCube> ().weapon;
		IPartItemBase inven_part = spriteObj.GetComponent<SecondParent> ().virtualParent.GetComponent<ItemCube> ().part;

		if(inven_weapon != null)
		{
			VerifyPickedItem(spriteObj.GetComponent<SecondParent> ().virtualParent, inven_weapon.Name);
		}
		else if (inven_part != null)
		{
			VerifyPickedItem(spriteObj.GetComponent<SecondParent> ().virtualParent, inven_part.Name);
		}
	}

	public void OnUpdateInventory()
	{
		// after user onclick execution, update inven.
		GameObject[,] items = Inventory.getInstance().items;

		for(int i = 0; i < Inventory.MAX_HEIGHT; i++)
		{
			for(int j = 0; j < Inventory.MAX_WIDTH; j++)
			{
				if(items[i,j] != null)
				{
					GameObject itemSpriteObj = items[i,j].GetComponent<ItemCube>().itemSpriteObj;

					itemSpriteObj.SetActive(true);
					itemSpriteObj.transform.parent = this.gameObject.transform; // move to ngui layout.

					Vector3 pos = slotObjs[i,j].transform.position;
					Vector3 scale = slotObjs[i,j].transform.localScale;
					Quaternion rot = slotObjs[i,j].transform.rotation;
					itemSpriteObj.transform.position = pos;
					itemSpriteObj.transform.localScale = Vector3.one;
					for(int k = 0; k < itemSpriteObj.transform.childCount; k++)
					{
						itemSpriteObj.transform.GetChild(k).transform.localPosition = Vector3.zero;
						itemSpriteObj.transform.GetChild(k).transform.rotation = rot;
						itemSpriteObj.transform.GetChild(k).transform.localScale = scale;

						// Twice call for parent activation wait.
						itemSpriteObj.transform.GetChild(k).gameObject.layer = LayerMask.NameToLayer("UI");
						itemSpriteObj.transform.GetChild(k).gameObject.SetActive(true);
						itemSpriteObj.transform.GetChild(k).gameObject.SetActive(true);
					}
				}
			}
		}
	}
}
