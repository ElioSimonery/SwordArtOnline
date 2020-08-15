using UnityEngine;
using System.Collections;

public class NotificationManager : MonoBehaviour {
	public readonly string[] NO_WEAPON =
	{
		"무기를 착용해 주세요~", 
		"Please equip a weapon."
	};
	public float exposureTime;

	private UILabel alarm;
	private float timer;


	private static NotificationManager instance;
	public static NotificationManager GetInstance()
	{
		if(instance == null)
			Debug.LogWarning("Notification instance is null. create first.");

		return instance;
	}
	// Use this for initialization
	void Awake () {
		alarm = GetComponent<UILabel> ();
		if(instance != null)
			Debug.LogWarning("Notification has instantiated several times.");

		instance = GetComponent<NotificationManager>();
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		alarm.alpha = 1.5f - (timer/exposureTime);
		if(timer >= exposureTime)
		{
			gameObject.SetActive(false);

			timer = 0;
		}
	}

	public bool isActive()
	{
		return gameObject.activeInHierarchy;
	}

	public void toast(string text)
	{
		gameObject.SetActive (true);
		alarm.text = text;
		timer = 0;
	}

	public bool isToasting()
	{
		return gameObject.activeSelf;
	}
}
