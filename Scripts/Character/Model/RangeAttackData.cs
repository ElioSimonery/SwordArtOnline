
// <<Entity Class>>
public class RangeAttackData : AttackData
{
	/* Base delay */
	public float basePostDelay; // equipped item will fix this value.

	// Result of fireRate.
	public float mFireRate;

	// Apply calculate policy.
	public override void calculateData()
	{
		base.calculateData ();

		mFireRate = basePostDelay * mAttackSpeedRate;
	}
}

