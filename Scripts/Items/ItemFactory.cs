using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemFactory : MonoBehaviour {

	protected Dictionary<int,GameObject> partsFactory = new Dictionary<int, GameObject>();
	protected Dictionary<int,GameObject> weaponsFactory = new Dictionary<int, GameObject>();

	void Start()
	{

	}

	public GameObject GetSingleRandomPart(float randomVariable, int itemSeed, GameObject posObj)
	{
		GameObject ret = GetSingleRandomPart(randomVariable, itemSeed, posObj.transform.position);
		return ret;
	}

	public GameObject GetSingleRandomPart(float randomVariable, int itemSeed, Vector3 pos)
	{
		float seed = Random.Range (0.0f, 1.0f);
		if (randomVariable > seed) 
		{
			GameObject target = GetRandomPart(itemSeed);
			if(target == null) 
			{
				Debug.Log("[ItemFactory] Drop Item Logic Error. Please check item table and factory.");
				return null;
			}
			GameObject newItem = (GameObject)Instantiate(target);
			float rank = Random.Range((int)ItemRank.NORMAL, (int)ItemRank.EPIC+1);
			if(newItem.GetComponent<ItemCube>().part != null)
				newItem.GetComponent<ItemCube>().part.initData((ItemRank)rank);
			
			newItem.SetActive(true);
			newItem.transform.position = pos;
			Debug.Log("[ItemFactory] Dropped an random part item : " + newItem.name 
			          + " at position " + newItem.transform.position);
			return newItem;
		}
		else return null;


	}

	public GameObject GetSingleRandomWeapon(float randomVariable, int itemSeed, GameObject posObj)
	{
		GameObject ret = GetSingleRandomWeapon(randomVariable, itemSeed, posObj.transform.position);
		return ret;
	}

	public GameObject GetSingleRandomWeapon(float randomVariable, int itemSeed, Vector3 pos)
	{

		float seed = Random.Range (0.0f, 1.0f);
		if (randomVariable > seed) 
		{
			GameObject target = GetRandomWeapon(itemSeed);
			if(target == null) 
			{
				Debug.LogWarning("[ItemFactory] Drop Item Logic Error. Please check item table and factory.");
				return null;
			}
			GameObject newItem = (GameObject)Instantiate(target);
			float rank = Random.Range((int)ItemRank.NORMAL, (int)ItemRank.EPIC+1);
			newItem.SetActive(true);
			newItem.GetComponent<ItemCube>().weapon.initData((ItemRank)rank);
			newItem.transform.position = pos;
			SoundEffectManager sm = SoundEffectManager.GetInstance();
			sm.play (sm.DROPPED_ITEM);
			Debug.Log("[ItemFactory] Dropped a random weapon item : " +newItem.name
			          + " at position " + newItem.transform.position);
			return newItem;
		}
		else return null;

	}

	public GameObject GetRandomPart(int seed)
	{
		if(seed == RANDOM)
			seed = Random.Range (ItemPartsTable._START, ItemPartsTable._END);
		int idx = ItemPartsTable.GetItem (seed);

		GameObject ret;
		if (partsFactory.TryGetValue (idx, out ret))
		{
			return ret;
		}
		else 
			return null;
	}

	public const int RANDOM = -1;
	public GameObject GetRandomWeapon(int seed)
	{
		if(seed == RANDOM)
			seed = Random.Range (ItemWeaponsTable._START, ItemWeaponsTable._END);
		int idx = ItemWeaponsTable.GetItem (seed);
		
		GameObject ret;
		if (weaponsFactory.TryGetValue (idx, out ret))
		{
			return ret;
		}
		else 
			return null;
	}

	public bool registItem(bool weapon, int k, GameObject obj)
	{
		Dictionary<int,GameObject> factory;

		if (weapon)
			factory = weaponsFactory;
		else
			factory = partsFactory;

		if (obj == null) 
		{
			Debug.Log("regist item is currently null.");
			return false;
		}
		if (factory.ContainsKey (k))
			return false;

		factory.Add (k, obj);
		return true;
	}
}
