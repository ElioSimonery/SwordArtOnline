using UnityEngine;
using System.Collections;

public class WakeUp : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay(Collider col)
	{
		if(col.tag == Common.TAG_MONSTER)
		{
			if(col.gameObject.GetComponent<Monster3DBase>())
				col.gameObject.GetComponent<Monster3DBase>().forceToWakeUp();
		}
	}
}
