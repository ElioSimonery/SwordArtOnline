using UnityEngine;
using System.Collections;

public class VerifyItem : MonoBehaviour {

	public GameObject itemCube = null;

	void Start()
	{
		gameObject.SetActive(false);
	}

	public void OnOK()
	{
		if(itemCube != null)
		{
			// save this item to local inventory
			itemCube.GetComponent<ItemCube>().recovery();
			if(GameManager.ioManager.saveItem(itemCube))
				SpeechController.GetInstance().pushPrint("개발자", "처음으로 돌아갑니다.");
			else
				SpeechController.GetInstance().pushPrint("개발자", "창고가 꽉 차 있어 저장하지 못했습니다.\n처음으로 돌아갑니다.");
			//GameManager.ioManager.loadIntroScene();
		}
	}

	public void OnCancel()
	{
		gameObject.SetActive (false);
	}
}