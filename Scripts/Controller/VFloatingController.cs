using UnityEngine;
using System.Collections;

// <<View class>> User Controller script
public class VFloatingController : MonoBehaviour {

	/* Unity interface */
	public GameObject pointObj;
	public bool 	isCharacterController = true;

	// View options
	public float	radius = 50f;
	public bool		isStatic = false;
	public bool		isSmooth = true;
	public bool		useInitializer = true;
	public float	initializePositionTime = 0.3f;
	public float	offsetSize = 120f;

	/* Internal data */
	private Vector3 	INITIAL_POS;
	private float		mInitialTimer;
	private UISprite[]	mSprites;
	private bool		mDisappearLock;

	// This is not singleton class, but wanna be accessed.
	private static VFloatingController mShooterInstance;
	private static VFloatingController mControlInstance;

	public static VFloatingController GetAgileInstance(bool controlInstance)
	{
		VFloatingController ret;
		if (controlInstance)
			ret = mControlInstance;
		else ret = mShooterInstance;

		if(ret == null)
			Debug.LogWarning("[VFloatingController] AgileInstance is null.");
		return ret; 
	}

	void Start () 
	{		
		if(!isCharacterController)
			mShooterInstance = this;
		else mControlInstance = this;
		INITIAL_POS = new Vector3();
		INITIAL_POS.x = this.gameObject.transform.localPosition.x;
		INITIAL_POS.y = this.gameObject.transform.localPosition.y;

		mSprites = GetComponentsInChildren<UISprite>();
		mDisappearLock = true;
	}

	void FixedUpdate () 
	{
		if(onPressed)
			movePoint(pointObj);

		if(useInitializer)
			positionInitializer ();

		if(isStatic)
			RoundedInnerPointStatic(pointObj, radius);
		else
			RoundedInnerPoint(pointObj, radius);
	}

	// Device independency
	protected const bool UPPER = true;
	protected const bool LOWER = false;
	protected static bool hasPositionX(float val, bool isOver)
	{
		if(Input.touchCount > 0)
			return hasTouchPositionX(val,isOver);
		else
			return (getMousePanelPosition().x > val) ? isOver : !isOver;
	}
	protected static bool hasTouchPositionX(float val, bool isOver)
	{
		Vector3[] v = getTouchPanelPosition (true);
		bool ret;
		for(int i = 0; i < v.Length; i++)
		{
			if(v[i].x > val)
				ret = true;
			else
				ret = false;
			
			if(!isOver) ret = !ret;
			
			if(ret) return ret;
		}
		return false;
	}

	// For Mouse Device
	protected static Vector3 getMousePanelPosition()
	{
		Vector3 panel_pos = Input.mousePosition; 
		panel_pos = new Vector3(panel_pos.x-Screen.width/2f, panel_pos.y-Screen.height/2f, 0);
		return panel_pos;
	}

	// For Touchable Device
	protected static Vector3[] getTouchPanelPosition(bool centerCoordinate)
	{
		int size = Input.touchCount;
		Vector3[] vts = new Vector3[size];

		for(int i = 0; i < size; i++)
		{
			Touch t = Input.GetTouch(i);
			Vector2 v = t.position;
			if(centerCoordinate)
			{
				v.x -= Screen.width/2f;
				v.y -= Screen.height/2f;
			}
			vts[i] = v;
		}

		return vts;
	}

	private void movePoint(GameObject point)
	{
		if(Input.touchCount == 0)
		{
			point.transform.localPosition = Input.mousePosition-offsetVector;
			return;
		}
		
		for(int i = 0; i < Input.touchCount; i++)
		{
			Vector3 pos = Input.GetTouch(i).position;
			float x = pos.x - Screen.width / 2f;
			
			if((isCharacterController && x < 0) || // controller or
			   (!isCharacterController && x > 0)) // shooter
			{
				point.transform.localPosition = pos-offsetVector;
				return;
			}
		}
	}

	public void RoundedInnerPoint(GameObject point, float r)
	{
		Vector3 pre_v = new Vector3();
		Vector3 result_v = new Vector3();
		
		if (Vector3.SqrMagnitude(point.transform.localPosition) > r*r) 
		{
			pre_v = point.transform.localPosition;
			Vector3 norm_v = pre_v.normalized;
			result_v = norm_v*r;
			point.transform.localPosition = result_v;
			Vector3 remainderVector = (pre_v-result_v);
			
			if(remainderVector.magnitude < offsetSize) return;
			remainderVector -= remainderVector.normalized*offsetSize;
			transform.localPosition += remainderVector;
			offsetVector += remainderVector;
		}
	}
	
	public void RoundedInnerPointStatic(GameObject point, float r)
	{
		Vector3 result_v =  new Vector3();
		if (Vector3.SqrMagnitude(point.transform.localPosition) > r*r) 
		{
			Vector3 norm_v = point.transform.localPosition.normalized;
			result_v = norm_v*r;
			point.transform.localPosition = result_v;
		}
	}

	// Auto Initializer when idle time.
	protected void positionInitializer()
	{
		if (isHandling()) 
		{
			mDisappearLock = false;
			mInitialTimer = 0.0f;
			foreach(UISprite sprite in mSprites)
				sprite.alpha = 1f;
		}
		else 
		{
			if(mDisappearLock) return;
			mInitialTimer += Time.deltaTime;
			disappearControlPos();
		}

		if (mInitialTimer >= initializePositionTime)
		{
			if (!mDisappearLock) 
			{
				initControlPos ();
			}
		}
	}

	public bool isHandling()
	{
		bool dragging = Input.GetMouseButton (0);
		if(dragging)
		{
			if(isCharacterController)
			{
				if(hasPositionX(0,LOWER))
					return true;
			}
			else
			{
				if(hasPositionX(0,UPPER))
					return true;
			}
		}
		return false;
	}

	private void initControlPos()
	{
		this.transform.localPosition = INITIAL_POS;
		pointObj.transform.localPosition = Vector3.zero;
		foreach(UISprite sprite in mSprites)
			sprite.alpha = 1f;
		mDisappearLock = true;
	}

	private void disappearControlPos()
	{
		pointObj.transform.localPosition = new Vector3(0f,0f,0f);

		if(isSmooth)
			foreach(UISprite sprite in mSprites)
				sprite.alpha = 1f-mInitialTimer/initializePositionTime;
	}
	// Force to release controller.
	public void forceToInitializeController()
	{
		OnReleasePoint();
		initControlPos ();
	}
	/* NGUI ButtonMessage Component call this method. */
	// Point handler
	private Vector3 offsetVector;
	private Vector3 pointWorldScreen;
	private bool onPressed =false;
	
	public void OnReleasePoint() 
	{
		onPressed = false;
	}

	public void OnPressPoint()
	{
		onPressed = true;
		pointWorldScreen = GameObject.Find ("Camera").GetComponent<Camera> ().WorldToScreenPoint (pointObj.transform.position);
		if(Input.touchCount > 0) // Mobile on press
		{
			if(isCharacterController)
			{
				for(int i = 0; i < Input.touchCount; i++)
				{
					if(Input.GetTouch(i).position.x < Screen.width/2f) // left is controller side.
					{
						offsetVector = Input.GetTouch(i).position;
						offsetVector -= offsetVector - pointWorldScreen;
						break;
					}
				}
			}
			else
			{
				for(int i = 0; i < Input.touchCount; i++)
				if(Input.GetTouch(i).position.x >= Screen.width/2f)
				{
					offsetVector = Input.GetTouch(i).position;
					offsetVector -= offsetVector - pointWorldScreen;
					break;
				}
			}
		}
		else // PC on press	
		{
			offsetVector = Input.mousePosition;
			offsetVector -= offsetVector - pointWorldScreen;
		}
	}
}
