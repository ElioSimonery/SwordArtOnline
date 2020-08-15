using UnityEngine;
using System.Collections;

//<<Boundary Class>> User's Input data is transported to CharacterBase
public class BFloatingController : MonoBehaviour
{
	public VFloatingController	view;
	public GameObject			TargetCharacter { get; set; }

	void FixedUpdate () 
	{
		if(view.isCharacterController)
			MoveTargetOf (TargetCharacter, view.pointObj);
		else // Shooter
			FireTargetOf (TargetCharacter, view.pointObj);
	}

	public void MoveTargetOf(GameObject characterObject, GameObject point)
	{
		Vector3 pos_controller = point.transform.localPosition;
		if(pos_controller.magnitude < 0.1f)
		{
			characterObject.GetComponent<CharacterControlHelper> ().c.move(false, Vector3.zero);
			return;
		}
		if (!view.isHandling ()) return;
		
		Vector3 v = new Vector3(pos_controller.x, 0f, pos_controller.y);
		v /= view.radius;
		characterObject.GetComponent<CharacterControlHelper> ().c.move (true, v);
	}
	
	// Fire
	public void FireTargetOf(GameObject characterObject, GameObject point)
	{
		Vector3 pos_controller = point.transform.localPosition;
		if (pos_controller.sqrMagnitude <= 0.1f)
		{
			characterObject.GetComponent<CharacterControlHelper>().c.shoot(Vector3.zero); // don't shoot.
			return;
		}
		if (!view.isHandling ()) return;
		
		Vector3 shoot_dir = new Vector3(pos_controller.x, 0f, pos_controller.y);
		shoot_dir /= view.radius;
		if(!characterObject.GetComponent<CharacterControlHelper>().c.shoot(shoot_dir))
		{
			NotificationManager nm = NotificationManager.GetInstance();
			nm.toast (nm.NO_WEAPON[Configuration.LanguageSelect]);
		}
	}
}

