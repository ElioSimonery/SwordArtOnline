using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Don't destroy this script in the game.
public class StashDAO : InventoryScript {
	public int stashWidthSize;
	public int stashHeightSize;
	public int maxPickCount = 3;
	public int pickCount = 0;

	private static StashDAO instance;
	// Data

	public static StashDAO GetInstance()
	{
		if(instance == null)
			Debug.LogWarning("Stash hasn't been created! GetInstance is null.");
		return instance;
	}

	protected override void Awake()
	{
		if(instance == null)
			instance = this;
		else
			Debug.LogWarning("Stash has been created twice.");

		// stash init
		stashHeightSize = Stash.GetInstance ().stashHeightSize;
		stashWidthSize = Stash.GetInstance ().stashWidthSize;
	}

	void Start()
	{
		OnInitStashScene();
	}

	// Interface
	public void OnInitStashScene()
	{
		// Only stash scene
		slotObjs = new GameObject[stashHeightSize, stashWidthSize];
		//init ui
		initializeSlots (stashHeightSize, stashWidthSize);
		// init weapon part slots
		initializeWeaponInfo ();
		OnUpdateStash ();
	}

	public void OnUpdateStash()
	{
		for(int i = 0; i < stashHeightSize; i++) for(int j = 0; j < stashWidthSize; j++)
		if(Stash.GetInstance().stashItems[i,j] != null)
		{
			GameObject itemSpriteObj = Stash.GetInstance().
				stashItems[i,j].GetComponent<ItemCube>().itemSpriteObj;
			
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


	public List<GameObject> getCheckedStashItems()
	{
		List<GameObject> ret = new List<GameObject> ();
		foreach(GameObject item in Stash.GetInstance().stashItems)
		{
			if(item.GetComponent<ItemCube>().isPicked())
			{
				ret.Add (item);
			}
		}

		return ret;
	}


	// DB Access 
	public void LoadFromDB()
	{
		// Connect to DB
		// Load items from DB for loop.
	}

	public void SaveToDB()
	{
		
	}
}
