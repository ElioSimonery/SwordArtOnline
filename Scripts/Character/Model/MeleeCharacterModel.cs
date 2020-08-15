

// <<Model Class>>
public class MeleeCharacterModel : CharacterModel
{
	public bool isMeleeAttackState { get; set; }

	protected int		MAX_COMBO;
	protected int		MAX_BASE_DAMAGE; // last combo attack damage will be max damage.
	protected float		COMBO_INIT_TIME = 2f;

	protected int 		mCurrentCombo = 0;
	protected float 	mComboTimer;
	protected MeleeAttackData[] mAttacks;
	
	public MeleeCharacterModel (MyClass iMyClass, int iWeaponSize, int iMax_HP, int iMax_MP, 
	                            float iMoveSpeed,
	                            int iCombo, float iComboInitTime, int[] iBaseDamage, float[] iPreDelay, float[] iPostDelay, float[] iMotionVelocity)
		: base(iMyClass, iWeaponSize, iMax_HP, iMax_MP, iMoveSpeed)
	{
		MAX_COMBO = iCombo;
		COMBO_INIT_TIME = iComboInitTime;

		mAttacks = new MeleeAttackData[MAX_COMBO];
		for(int i = 0; i < MAX_COMBO; i++)
		{
			mAttacks[i] = new MeleeAttackData();
			mAttacks[i].mBaseDamage = iBaseDamage[i];
			mAttacks[i].basePreDelay = iPreDelay[i];
			mAttacks[i].basePostDelay = iPostDelay[i];
			mAttacks[i].motionVelocity = iMotionVelocity[i];
			mAttacks[i].calculateData();
		}
		MAX_BASE_DAMAGE = mAttacks [MAX_COMBO - 1].mBaseDamage;
	}


	/* Data Policies */
	public override void addDamage(int dmg)
	{
		foreach (MeleeAttackData atk in mAttacks) 
		{
			float increaseRate = (float)atk.mBaseDamage/(float)MAX_BASE_DAMAGE;
			atk.addDamage((int)((float)dmg*increaseRate));
		}
	}

	public override void increaseDamage(float rate)
	{
		foreach(MeleeAttackData atk in mAttacks)
			atk.increasedDamageBy(rate);
	}
	
	public override void increaseAttackSpeed(float rate)
	{
		foreach(MeleeAttackData atk in mAttacks)
			atk.increasedSpeedBy(rate);
	}

	public override void calculateData()
	{
		foreach(MeleeAttackData atk in mAttacks)
			atk.calculateData();
	}

	/* Utilities */
	public bool isDelayFinished()
	{ return mAttacks [mCurrentCombo].postDelayFinished(); }

	public void cancelAttack(bool ignorePreDelay, bool ignorePostDelay)
	{
		if(getCurrentAttack().hit)
		{
			if(ignorePostDelay)
				prepareNextAttack ();
		}
		else 
		{
			if(ignorePreDelay)
				getCurrentAttack().init ();
		}
	}

	public void comboInitializer(float fixedDeltaTime)
	{
		// no attack will initialize combo
		mComboTimer += fixedDeltaTime;
		if(mComboTimer >= COMBO_INIT_TIME)
		{
			mComboTimer = 0f;
			mCurrentCombo = 0;
		}
		
	}

	public void prepareNextAttack()
	{
		mAttacks [mCurrentCombo].init ();
		mCurrentCombo = ((mCurrentCombo + 1) % MAX_COMBO);
	}

	// Function returns
	// current_attack combo
	public int setMeleeAttack(bool state)
	{
		isMeleeAttackState = state;
		return getCurrentCombo();
	}

	/* Simple Get/Set */
	public MeleeWeaponBase getCurrentEquippedMeleeWeapon() { return (MeleeWeaponBase)weapons [current_weapon]; }
	public bool isInitialized() { return mAttacks != null ? true : false; }
	public int getCurrentCombo() { return mCurrentCombo; }
	public MeleeAttackData[] getMeleeAttackData(){ return mAttacks; }
	public MeleeAttackData getCurrentAttack() {return mAttacks [mCurrentCombo]; }
}


