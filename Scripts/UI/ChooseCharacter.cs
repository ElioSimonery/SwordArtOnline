using UnityEngine;
using System.Collections;

public class ChooseCharacter : MonoBehaviour {
	public GameObject[] characters;

	void Update()
	{
		if (Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
				if(hit.transform.gameObject.tag == Common.TAG_PLAYER)
				{
					foreach(GameObject c in characters)
					{
						Light light = c.GetComponentInChildren<Light>();
						light.enabled = false;
					}
					GameObject character = hit.transform.gameObject;
					Light choosed = character.GetComponentInChildren<Light>();
					choosed.enabled = true;
					Debug.Log("[ChooseCharacter] Choosed " + character.name);
					IOManager.GetInstance().forcePushData(Common.TEMPORARY_LOADCHARACTER, 
				                               hit.transform.gameObject.name);
				}
		}
	}

	// these functions are called by ngui button message.
	void loadNextScene()
	{
		if (string.IsNullOrEmpty (IOManager.GetInstance ().getData (Common.TEMPORARY_LOADCHARACTER)))
		{
			Debug.Log("Choose your character.");
			return;
		}
		string sceneName = IOManager.GetInstance ().getData (Common.TEMPORARY_LOADSCENE);
		IOManager.GetInstance ().LoadSceneAsync (sceneName);
	}

	void backToIntro()
	{
		IOManager.GetInstance ().removeData (Common.TEMPORARY_LOADCHARACTER);
		IOManager.GetInstance ().removeData (Common.TEMPORARY_LOADSCENE);
		IOManager.GetInstance ().LoadSceneAsync (Common.SCENE_INTRO);
	}
}
