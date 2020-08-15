using UnityEngine;

public class Stash : MonoBehaviour
{
	public string SceneName = "stashScene";
	public int stashHeightSize;
	public int stashWidthSize;
	public GameObject[,] stashItems;

	private static Stash instance;
	
	private void Awake()
	{
		stashItems = new GameObject[stashHeightSize,stashWidthSize];
		instance = this;
		Debug.Log ("stash created.");
	}

	public static Stash GetInstance()
	{
		if (instance == null)
			Debug.Log ("Stash haven't ever created.");

		return instance;
	}

	public bool AddItem(GameObject itemCube)
	{
		bool ret = false;
		for(int i = 0; i < stashHeightSize; i++) for(int j = 0; j < stashWidthSize; j++)
		{
			if(stashItems[i,j] == null)
			{
				itemCube.GetComponent<ItemCube> ().setStashItem (true);
				stashItems[i,j] = itemCube;
				ret = true;
				break;
			}
		}
		if (!ret)
			return ret;
		
		//insert query.
		Debug.Log ("saved item"+itemCube.name);
		return ret;
	}
}

