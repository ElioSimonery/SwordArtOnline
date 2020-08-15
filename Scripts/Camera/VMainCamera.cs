using UnityEngine;
using System.Collections;

// <<View Class>>
public class VMainCamera : MonoBehaviour {
	// Const static
	private static Vector3			NULL_VECTOR = Vector3.zero;
	private static readonly string	NULL_CHARACTER	= 
		"MainCamera.script : characterBase is null. please set to character.";

	// Internal UI Data
	protected GameObject	mCharacterBase	= null;
	protected float			mZoom			= 9.0f;
	private Vector3 		mNormalVector	= new Vector3(0.0f, 7.0f, -5.3f);

	void Start () {
		mNormalVector = mNormalVector.normalized;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (mCharacterBase == null)
			Debug.LogWarning (NULL_CHARACTER);
		Vector3 targetPos = mCharacterBase.transform.position + mZoom*mNormalVector;
		transform.position = Vector3.SmoothDamp (transform.position, targetPos, ref NULL_VECTOR, 0.1f);
	}

	public void setTrackingObject(ref GameObject character)
	{
		mCharacterBase = character;
	}

	public void setZoom(ref float zoom)
	{
		mZoom = zoom;
	}
}
