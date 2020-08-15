using UnityEngine;
using System.Collections;

public class UISettingsController : MonoBehaviour {
	protected GameObject controllerObj;
	protected GameObject shooterObj;
	protected GameObject anchor_stateObj;
	protected GameObject anchor_hudObj;
	protected GameObject anchor_inventoryObj;
	protected GameObject anchor_descriptorObj;
	protected GameObject anchor_settingsObj;
	protected GameObject anchor_speechObj;
	public int zoomCount = 0;

	public int getZoomCount() { return zoomCount; }
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnSizeUp()
	{
		Debug.Log ("up"+zoomCount);
		size (1.1f);
		zoomCount++;
	}

	public void OnSizeDown()
	{
		Debug.Log ("down"+zoomCount);
		size (0.9f);
		zoomCount--;
	}

	void size(float zoom)
	{
		controllerObj = GameObject.Find ("Controller");
		shooterObj = GameObject.Find ("Shooter");
		anchor_stateObj = GameObject.Find ("_anchor_state_hud10");
		anchor_settingsObj = GameObject.Find ("_anchor_settings55");
		anchor_inventoryObj = GameObject.Find ("_anchor_inventory40");

		anchor_descriptorObj = GameObject.Find ("panel_anchor_descriptor50");
		anchor_hudObj = GameObject.Find ("panel_anchor_hud20");
		anchor_speechObj = GameObject.Find ("panel_anchor_speech70");

		// 컨트롤러, 슈터는 스프라이트 객체들 스케일을 모두 업
		UISprite[] c_sprites = controllerObj.GetComponentsInChildren<UISprite> ();
		foreach(UISprite s in c_sprites)
			s.gameObject.transform.localScale *= zoom;

		UISprite[] s_sprites = shooterObj.GetComponentsInChildren<UISprite> ();
		foreach(UISprite s in s_sprites)
			s.gameObject.transform.localScale *= zoom;

		// hp상태허드는 최상위 사이즈 스케일 업
		anchor_stateObj.transform.localScale *= zoom;
		// 허드 최상위 스케일 업
		anchor_hudObj.transform.localScale *= zoom;
		// 인벤토리 최상위 스케일 업
		anchor_inventoryObj.transform.localScale *= zoom;
		// 디스크립터 최상위 스케일 업
		anchor_descriptorObj.transform.localScale *= zoom;
		// 세팅화면도 스케일 변경
		anchor_settingsObj.transform.localScale *= zoom;

		anchor_speechObj.transform.localScale *= zoom;
	}
}
