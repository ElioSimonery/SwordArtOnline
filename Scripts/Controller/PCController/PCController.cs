using UnityEngine;

public class PCController : MonoBehaviour
{
	private static PCController mInstance;
	private bool mPressed = false;
	
	void Awake()
	{
		mInstance = this;
		gameObject.SetActive(false);
		if(Application.platform == RuntimePlatform.WindowsEditor ||
		   Application.platform == RuntimePlatform.WindowsPlayer)
		{
			gameObject.SetActive(true);
		}
	}
	void Start()
	{
		deactivateVController ();
	}

	void FixedUpdate()
	{
		if (GameManager.PlayerObject == null)
			return;
		Direction ();
		Shooter ();
	}

	public static PCController GetInstance()
	{
		if (mInstance == null)
			Debug.LogError ("[PCController] has not been created.");

		return mInstance;
	}

	public void deactivateVController()
	{
		VFloatingController ctrl = VFloatingController.GetAgileInstance (true);
		if(ctrl != null)
			ctrl.gameObject.SetActive (false);
		VFloatingController shooter = VFloatingController.GetAgileInstance (false);
		if(shooter != null)
			shooter.gameObject.SetActive (false);
	}

	void Direction()
	{
		Vector3 dir = Vector3.zero;
		if (Input.GetKey (KeyCode.W))
			dir += Vector3.forward;
		if (Input.GetKey (KeyCode.S))
			dir += Vector3.back;
		if (Input.GetKey (KeyCode.A))
			dir += Vector3.left;
		if (Input.GetKey (KeyCode.D))
			dir += Vector3.right;

		if(dir == Vector3.zero)
			GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.move (false, dir);
		else
			GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.move (true, dir.normalized);
	}

	void Shooter()
	{
		if (Input.GetMouseButtonDown (0))
			mPressed = true;
		if (Input.GetMouseButtonUp (0))
			mPressed = false;

		Vector3 panel_pos = Input.mousePosition;
		panel_pos = new Vector3(panel_pos.x-Screen.width/2f, 0, panel_pos.y-Screen.height/2f);
		if(mPressed)
			GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.shoot(panel_pos.normalized);
		else
		{
			GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.shoot (Vector3.zero);
		}
	}

}
