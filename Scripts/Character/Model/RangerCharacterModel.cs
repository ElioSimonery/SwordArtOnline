using UnityEngine;

// <<Model Class>>
public class RangerCharacterModel : CharacterModel
{
	protected RangeAttackData	attack;
	protected bool isMovingShot = false;
	public RangerCharacterModel(MyClass iMyClass, int iWeaponSize, int iMax_HP, int iMax_MP, 
	                            float iMoveSpeed,
	                            int iBaseDamage)
		: base(iMyClass, iWeaponSize, iMax_HP, iMax_MP, iMoveSpeed)
	{
		attack = new RangeAttackData();
		attack.mBaseDamage = iBaseDamage;
		attack.calculateData ();
	}

	public void movingShot(bool isMoving)
	{
		isMovingShot = isMoving;
	}

	public override float getMoveSpeed ()
	{
		if(isMovingShot)
			return base.getMoveSpeed () * 2f / 3f;
		else 
			return base.getMoveSpeed ();
	}



	/* Data Policies */
	public override void addDamage(int dmg) { attack.addDamage (dmg); }
	public override void increaseDamage(float rate) { attack.increasedDamageBy(rate); }
	public override void increaseAttackSpeed(float rate) { attack.increasedSpeedBy(rate); }
	public override void calculateData() { attack.calculateData(); }
	/* Simple Get/Set */
	public RangeAttackData getRangeAttackData() { return attack; }
	public float getFireRate() { return attack.mFireRate; }
	public void setBaseFireRate(float fireRate) 
	{ 
		attack.basePostDelay = fireRate; 
		attack.calculateData ();
	}

}
