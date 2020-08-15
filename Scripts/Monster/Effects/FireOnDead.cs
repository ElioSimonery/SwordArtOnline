using UnityEngine;

public class FireOnDead : MonoBehaviour
{
	public float effectTime;
	public int fireDamage = 2;
	public float dotTime = 0.5f;

	public static GameObject _thisObj;
	protected float ttlTimer;
	protected float fireTimer;
	protected bool startFire;

	void Awake()
	{
		if (startFire) return;

		if (_thisObj != null)
			Debug.LogWarning ("[FireOnDead] has already initialized !");
		_thisObj = gameObject;
		gameObject.SetActive (false);
	}

	void Update()
	{
		ttlTimer += Time.deltaTime;

		if(ttlTimer >= effectTime)
		{
			Destroy(gameObject);
		}
	}


	void OnTriggerStay(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
		{
			fireTimer += Time.deltaTime;

			if(fireTimer >= dotTime)
			{
				col.gameObject.GetComponent<CharacterControlHelper>().c.hit(fireDamage);
				fireTimer = 0f;
			}
		}
	}

	void OnTriggerExit(Collider col)
	{

	}

	public static void OnFire(GameObject posObject, bool applyHPDamage = false)
	{
		GameObject newFireObj = GameManager.CopyObjects (_thisObj);
		newFireObj.transform.position = posObject.transform.position;
		newFireObj.GetComponent<FireOnDead> ().startFire = true;
		if (applyHPDamage)
			newFireObj.GetComponent<FireOnDead> ().applyMaxHPRateDamage ();

		newFireObj.SetActive (true);
	}

	public void applyMaxHPRateDamage(int rate = 5)
	{
		fireDamage = GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.viewMaxHP () * rate / 100;
	}
}
