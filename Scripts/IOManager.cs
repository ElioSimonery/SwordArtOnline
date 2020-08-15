using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IOManager : MonoBehaviour {
	#region MEMBER VARIABLES
	public int currentStageNumber;
	public string loadNextSceneName;
	public string[] characterPrefabNames;
	public GameObject[] characterPrefabs;

	// Save&Load scene data
	private GameObject[] weapons;
	private int current_weapon;
	private Dictionary<string, string> bundleData;
	private Dictionary<string, GameObject> characterObj;

	private static IOManager mInstance;
	#endregion

	#region INITIALIZERS
	void Awake()
	{
		//PlayerPrefs.DeleteAll (); FOR DEBUG
		if (mInstance != null)
			Debug.LogWarning ("[IOManager] has already created.");
		mInstance = this;

		DontDestroyOnLoad (gameObject);
		Debug.Log ("[IOManager] Created from Awake()");
		bundleData = new Dictionary<string, string> ();
		characterObj = new Dictionary<string, GameObject> ();
		for(int i = 0; i < characterPrefabNames.Length; i++)
			characterObj[characterPrefabNames[i]] = characterPrefabs[i];
	}

	public static IOManager GetInstance() { return mInstance; }

	IEnumerator Start()
	{
		AsyncOperation async = Application.LoadLevelAsync (loadNextSceneName);
		yield return async;
	}
	#endregion

	#region BUNDLE_DATA ACCESSOR
	public string getData(string key, bool remove = false)
	{
		string ret = bundleData [key];
		if (remove)
			bundleData.Remove (key);
		return ret;
	}

	public bool pushData(string key, string value)
	{
		if (bundleData.ContainsKey (key))
			return false;
		else
		{
			bundleData[key] = value;
			return true;
		}
	}

	public bool removeData(string key)
	{
		if (bundleData.ContainsKey (key))
		{
			return bundleData.Remove (key);
		}
		else
		{
			Debug.Log("removeData() bundleData doesn't have the key :" + key);
			return false;
		}
	}

	public void forcePushData(string key, string value)
	{
		if (bundleData.ContainsKey (key))
			bundleData.Remove (key);
		bundleData[key] = value;
	}
	#endregion

	#region SCENE_ACCESSOR
	public void LoadSceneAsync(string sceneName)
	{
		Debug.Log ("[IOManager] Async loading scene -> " + sceneName);
		GameUIPanelManager.GetInstance ().loading (true);
		Application.LoadLevelAsync (sceneName);
	}
	#endregion

	#region CHANGE_SCENE_MANAGER
	public bool saveItem(GameObject itemCube)
	{
		if (Stash.GetInstance ().AddItem (itemCube))
			GameManager.garbage.pop (itemCube);
		else
			return false;
		return true;
	}

	public void PrepareEquipped(GameObject characterObj)
	{
		// this code will be changed.
		WeaponBase[] _wbs = characterObj.GetComponent<CharacterControlHelper> ().c.getWeapons ();
		int _szWeapon = characterObj.GetComponent<CharacterControlHelper> ().c.viewWeaponCapability ();
		weapons = new GameObject[_szWeapon];
		for(int i = 0; i < _szWeapon; i++)
			if(_wbs[i] != null)
			{
				weapons[i] = _wbs[i].gameObject;
				weapons[i].GetComponent<ItemCube>().recovery();
				_wbs[i].recoveryParts();
				
			}
		// weapons & parts recovery.
		current_weapon = characterObj.GetComponent<CharacterControlHelper> ().c.getCurrentWeaponIdx ();
	}

	public void LoadEquipped(GameObject characterObj)
	{
		Debug.Log("[IOManager] Processing LoadEquipped()");
		if(weapons != null)
		{
			int _szWeapon = characterObj.GetComponent<CharacterControlHelper> ().c.viewWeaponCapability();
			WeaponBase[] _wbs = new WeaponBase[_szWeapon];

			for(int i = 0; i < weapons.Length; i++)
			{
				if(weapons[i] != null)
				{
					_wbs[i] = weapons[i].GetComponent<ItemCube>().weapon;
					Debug.Log("[IOManager] Equipped : " + _wbs[i].Name);
				}
			}

			foreach(WeaponBase w in _wbs)
				if(w)
				{
					w.GetComponent<ItemCube>().onInitItem(GameManager.inventoryObject);
					w.onInitParts(GameManager.inventoryObject);
				}

			characterObj.GetComponent<CharacterControlHelper> ().c.onInitSetWeapons (ref _wbs, current_weapon);
			weapons = null;
		}
		else
			Debug.Log("[IOManager] prev weapons didn't saved. Couldn't load equipped item. ");
		Debug.Log("[IOManager] Finished LoadEquipped()");
	}

	public GameObject LoadCharacter()
	{
		GameObject character = (GameObject)Instantiate(Resources.Load ("loadCharacter/" + getData(Common.TEMPORARY_LOADCHARACTER)));
		Debug.Log ("[IOManager] Loaded character : "+character.name);
		character.SetActive (true);
		// if DummyCharacter -> return.
		if (GameManager.controllerObj != null)
			GameManager.controllerObj.GetComponent<BFloatingController> ().TargetCharacter = character;
		if (GameManager.shooterObj != null)
			GameManager.shooterObj.GetComponent<BFloatingController> ().TargetCharacter = character;
		if (GameManager.World.GetComponent<DungeonModelHelper> () != null)
			GameManager.World.GetComponent<DungeonModelHelper> ().d.characterBase = character;
		if (GameManager.MainCameraObj != null)
			GameManager.MainCameraObj.GetComponent<VMainCamera> ().setTrackingObject(ref character);
		return character;
	}

	public void PrepareInventory()
	{
		foreach (GameObject item in Inventory.getInstance().items)
			if(item != null)
			{
				item.GetComponent<ItemCube>().recovery ();
				if(item.GetComponent<ItemCube>().weapon != null)
					item.GetComponent<ItemCube>().weapon.recoveryParts();
			}	
	}
	
	public void LoadInventory()
	{
		Inventory inven = Inventory.getInstance();
		
		for(int i = 0; i < Inventory.MAX_HEIGHT; i++)
			for(int j = 0; j < Inventory.MAX_WIDTH; j++)
				if(inven.items[i,j] != null)
				{
					// Rematch the missing scripts. 
					inven.items[i,j].GetComponent<ItemCube>().onInitItem( 
						GameManager.inventoryObject);
					
					if(inven.items[i,j].GetComponent<ItemCube>().weapon != null)
					{
						inven.items[i,j].GetComponent<ItemCube>().weapon.init();
						inven.items[i,j].GetComponent<ItemCube>().weapon.onInitParts(GameManager.inventoryObject);
					}
				}
	}

	public void PrepareSettings(UISettingsController ui)
	{
		PlayerPrefs.SetFloat(Common.DATA_SETTINGS_ZOOM, (float)ui.zoomCount);
		Debug.Log ("[IOManager] saved zoom rate : " + (float)ui.zoomCount);
	}

	public void LoadSettings(UISettingsController ui)
	{
		int zoomCount = (int)PlayerPrefs.GetFloat (Common.DATA_SETTINGS_ZOOM);
		Debug.Log ("[IOManager] loaded zoom rate : " + zoomCount);
		for(int i = 0; i < zoomCount; i++)
			ui.OnSizeUp ();
		for(int i = 0; i > zoomCount; i--)
			ui.OnSizeDown ();
		ui.zoomCount = zoomCount;
		zoomCount = 0;
	}
	#endregion
	
	#region GAMEDATA_IO
	public void SaveStorySceneName(string sceneName = null)
	{
		if(string.IsNullOrEmpty(sceneName))
			sceneName = Application.loadedLevelName;
		PlayerStageIO.GetInstance ().save (sceneName);
		Debug.Log ("[IOManager] saved story scene name : " + sceneName);
	}
	
	public string GetLoadStorySceneName()
	{
		return PlayerPrefs.GetString (Common.DATA_STORY_SCENE);
	}


	public bool IsInfinityModeOpened()
	{
		if (string.IsNullOrEmpty (PlayerPrefs.GetString (Common.DATA_INFINITY_OPENED)))
			return false;
		else
			return true;
	}

	public void OpenInfinityMode()
	{
		PlayerPrefs.SetString (Common.DATA_INFINITY_OPENED, "open");
	}
	#endregion
}
