

// will be easily attached to GameObject
public class ItemPartsTable
{
	public static int _START = 1;
	public static int TRIPLE_PART = 1; // <= value <
	public static int GUIDE_PART = 25;
	public static int WATER_PART = 50;
	public static int FIRE_PART = 75;
	public static int WIND_PART = 100;
	public static int HOLY_PART = 125;
	public static int DARK_PART = 150;
	public static int _END = 175;

	private static int[] arr = 
	{
		TRIPLE_PART, 
		GUIDE_PART,

		WATER_PART,
		FIRE_PART,
		WIND_PART,
		HOLY_PART,
		DARK_PART,
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


