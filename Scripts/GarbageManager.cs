using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GarbageManager : MonoBehaviour {
	private Dictionary<GameObject, GameObject> items;
	// Use this for initialization
	void Awake () {
		// Dontdestroy
		items = new Dictionary<GameObject, GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void push(GameObject garbage)
	{
		items.Add (garbage, garbage);
	}

	public void pop(GameObject item)
	{
		if (!items.Remove (item))
			Debug.LogWarning("[GarbageManager] "+item.name+" doesn't contain garbage.");
	}

	public void makeCleanScene()
	{
		foreach(KeyValuePair<GameObject, GameObject> entry in items)
		{
			if(entry.Value != null)
				Destroy(entry.Value);
		}
	}
}
