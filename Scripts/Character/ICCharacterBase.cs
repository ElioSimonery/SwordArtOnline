using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClass = CharacterModel.MyClass;

// <<Control class>> CharacterBase interface.
public abstract class ICCharacterBase : MonoBehaviour {
	// You don't have to input this value, code will find the animation from its gameObject.
	public Animator anim;

	// For Unity Interface variables,
	// These aren't be handled internal data for any calculation. so don't worry.
	public int 		iMAX_HP = 300;
	public int 		iMAX_MP = 100;
	public float	iMoveSpeed = 1f;
	public MyClass 	iCharacterClass = MyClass.Warrior;
	public int		iWeaponCapability = 3;

	// Real model data
	protected CharacterModel model;

	protected virtual void Awake()
	{
		Debug.Log("Character Awake called");
	}

	// Use this for initialization
	protected virtual void Start () {
		anim = GetComponent<Animator> ();
		// init model in derived class.
		GameManager.PlayerHud.GetComponent<MiniHud> ().setPoint (true, iMAX_HP);
		GameManager.PlayerHud.GetComponent<MiniHud> ().setPoint (false, iMAX_MP);
	}

	public abstract void addDamage(int dmg);
	public abstract void increaseDamage(float rate);
	public abstract void increaseAttackSpeed(float rate);
	public abstract void calculateData();

	protected bool isFloating = false;
	public virtual void move(bool isMove, Vector3 v)
	{
		if (isMove)
		{
			transform.rotation = Quaternion.LookRotation (v);

			v *= model.getMoveSpeed();
			if(isFloating) v += Physics.gravity;
			GetComponent<Rigidbody>().AddForce ((v - GetComponent<Rigidbody>().velocity), ForceMode.VelocityChange);

			anim.SetFloat ("Speed", 1f);
		}
		else
		{
			anim.SetFloat("Speed", 0f);
		}
	}

	protected virtual void OnCollisionStay(Collision col)
	{
		if (col.gameObject.tag == Common.TAG_OBJECT ||
		    col.gameObject.tag == Common.TAG_TERRAIN)
		{
			isFloating = false;
		}

	}
	protected virtual void OnCollisionExit(Collision col)
	{
		if (col.gameObject.tag == Common.TAG_OBJECT ||
		    col.gameObject.tag == Common.TAG_TERRAIN)
		{
			isFloating = true;
		}
	}
	// Function returns 
	// weapon equipped state.

	public virtual bool shoot(Vector3 dir)
	{
		if (dir == Vector3.zero)
			return false; // false?
		if(model.getCurrentEquippedWeapon() != null)
		{
			model.getCurrentEquippedWeapon().fire (this.gameObject, dir);
			return true;
		}
		return false;
	}

	// Function returns :
	// true when hit
	// false when couldn't hit
	public virtual bool hit(int damage)
	{
		if (!model.canHit ())
			return false;

		if(model.getDamage (damage))
		{
			GameManager.PlayerHud.GetComponent<MiniHud>().
				setData (true, model.getHP(), (float)model.getHP()/(float)model.getMaxHP());
			return true;
		}
		else
		{
			// add dead animation
			GameManager.OnDeadFinalizeScene();
			gameObject.SetActive(false);
			return false;
		}
	}

	public bool canEquip(WeaponBase weapon)
	{
		return model.canEquip (weapon);
	}

	// Function returns 
	// prev weapon
	// if null, cannot equip the weapon or prev weapon was null.
	public virtual WeaponBase equipWeapon(WeaponBase weapon)
	{
		if (!model.canEquip (weapon))
			return null;

		WeaponBase prevWeapon = model.equipWeapon (ref weapon);
		model.getCurrentEquippedWeapon().onEquipApplyData (this);
		return prevWeapon;
	}

	// Function returns
	// prev weapon
	public virtual WeaponBase unequipWeapon()
	{
		anim.SetBool("Equipped", false);
		model.getCurrentEquippedWeapon().unEquipApplyData (this);
		return model.unEquipCurrentWeapon ();
	}

	public virtual void onInitSetWeapons(ref WeaponBase[] inWeapon, int idx)
	{ 
		model.OnInitWeapons(ref inWeapon, idx); 
		if(getCurrentWeapon() != null)
			getCurrentWeapon().onEquipApplyData (this); 
	}

	/* Simple get/set and utility functions */

	public WeaponBase[] getWeapons()
	{ return model.getWeapons(); }

	public int getCurrentWeaponIdx()
	{ return model.getCurrentIdx(); }
	public WeaponBase getCurrentWeapon()
	{ return model.getCurrentEquippedWeapon(); }

	public void setHP(int hp) { model.setHP (hp); }
	public int viewWeaponCapability() { return model.getWeaponCapability (); }
	public int viewMaxHP() { return model.getMaxHP(); }
	public int viewHP() { return model.getHP(); }
	public float viewHPRate() {return (float)viewHP () / (float)viewMaxHP ();}

	public void setLock(bool locked) { model.lockHP = locked; }
	public bool getLock() { return model.lockHP; }
	public bool isDead() {return model.isDead; }
}
