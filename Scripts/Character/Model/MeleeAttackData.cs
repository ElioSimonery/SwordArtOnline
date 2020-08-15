
// <<Entity Class>>
public class MeleeAttackData : AttackData
{
	/* Base delay */
	public float basePreDelay;
	public float basePostDelay;

	// Result of calculation. Use these value.
	public float mPreDelay;
	public float mPostDelay;
	
	public float motionVelocity;
	
	public bool preDelaying() { return (timer < mPreDelay) ? true : false; }
	public bool preDelayFinished() { return !preDelaying (); }

	public override void calculateData()
	{
		base.calculateData ();
		mPreDelay	= mAttackSpeedRate * basePreDelay;
		mPostDelay	= mAttackSpeedRate * basePostDelay;
	}

	// Function returns
	// true only if in postDelaying.
	public bool postDelaying() {
		if (preDelaying ())
			return false;

		if (timer < mPostDelay)
			return true;
		else return false;
	}

	public bool postDelayFinished()
	{
		if (timer > mPostDelay)
			return true;
		else return false;
	}

	// make get set, returns calculated delay.
}