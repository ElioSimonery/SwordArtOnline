using UnityEngine;
using System.Collections;

public class GuideBulletTracker: MonoBehaviour {
	public float operationTime;
	private float operationTimer;

	public int max_count;
	private int count;
	private bool operation = false;
	private GameObject trackingTarget;
	private bool foundTarget = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.zero;
		operationTimer += Time.deltaTime;

		if(operationTimer > operationTime)
		{
			if(foundTarget)
			{
				operationTimer = 0f;
				foundTarget = false;
			}
			operation = true;
		}
		else
			operation = false;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == Common.TAG_MONSTER) 
		{
			if(!operation) return;
			if(count > max_count) return;
			foundTarget = true;

			if(trackingTarget == null)
				trackingTarget = col.gameObject;
			if(trackingTarget != col.gameObject) // I'm still tracking another monster.
				return;
			Vector3 toMonster = trackingTarget.transform.position - transform.parent.transform.position;
			Vector3 bulletVelocity = toMonster.normalized * transform.parent.transform.GetComponent<Rigidbody>().velocity.magnitude;
			if(bulletVelocity.magnitude < 3f)
			{
				bulletVelocity = 3*toMonster.normalized; // min velocity value is 1 for sword weapon.
			}
			transform.parent.transform.GetComponent<Rigidbody>().velocity = bulletVelocity;
			count++;
		}
		
	}

	void OnTriggerExit(Collider col) 
	{
		// When exit, it doesn't track initial monster anymore. because the fastest way for detecting&colliding is much better.
		if (col.gameObject.tag == Common.TAG_MONSTER) 
		{
			if(!operation) return;
			if(count > max_count) return;

			foundTarget = true;
			Vector3 toMonster = col.gameObject.transform.position - transform.parent.transform.position;
			transform.parent.transform.GetComponent<Rigidbody>().velocity = toMonster.normalized * transform.parent.transform.GetComponent<Rigidbody>().velocity.magnitude;
			count++;
		}
	}
}
