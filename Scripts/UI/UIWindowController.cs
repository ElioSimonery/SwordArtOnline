using UnityEngine;
using System.Collections;

public class UIWindowController : MonoBehaviour {

	public bool sendBoolValue;
	public GameObject targetWindow;

	public void switchSendBool()
	{
		if (sendBoolValue)
			sendBoolValue = false;
		else
			sendBoolValue = true;
	}

	public void setActive()
	{
		targetWindow.SetActive (sendBoolValue);
	}
}
