using UnityEngine;
using System.Collections;

public class LabelChange : MonoBehaviour {
	// make it enum
	public const int EQUIP = 0;
	public const int DISCARD = 1;
	public const int ON_TAKE = 2;

	public string[] stringsToChange;
	public GameObject labelObject;
	public int mode;
	public static LabelChange instance;

	public static LabelChange getInstance()
	{
		if (instance == null)
		{

			Debug.LogWarning ("LabelChange hasn't been created. force to open inventory.");
			/*
			InventoryScript inven = GameManager.inventoryObject.GetComponent<InventoryScript> ();
			if(!inven.IsOpen())
			{
				inven.OpenInventoryUI();
			}*/
		}
		return instance;
	}
	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setMode(int _mode)
	{
		mode = _mode;
		labelObject.GetComponent<UILabel> ().text = stringsToChange [mode%stringsToChange.Length];
	}
	void OnChangeLabel()
	{
		do 
		{
			mode++;
			mode = mode % stringsToChange.Length;
			setMode (mode);
		} while(mode == ON_TAKE);
	}
}
