
// <<Entity Class>>
public class LevelUpTable
{
	public const int DEFAULT_TABLE = 0;
	public const int AUTO_TABLE = 1;

	protected int[,] LVL;

	public LevelUpTable ()
	{
		LVL = new int[2, 100];

		LVL [AUTO_TABLE, 0] = 200;
		for(int i = 1; i < 100; i++)
		{
			LVL[AUTO_TABLE,i] = i;//i*1000 + (int)((0.1f)*LVLTable[AUTO_TABLE,i-1]);
		}
	}

	public int[,] LVLTable(int policy)
	{
		return LVL;
	}
}

