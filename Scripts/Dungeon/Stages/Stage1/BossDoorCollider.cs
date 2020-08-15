using UnityEngine;
using System.Collections;

// <<Script>>
public class BossDoorCollider : MonoBehaviour {
	public GameObject doorObj;

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == Common.TAG_PLAYER) 
		{
			NotificationManager.GetInstance().toast("[출입구 봉쇄 됨]");
			doorObj.GetComponent<Animator>().SetBool ("Open",true);
			gameObject.SetActive(false);
		}
	}


}
