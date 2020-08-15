using UnityEngine;
using System.Collections;

public class GameUIPanelManager : MonoBehaviour {

	public GameObject mGameUIPanelParent;
	//public GameObject mSpeechPanel;
	public GameObject mHudPanel;
	public GameObject mDescriptorPanel;
	public GameObject mBackgroundAnimSprite;

	private static GameUIPanelManager mInstance;
	private static GameObject mLoadingObject;
	private static bool activated = false;
	void Awake()
	{
		if(mInstance == null)
		{
			mInstance = this;
			Debug.Log ("[GameUIPanelManager] has created");
		}
		else
			Debug.LogWarning("[GameUIPanelManager] has created twice.");
	}

	void Start()
	{

	}

	public void loading(bool value)
	{
		if (mLoadingObject != null)
			Destroy (mLoadingObject);
		if (!value) return;

		GameObject loading = Resources.Load ("ui/Loading") as GameObject;
		GameObject loadingObj = Instantiate (loading) as GameObject;
		loadingObj.layer = LayerMask.NameToLayer ("UI");
		loadingObj.transform.parent = GameObject.Find("Panel").transform;
		loadingObj.transform.localPosition = Vector3.zero;
		loadingObj.transform.localScale = Vector3.one;
		mLoadingObject = loadingObj;
	}

	public static GameUIPanelManager GetInstance()
	{
		if (mInstance == null) 
			Debug.LogWarning("[GameUIPanelManager] hasn't been created.");
		return mInstance;
	}

	public void setUIAnimBackground(UIAtlas storyAtlas, string spriteName, bool value)
	{
		mBackgroundAnimSprite.GetComponent<UISprite> ().atlas = storyAtlas;
		mBackgroundAnimSprite.GetComponent<UISprite> ().spriteName = spriteName;

		animActivation (value);
	}


	public void setUIAnimBackground(string spriteName, bool value)
	{
		mBackgroundAnimSprite.GetComponent<UISprite> ().spriteName = spriteName;
		animActivation (value);
	}

	public void animActivation(bool value)
	{
		GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.setLock (value);
		mBackgroundAnimSprite.SetActive (value);

		mGameUIPanelParent.SetActive (!value);
		mHudPanel.SetActive(!value);
		mDescriptorPanel.SetActive (!value);
		activated = value;
	}

	public bool isAnimActivated()
	{
		return activated;
	}
}
