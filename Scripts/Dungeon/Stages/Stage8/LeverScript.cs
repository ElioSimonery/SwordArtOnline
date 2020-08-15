using UnityEngine;


public class LeverScript : MonoBehaviour
{
	public int leverNumber;
	public GameObject prev_leverObject;
	public bool isLeverTriggered = false;

	private void OnTriggerEnter(Collider col)
	{
		if (isLeverTriggered)
			return;

		if (col.tag == Common.TAG_MISSILE) 
		{
			OnPushLever();
		}
	}

	private bool OnPushLever()
	{
		if(prev_leverObject == null) // Initial lever
		{
			leverOn();
			return true;
		}
		else
		{
			if(!prev_leverObject.GetComponent<LeverScript>().isLeverTriggered)
			{
				NotificationManager.GetInstance().toast(leverNumber+"번 레버가 당겨지지 않습니다.");
				return false;
			}
			else
			{
				leverOn();
				return true;
			}
		}
	}

	void leverOn()
	{
		NotificationManager.GetInstance().toast(leverNumber + "번 레버가 작동했습니다 !");
		isLeverTriggered = true;
	}

}


