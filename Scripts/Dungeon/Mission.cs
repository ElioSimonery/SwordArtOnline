
public abstract class MissionBase
{
	public float startTime = 0f;
	public bool isCleared = false;
	public bool isInProgress = false;
	
	public virtual void start()
	{
		isInProgress = true;
	}
	
	public abstract bool checkMission ();
}