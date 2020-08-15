using UnityEngine;


public class WarpScript : MonoBehaviour
{
	public GameObject toLocation;

	protected virtual void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_PLAYER)
			Teleport(col.gameObject, toLocation.transform.position);
	}

	public static void Teleport(GameObject who, Vector3 destLocation)
	{
		who.transform.position = destLocation;
	}

}
