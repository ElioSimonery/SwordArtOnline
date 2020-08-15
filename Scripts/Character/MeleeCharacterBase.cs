using UnityEngine;
using System.Collections;

// <<Control class>> Melee character base
public abstract class MeleeCharacterBase : ICCharacterBase {
	public const int WEAPON_IDLE = -1;
	/* Unity interface */
	public int		iCombo = 4;
	public float	iComboInitializeTime = 2f;
	public float[]	iPreDelay;
	public int[]	iBaseDamages;
	public float[]	iMotionVelocity;
	public float[]	iPostDelay;

	protected MeleeCharacterModel mModel 
	{
		get
		{ 
			if(model is MeleeCharacterModel) 
				return (MeleeCharacterModel)model; 
			else
				Debug.LogError("[MeleeCharacterBase] hasn't a real melee model.");
			return null;
		}

		set{ mModel = value; } 
	}

	protected virtual void Awake()
	{
		model = new MeleeCharacterModel (iCharacterClass, iWeaponCapability, iMAX_HP, iMAX_MP,
		                                 iMoveSpeed,
		                                 iCombo, iComboInitializeTime, iBaseDamages, iPreDelay, iPostDelay, iMotionVelocity);		
	}

	protected override void Start () 
	{
		base.Start ();
	}

	/* Base Move, Shoot Algorithms */
	public override void move(bool isMove, Vector3 v)
	{
		if(mModel.isMeleeAttackState)
			return;

		// no attack will initialize combo
		mModel.comboInitializer (Time.fixedDeltaTime);

		base.move (isMove, v);
	}

	// Function returns 
	// weapon equipment state.
	bool tmpItem = false;
	public override bool shoot(Vector3 dir)
	{
		if (model.getCurrentEquippedWeapon() == null)
		{
			if(tmpItem == true) return false;

			GameManager.World.GetComponent<ItemFactory>().GetSingleRandomWeapon(1.0f, ItemWeaponsTable.SHORT_SWORD, gameObject); // drop item.
			tmpItem = true;
			return false;
		}
		
		// Idle shoot handling
		if (dir == Vector3.zero) 
			return idleShoot (); // always return.

		int current_combo = mModel.setMeleeAttack (true);
		if (model.getCurrentEquippedWeapon().fire (this.gameObject, dir))
			SoundEffectManager.GetInstance().play(((MeleeWeaponBase)model.getCurrentEquippedWeapon()).
			                                      swingEffect[current_combo].name);

		// Handles 'view'
		transform.rotation = Quaternion.LookRotation (dir);
		if(!hasAttacked()) // Add velocity in preDelaying.
			transform.GetComponent<Rigidbody>().velocity = getAttackForwardVelocity () * dir.normalized; 
		anim.SetFloat("Speed", 0f);
		anim.SetInteger("Combo", current_combo);

		return true;
	}

	// Function 'Always' returns 
	// true and use return idleShoot() directly.
	protected bool idleShoot()
	{
		if(isDelayFinished())
		{
			// After the delay has been finished, prepare next things so we couldn't ignore postDelay.
			mModel.prepareNextAttack();
			mModel.setMeleeAttack(false);
			anim.SetInteger("Combo", WEAPON_IDLE); // set idle.
		}
		else
		{
			// if there is a remained predelay,		1.cancel the attack.
			// if there is a remained postdelay,	2.postdelaying.
			cancelAttack(true, false);

			// 1.Cancel the predelay.
			if(lessPreDelaying()) 
			{
				mModel.setMeleeAttack(false);
				anim.SetInteger("Combo", WEAPON_IDLE);
			}
			// 2.PostDelaying. call for postDelay calculating.
			else if(hasAttacked())
				mModel.getCurrentEquippedMeleeWeapon().coolingDelay(this.gameObject);
		}
		return true;
	}
	
	protected bool lessPreDelaying()
	{
		if (!mModel.isInitialized())
			return false;
		return mModel.getCurrentAttack().preDelaying();
	}

	protected bool hasAttacked()
	{
		if (!mModel.isInitialized())
			return false;
		return mModel.getCurrentAttack().hit;
	}
	
	protected bool isDelayFinished()
	{
		if (!mModel.isInitialized())
			return false;
		return mModel.isDelayFinished();
	}

	protected void cancelAttack(bool ignorePreDelay, bool ignorePostDelay)
	{
		if (!mModel.isInitialized()) 
			return;
		mModel.cancelAttack (ignorePreDelay, ignorePostDelay);
	}
	
	public void prepareNextAttack()
	{
		mModel.prepareNextAttack ();
	}

	protected float getAttackForwardVelocity()
	{
		if (mModel.isInitialized())
			return mModel.getCurrentAttack().motionVelocity;
		else return 0f;
	}

	/* Sugar Syntax for model */
	public MeleeAttackData getCurrentAttack() { return mModel.getCurrentAttack(); }
	public int getCurrentCombo() { return mModel.getCurrentCombo(); }

	public override void addDamage(int dmg)
	{ mModel.addDamage (dmg); }
	
	public override void increaseDamage(float rate)
	{ mModel.increaseDamage (rate); }
	
	public override void increaseAttackSpeed(float rate)
	{ mModel.increaseAttackSpeed (rate); }
	
	public override void calculateData()
	{ mModel.calculateData (); }

}
