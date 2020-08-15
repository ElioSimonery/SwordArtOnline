using UnityEngine;
using System.Collections;

public class ItemCube : MonoBehaviour {
	protected GameObject userInventoryObj;
	public GameObject itemSpriteObj;
	// cube item
	public WeaponBase weapon; // containing weapon
	public IPartItemBase part; // containing part

	protected bool isStashItem = false;
	protected bool isPickedStashItem = false;

	void Awake()
	{
		DontDestroyOnLoad (gameObject);
	}
	// Use this for initialization
	void Start () {

	}

	public void setStashItem(bool value)
	{
		isStashItem = value;
	}

	public bool isStash()
	{
		return isStashItem;
	}

	public void setStashPickItem(bool value)
	{
		isPickedStashItem = value;
	}

	public bool isPicked()
	{
		return isPickedStashItem;
	}

	public void setWeapon(WeaponBase w)
	{
		weapon = w;
	}

	public void setPart(IPartItemBase p)
	{
		part = p;
	}
	public void setRGBA(float r, float g, float b, float a)
	{
		Color c = new Color(r,g,b,a);
		GetComponent<Renderer>().material.color = c;
	}

	public void setAlpha(float a)
	{
		Color c = new Color(GetComponent<Renderer>().material.color.r, 
	                        GetComponent<Renderer>().material.color.g, 
	                        GetComponent<Renderer>().material.color.b, 
	                        a); 
		GetComponent<Renderer>().material.color = c;
		GetComponent<Light> ().color = c;
	}

	public void setItemSprite(Sprite s)
	{
		gameObject.GetComponentInChildren<SpriteRenderer> ().sprite = s;
	}

	public void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == Common.TAG_PLAYER)
		{
			if(Inventory.getInstance().insertItem(gameObject))
			{
				Debug.Log("[ItemCube] Character took the item : " + gameObject.name);
				GameManager.garbage.pop(gameObject);
				GameManager.inventoryObject.GetComponent<InventoryScript>().OnUpdateInventory();
				gameObject.SetActive(false);
			}
			else
			{
				Debug.Log("[ItemCube] your inventory is full.");
			}
		}
	}

	public virtual void onInitItem(GameObject inventoryObj)
	{
		if(weapon != null)
		{
			weapon.init ();
		}
		userInventoryObj = inventoryObj;
		itemSpriteObj.transform.GetChild(0).GetComponent<InvenSlotController>().init (inventoryObj);
	}

	public void recovery()
	{
		itemSpriteObj.transform.parent = transform;
	}
}
