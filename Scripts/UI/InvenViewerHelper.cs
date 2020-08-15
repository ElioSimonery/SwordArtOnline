using UnityEngine;
using System.Collections;

public class InvenViewerHelper : MonoBehaviour {
	public bool isInventory;
	public GameObject UIObjectToControl;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OpenHelper()
	{
		if(isInventory)
			UIObjectToControl.GetComponent<InventoryScript> ().OpenInventoryUI ();
		else
		{
			//skill ui open/close
		}
	}
}
