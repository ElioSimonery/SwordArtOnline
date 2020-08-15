using UnityEngine;

public class ItemWeaponsTable
{
	public static int _START = 1;
	public static int BRANCH_GUN = 1; // 1 <= < 5
	public static int AUTO_GUN = 6; // 5<= <9
	public static int SHORT_SWORD = 11;
	public static int _END = 16;

	private static int[] arr = 
	{
		BRANCH_GUN, 
		AUTO_GUN,
		SHORT_SWORD,
		_END
	};


	public static int GetItem(int seed)
	{
		for (int i = 0; i < arr.Length; i++) 
		{
			if(arr[i] > seed)
			{
				if(i == 0) return 0;

				return arr[i-1];
			}
		}
		return -1;
	}
}


