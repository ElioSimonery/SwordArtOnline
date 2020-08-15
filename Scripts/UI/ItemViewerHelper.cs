using UnityEngine;
using System.Collections;

public class ItemViewerHelper : MonoBehaviour {
	public GameObject item_info_viewerObj;

	void Awake()
	{
		DontDestroyOnLoad (transform.gameObject);
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ViewOn(GameObject spriteObj)
	{
		GameManager.descriptor.displayItem (gameObject);
		GameManager.descriptorObject.SetActive (true);
	}

	void ViewOff()
	{
		GameManager.descriptorObject.SetActive (false);
	}
}
