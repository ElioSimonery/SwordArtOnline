using UnityEngine;
using System.Collections;

public class MainHud : MonoBehaviour {
	//public Camera nguiCamera;
	//public UIPanel panelContainer;
	//public GameObject anchor_hud;

	public UIWidget hud_handler;
	//public UIWidget background;

	public bool isOpenState;
	//private float hud_height;
	// Use this for initialization
	void Start () {
		isOpenState = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnHandleUI()
	{
		isOpenState = isOpenState ? false:true;
		Vector3 v = hud_handler.gameObject.transform.localPosition;
		if (isOpenState) // then close this time.
			v.y = 100f;
		else
			v.y = 0f;
		hud_handler.gameObject.transform.localPosition = v;


		Vector3 v2 = gameObject.transform.localPosition;
		if (isOpenState) // then close this time.
			v2.y = 0f;
		else
			v2.y = -100f;
		gameObject.transform.localPosition = v2;

	}
}
