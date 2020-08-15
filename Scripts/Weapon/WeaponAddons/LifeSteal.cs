using UnityEngine;
using System.Collections;

public class LifeSteal : MonoBehaviour
{
	public float traceStartTime = 0.2f;
	public float traceVelocityTime = 1f;
	public float TTL = 2f;
	private float timer;
	
	void Start () 
	{
		Vector3 dir = transform.position - GameManager.PlayerObject.transform.position;

		Vector3 v = new Vector3 (Random.Range (-1f, 1f), Random.Range (0, 1f), Random.Range (-1f, 1f));
		Vector3 ortho = v - Vector3.Dot (v, dir) / Vector3.Dot (dir, dir) * dir;
		ortho = ortho.normalized * dir.magnitude;
		gameObject.GetComponent<Rigidbody>().velocity = (dir+ortho).normalized * 5f;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		timer += Time.fixedDeltaTime;

		Vector3 dir = Vector3.zero;
		if(GameManager.PlayerObject)
			dir = GameManager.PlayerObject.transform.position - transform.position;

		if(dir.sqrMagnitude < 2f)
		{
			Destroy(gameObject);
			return;
		}

		if(timer >= TTL)
		{
			Destroy(gameObject);
			return;
		}

		if(timer >= traceVelocityTime)
		{
			GetComponent<Rigidbody>().velocity = dir * GetComponent<Rigidbody>().velocity.magnitude;
			return;
		}

		if(timer >= traceStartTime)
		{
			GetComponent<Rigidbody>().AddForce(dir*(2f+timer));
		}
		
	}
}


