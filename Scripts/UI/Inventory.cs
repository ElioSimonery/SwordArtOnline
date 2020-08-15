using UnityEngine;
using System.Collections;

// Singleton
public class Inventory {
	public const int MAX_WIDTH = 4;
	public const int MAX_HEIGHT = 2;
	public const int MAX_SIZE = MAX_WIDTH * MAX_HEIGHT;
	public GameObject[,] items;


	private static Inventory instance;

	private Inventory()	
	{
		items = new GameObject[MAX_HEIGHT,MAX_WIDTH];
		for(int i = 0; i < MAX_HEIGHT; i++)
		{
			for(int j = 0; j < MAX_WIDTH; j++)
			{
				GameManager.Destroy(items[i,j]);
			}
		}
	}

	public static Inventory getInstance()
	{
		if(instance == null)
			instance = new Inventory ();

		return instance;
	}

	public bool hasItem(GameObject obj)
	{
		for(int h = 0; h < MAX_HEIGHT; h++)
			for(int w = 0; w < MAX_WIDTH; w++)
				if(items[h,w] == obj)
					return true;
		return false;
	}

	public bool insertItem(GameObject cubeObj)
	{
		for(int h = 0; h < MAX_HEIGHT; h++)
		{
			for(int w = 0; w < MAX_WIDTH; w++)
			{
				if(items[h,w] == null)
				{
					items[h,w] = cubeObj;
					return true;
				}
			}
		}

		return false; // inventory full.
	}

	public void removeItem(GameObject item)
	{
		for(int h = 0; h < MAX_HEIGHT; h++)
		{
			for(int w = 0; w < MAX_WIDTH; w++)
			{
				if(items[h,w] == item)
				{
					items[h,w] = null;
				}
			}
		}
	}

	public int size()
	{
		return items.Length;
	}
} 