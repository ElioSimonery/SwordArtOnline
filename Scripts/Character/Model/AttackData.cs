using UnityEngine;

// <<Entity Class>>
public class AttackData
{
	public const float TimerZero = 0f;
	public const float MAX_SPEED_RATE = 100;

	// Base damage of its model.
	public int		mBaseDamage; 
	
	// Result of calculation. Use this value actually.
	public int		mResultDamage;
	public float	mAttackSpeedRate;
	
	public bool 	hit { get; set; }
	public float	timer = 0f;

	// Internal data for calculation.
	protected int	accumDamage = 0;		// + summation
	protected float	damageRate = 1f;		// * multiplication
	protected float	inverseSpeedRate = 1f;	// * multiplication

	public AttackData()
	{
		init ();
	}

	public int attack()
	{
		calculateData ();
		hit = true;
		return mResultDamage;
	}
	
	public void init()
	{
		hit = false;
		timer = TimerZero;
	}
	
	// Apply calculate policy.
	public virtual void calculateData()
	{
		checkLimitation ();

		mResultDamage = (int)((accumDamage + mBaseDamage) * damageRate);

		mAttackSpeedRate = (1/inverseSpeedRate);
	}

	protected virtual void checkLimitation()
	{
		if (inverseSpeedRate > 100)
			inverseSpeedRate = 100;
	}

	/* interface */
	// Functions return
	// result value of current calculation.
	public float increasedDamageBy(float rate)
	{ 
		damageRate += rate; 
		calculateData ();
		return damageRate;
	}
	
	public int addDamage(int accum)
	{ 
		accumDamage += accum; 
		calculateData ();
		return accumDamage;
	}
	
	public float increasedSpeedBy(float rate)
	{ 
		inverseSpeedRate += rate; 
		calculateData ();
		return inverseSpeedRate;
	}
	
}