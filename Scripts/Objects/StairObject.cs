using UnityEngine;
using System.Collections;

public class StairObject : MonoBehaviour {
	public string strToLoadNextScene;
	public bool checkTriggerOnStay = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == Common.TAG_PLAYER) 
		{
			// Go to next scene
			if(!GameManager.missionCleared) return;
			GameManager.ReloadCurrentScene(strToLoadNextScene);
			GameManager.ioManager.currentStageNumber++;
			GameManager.ioManager.SaveStorySceneName(strToLoadNextScene);
		}
	}

	void OnTriggerStay(Collider col)
	{
		if(checkTriggerOnStay)
			OnTriggerEnter(col);
	}

}
