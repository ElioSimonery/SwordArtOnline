using UnityEngine;
using System.Collections;

public class ControllerSettings : MonoBehaviour {
	public GameObject ControllerObj;
	public GameObject ControllerValueLabelObj;

	public string CheckBoxTrueString = "O";
	public string CheckBoxFalseString = "X";
	public bool mValue;

	protected string text;
	// Use this for initialization
	void Start () {
		if (mValue)
			text = CheckBoxTrueString;
		else
			text = CheckBoxFalseString;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void switchLabel()
	{
		if(mValue)
		{
			text = CheckBoxFalseString;
			mValue = false;
		}
		else
		{
			text = CheckBoxTrueString;
			mValue = true;
		}
		ControllerValueLabelObj.GetComponent<UILabel> ().text = text;
	}
	void foo()
	{
	}

	void OnSwtichTouched()
	{
		switchLabel ();
		ControllerObj.GetComponent<VFloatingController> ().isStatic = mValue;
		Debug.Log (mValue);
	}
}
