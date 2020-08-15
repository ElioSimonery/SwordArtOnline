using UnityEngine;

public class BabelUtils
{
	public static float PI = Mathf.PI;
	public static Vector3 RotateXZ(Vector3 v, float rad)
	{
		return new Vector3 (v.x * Mathf.Cos (rad) - v.z * Mathf.Sin (rad), 0, 
		                    v.x * Mathf.Sin (rad) + v.z * Mathf.Cos (rad));
	}

	public static Vector2 GetRandomVector2(float size)
	{
		Vector2 ret = new Vector3 (Random.Range(-1f,1f),Random.Range(0f,1f));
		ret.Normalize ();
		ret *= size;
		return ret;
	}
}


