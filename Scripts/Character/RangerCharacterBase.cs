using UnityEngine;

// <<Control class>> Ranger character base
public abstract class RangerCharacterBase : ICCharacterBase
{
	/* Unity interface */
	public int iBaseDamage;

	protected RangerCharacterModel mModel 
	{
		get
		{ 
			if(model is RangerCharacterModel) 
				return (RangerCharacterModel)model; 
			else
				Debug.LogError("RangerCharacterBase hasn't a real melee model.");
			return null;
		}
		
		set{ mModel = value; }
	}

	protected virtual void Awake()
	{
		model = new RangerCharacterModel (iCharacterClass, iWeaponCapability, iMAX_HP, iMAX_MP,
		                                  iMoveSpeed,
		                                  iBaseDamage);
	}

	protected override void Start () 
	{
		base.Start ();
	}
	
	public override void move(bool isMove, Vector3 v)
	{
		base.move (isMove, v);
	}

	bool tmpItem = false;
	public override bool shoot(Vector3 dir)
	{
		if (model.getCurrentEquippedWeapon() == null)
		{
			if(tmpItem == true) return false;
			
			GameManager.World.GetComponent<ItemFactory>().GetSingleRandomWeapon(1.0f, ItemWeaponsTable.BRANCH_GUN, gameObject); // drop item.
			tmpItem = true;
			return false;
		}

		if(dir == Vector3.zero)
		{
			mModel.movingShot(false);
			anim.SetBool("Fire", false);
		}
		else
		{
			mModel.movingShot(true);
			anim.SetBool("Fire", true);
			transform.rotation = Quaternion.LookRotation (dir);
		}

		return base.shoot (dir);
	}

	public override WeaponBase equipWeapon(WeaponBase weapon)
	{
		if (!model.canEquip (weapon))
			return null;

		WeaponBase prev_weapon = base.equipWeapon(weapon);
		anim.SetBool("Equipped", true);
		return prev_weapon;
	}

	public override WeaponBase unequipWeapon()
	{
		anim.SetBool("Equipped", false);
		return base.unequipWeapon ();
	}

	public override void onInitSetWeapons(ref WeaponBase[] inWeapon, int idx)
	{ 
		model.OnInitWeapons(ref inWeapon, idx); 
		if(getCurrentWeapon() != null)
		{
			getCurrentWeapon().onEquipApplyData (this); 
			anim.SetBool("Equipped", true);
		}
		else anim.SetBool("Equipped", false);
	}

	/* Sugar Syntax for model */
	public RangeAttackData getCurrentAttack() { return mModel.getRangeAttackData(); }
	public override void calculateData() { mModel.calculateData (); }
	public void setBaseFireRate(float fireRate)	{ mModel.setBaseFireRate (fireRate); }
	public override void addDamage(int dmg) { mModel.addDamage (dmg); }
	public override void increaseDamage(float rate)	{ mModel.increaseDamage (rate); }
	public override void increaseAttackSpeed(float rate) { mModel.increaseAttackSpeed (rate); }
	public float getFireRate() { return mModel.getFireRate(); }

}
