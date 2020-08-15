using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public bool isMenuScene = true;
	public bool loadDummyCharacter = false;
	public static GameObject World;
	public static GameObject MainCameraObj;
	public static GameObject PlayerObject;

	public static GameObject PlayerHud;
	public static GameObject UIHud;

	public static Dictionary<string, GameManagerComponent> gm_components = null;

	// Character load data
	public static GameObject controllerObj;
	public static GameObject shooterObj;
	// public static GameObject 

	public static GameObject inventoryObject;
	public static GameObject descriptorObject;
	public static GameObject settingsObject;
	public static UISettingsController uiSettings;
	public static ItemInfoDescriptor descriptor;

	public static GameObject ioManagerObj;
	public static IOManager ioManager;
	public static GarbageManager garbage;
	public static bool missionCleared { get; set; }


	void Awake () {
		if (gm_components != null) Debug.LogWarning ("[GameManager] has been already created!");
		gm_components = new Dictionary<string, GameManagerComponent> ();

		World = GameObject.Find ("World"); // this object actually..
		MainCameraObj = GameObject.Find ("Main Camera");
		PlayerHud = GameObject.Find ("PlayerHud");
		UIHud = GameObject.Find ("panel_anchor_hud20");
		inventoryObject = GameObject.Find ("inventory");
		descriptorObject = GameObject.Find("item_descriptor");
		settingsObject = GameObject.Find("settings");
		ioManagerObj = GameObject.Find ("IOManager");
		controllerObj = GameObject.Find ("Controller");
		shooterObj = GameObject.Find ("Shooter");

		garbage = GetComponent<GarbageManager> ();

		if(descriptorObject != null)
			descriptor = descriptorObject.GetComponent<ItemInfoDescriptor> ();
		if(settingsObject != null)
			uiSettings = settingsObject.GetComponent<UISettingsController> ();

		if(ioManagerObj != null)
		{
			ioManager = ioManagerObj.GetComponent<IOManager>();
			if(loadDummyCharacter)
				PlayerObject = ioManager.LoadCharacter();
			if(!isMenuScene)
				PlayerObject = ioManager.LoadCharacter();
		}
		else
			Debug.LogWarning("IOManagerObject is null. can't save & load scene.");

		if(inventoryObject != null)
		{
			inventoryObject.GetComponent<InventoryScript> ().windowUIObject.SetActive (true); // call start
			inventoryObject.GetComponent<InventoryScript> ().windowUIObject.SetActive (false);
		}
	}

	void Start()
	{
		OnLoadScene ();
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void Update()
	{
		if(PlayerObject != null &&
		   PlayerObject.GetComponent<CharacterControlHelper>().c.isDead() && 
		   SpeechController.GetInstance().noSpeech)
		{
			ReloadCurrentScene(Application.loadedLevelName);
		}
	}

	public void RecoveryOnChangedScene()
	{
		Debug.Log ("Called GameManager scene recovery");
		descriptorObject = GameObject.Find("item_descriptor");
		if(descriptorObject != null)
			descriptor = descriptorObject.GetComponent<ItemInfoDescriptor> ();
	}

	public void OnLoadScene()
	{
		if (isMenuScene)
			return;
		if(ioManager != null)
		{
			ioManager.LoadSettings(uiSettings);
			settingsObject.SetActive(false);
			ioManager.LoadInventory ();
			ioManager.LoadEquipped(PlayerObject);
		}
	}

	public static bool CallGMComponent(string componentName)
	{
		if(gm_components.ContainsKey(componentName))
		{
			gm_components[componentName].callbackMethod();
			Debug.Log("[GameManager] called component " + componentName);
			return true;
		}
		return false;
	}
	public static bool AddGMComponent(GameManagerComponent newComponent)
	{
		if (gm_components.ContainsKey (newComponent.ComponentName))
		{
			Debug.LogWarning("[GameManager] has already the component : " + newComponent.ComponentName);
			return false;
		}

		gm_components.Add (newComponent.ComponentName, newComponent);
		return true;
	}

	public static GameManagerComponent GetGMComponent(string componentName)
	{
		if (!gm_components.ContainsKey (componentName))
		{
			Debug.LogWarning("[GameManager] Not exist such a component : " + componentName);
			return null;
		}
		
		return gm_components [componentName];
	}

	public static bool IsSafePosition(GameObject characterObj, GameObject monsterObj)
	{
		float wakeDistance = monsterObj.GetComponent<IMonsterBase>().getWakeDistance();
		Vector3 d = characterObj.transform.position - monsterObj.transform.position;
		
		return d.magnitude > wakeDistance*1.1f ? true : false;
	}

	public static GameObject CopyObjects(GameObject obj)
	{
		return (GameObject)Instantiate (obj);
	}

	public static void DestroyObjects(GameObject obj)
	{
		Destroy (obj);
	}

	// 케릭터 죽을 때 호출
	public static void OnDeadFinalizeScene()
	{
		MainCameraObj.GetComponent<GrayscaleEffect> ().enabled = true;

		// 기타 UI 모두 setActive false
		PlayerHud.SetActive (false);
		UIHud.SetActive (false);
		settingsObject.SetActive (false);
		controllerObj.SetActive (false);
		shooterObj.SetActive (false);

		inventoryObject.SetActive (true);
		InventoryScript inven = inventoryObject.GetComponent<InventoryScript> ();
		if(!inven.IsOpen()) inven.OpenInventoryUI ();

		SpeechController.GetInstance ().pushPrint ("개발자",
		                                           "죽었습니다.\n 진행중인 스테이지를 다시 시작합니다.",
		                                           (Resources.Load("portraits/DeveloperPortraits") as GameObject).GetComponent<UIAtlas>(),
		                                           "1");
	}

	public static void ReloadCurrentScene(string currentSceneName)
	{
		GameManager.ioManager.PrepareInventory();
		GameManager.ioManager.PrepareSettings(GameManager.uiSettings);
		GameManager.ioManager.PrepareEquipped(GameManager.PlayerObject);
		GameManager.garbage.makeCleanScene();
		Application.LoadLevel(currentSceneName);
	}

}