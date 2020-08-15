using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntroUIManager : MonoBehaviour {
	public GameObject infinityButtonObj;
	public UILabel bottomLabel;
	public string introSceneName;
	public string infinitySceneName;
	public string storySceneName;
	public string chooseCharacterSceneName;
	public string stashSceneName;

	// Use this for initialization
	void Start () 
	{
		string nextStory = IOManager.GetInstance().GetLoadStorySceneName ();
		Debug.Log ("[IntroUIManager] Saved Prev Storyscene has appeared: " + nextStory);
		if (string.IsNullOrEmpty (nextStory))
			IOManager.GetInstance().SaveStorySceneName (storySceneName);
		bottomLabel.text = IOManager.GetInstance().GetLoadStorySceneName();
		
		if(IOManager.GetInstance().IsInfinityModeOpened())
			infinityButtonObj.SetActive (true);
		else
			infinityButtonObj.SetActive (false);
	}

	// these functions are called by ngui button message.
	void StartInfinityScene()
	{
		goToChooseCharacterScene (infinitySceneName);
	}

	void StartStoryScene()
	{
		goToChooseCharacterScene (IOManager.GetInstance ().GetLoadStorySceneName ());
	}

	void OnTouchedStoryLabel()
	{
		List<string> availStages = PlayerStageIO.GetInstance ().load ();

		string currentStage = bottomLabel.text;
		for(int i = 0; i < availStages.Count; i++)
		{
			if(!string.IsNullOrEmpty(availStages[i]))
			{
				if(currentStage == availStages[i])
				{
					bottomLabel.text = availStages[(i+1)%availStages.Count];
					return;
				}
			}
		}
	}

	protected void goToChooseCharacterScene(string nextScene)
	{
		IOManager.GetInstance ().forcePushData (Common.TEMPORARY_LOADSCENE, nextScene);
		IOManager.GetInstance ().LoadSceneAsync (chooseCharacterSceneName);
	}
}
