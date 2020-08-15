
using System;

// <<Model Class>>
// Character model data
public abstract class CharacterModel
{
	protected int MAX_HP = 300;
	protected int MAX_MP = 100;
	protected MyClass characterClass;

	protected int hp = 300;
	protected int mp = 100;
	protected float moveSpeed = 1f;
	
	public bool lockHP { get; set; }
	public bool isDead { get; set; }
	
	protected int weaponCapability = 3;
	protected WeaponBase[] weapons;
	protected int current_weapon;

	public enum MyClass
	{
		Warrior,
		Gunner,
		Magician
	};

	public abstract void addDamage (int dmg);
	public abstract void increaseDamage (float rate);
	public abstract void increaseAttackSpeed (float rate);
	public abstract void calculateData ();

	/** Initializer **/
	public CharacterModel(MyClass iMyClass, int iWeaponSize, int iMax_HP, int iMax_MP,
	                      float iMoveSpeed)
	{
		//set data.
		characterClass = iMyClass;
		weaponCapability = iWeaponSize;
		hp = MAX_HP = iMax_HP;
		mp = MAX_MP = iMax_MP;
		moveSpeed = iMoveSpeed;

		if(weapons == null)
			weapons = new WeaponBase[weaponCapability];
	}

	public void OnInitWeapons(ref WeaponBase[] _weapons, int _idx) 
	{ 
		weapons = _weapons; 
		current_weapon = _idx; 
	}

	/** Utilities **/

	public bool canHit()
	{
		if (isDead)
			return false;
		if (lockHP)
			return false;

		return true;
	}

	// Function returns
	// true when live
	// else false
	public virtual bool getDamage(int damage)
	{
		if (hp - damage > 0)
		{
			hp -= damage;
			if(hp > MAX_HP) hp = MAX_HP;
			return true;
		}
		else
		{
			// Dead.
			hp = 0;
			isDead = true;
			return false;
		}
	}

	// It can be implemented through inheritance, but it is quite visible than that.
	public bool canEquip(WeaponBase weapon)
	{
		switch(characterClass)
		{
		case MyClass.Gunner:
			if(weapon.type == WeaponBase.WeaponType.Gun)
				return true;
			break;
		case MyClass.Warrior:
			if(weapon.type == WeaponBase.WeaponType.Sword)
				return true;
			break;
		case MyClass.Magician:
			if(weapon.type == WeaponBase.WeaponType.Dragon)
				return true;
			break;
		default:
			return false;
		}
		return false;
	}

	// Function returns
	// prev equipped weapon.
	public virtual WeaponBase equipWeapon(ref WeaponBase weapon)
	{
		WeaponBase ret = weapons [current_weapon];
		weapons[current_weapon] = weapon;
		return ret;
	}

	// Function returns
	// prev equipped weapon.
	public WeaponBase unEquipCurrentWeapon()
	{
		WeaponBase ret = weapons [current_weapon];
		weapons [current_weapon] = null;
		return ret;
	}

	/** Get/Set **/
	public WeaponBase[] getWeapons() {return weapons; }
	public int getCurrentIdx() { return current_weapon; }
	public int getWeaponCapability () { return weaponCapability; }
	public WeaponBase getCurrentEquippedWeapon() { return weapons [current_weapon]; }

	public MyClass getMyClass() { return characterClass; }
	public int getMaxHP() { return MAX_HP; }
	public int getHP() { return hp; }
	public void setHP(int _hp) { hp = (_hp > MAX_HP) ? MAX_HP : _hp; }
	public virtual float getMoveSpeed() { return moveSpeed; }
}

