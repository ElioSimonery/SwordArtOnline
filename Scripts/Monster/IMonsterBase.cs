using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// <<Control class>> // abstract
public class IMonsterBase : MonoBehaviour {
	/* Unity Interface */
	public SphereCollider wakeCollider;
	public Animator		anim;

	// Model & View class
	protected MonsterModel model;
	protected MonsterView view;

	/* Unity Interface */
	public int iMax_HP = 50;
	public int iMax_MP = 50;
	// Attack Policies
	public int iAttackDamage = 10;
	public float iAttackRange = 3.0f;
	public float iAttackCooltime = 1f; // attack speed
	public float iAttackPostDelay = 0.5f; // postDelay is equal or less than cooltime.
	// Idletime Prowling Policies
	public bool iCanProwling = true;
	public float iProwlingInterval = 5.0f;
	public float iProwlingDeviation = 2.0f;
	public float iProwlingDuration = 0.5f; 
	// Speed
	public float iMoveSpeed = 5.0f;
	// Item Drop Rate
	public float iWeaponDropRate = 1.0f;
	public float iPartDropRate = 1.0f;

	// Use this for initialization
	protected virtual void Awake()
	{
		model = new MonsterModel (iMax_HP, iMax_MP, 
		                          iAttackDamage, iAttackRange, iAttackCooltime, iAttackPostDelay, 
		                          iCanProwling, iProwlingInterval, iProwlingDeviation, iProwlingDuration,
		                          iMoveSpeed, iWeaponDropRate, iPartDropRate);
		view = GetComponent<MonsterView> ();
		if (!view) 
			Debug.LogError("put MonsterView Script to the Monster "+gameObject.name);
	}

	protected virtual void Start () 
	{
		if(wakeCollider == null)
			Debug.LogError("wake collider is null.");
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		model.UpdateData (Time.deltaTime);

		if (!GameManager.PlayerObject) 
		{
			model.initBehavioralState();
			model.UpdateSufferedState ();
			anim.SetBool("isWake", false);
			anim.SetBool("isAttack", false);
			return;
		}

		if (model.hasPostDelay)
		{
			model.UpdateSufferedState();
			view.freezeVelocity();
			return;
		}

		model.UpdateSufferedState (); // take action can be bothered by specific state.
		OnUpdateTakeAction ();
	}
	
	protected virtual void OnUpdateTakeAction()
	{
		if (model.isAttackState) 
		{
			anim.SetBool ("isAttack", true);
			tryAttack (GameManager.PlayerObject);
		}
		else anim.SetBool("isAttack", false);
		
		if(model.isWakeState)
		{
			anim.SetBool("isWake", true);
			tryMove ();
		}
		else anim.SetBool("isWake", false);
		
		tryProwling ();
	}

	public void forceToWakeUp()
	{
		model.isWakeState = true;
		model.isProwling = false;
	}
	
	public virtual bool hit(int damage)
	{
		if(model.isDead) //hit function can be multiply called.
		{
			view.OnDestroyView();
			Destroy(gameObject);
			return false;
		}

		if (model.getDamage (damage))
		{
			view.OnHitEffect(getHPRate());
			forceToWakeUp();
			return true;
		}


		dropRandomItem();
		OnDeadDestroy ();
		return false;
	}

	public void OnDeadDestroy()
	{
		view.OnDestroyView();
		GameManager.CallGMComponent (GMAccMonsterComponent.COMPONENT_NAME);
		Destroy(gameObject);
	}

	// Drop Policy. this code will be migrated
	public virtual GameObject[] dropRandomItem()
	{
		GameObject[] ret = new GameObject[2];
		Vector3 pos = gameObject.transform.position;
		ret[0] = GameManager.World.GetComponent<ItemFactory>().GetSingleRandomPart(model.part_drop_rate, ItemFactory.RANDOM, pos); // drop item.
		pos.x += Random.Range (-1f, 1f);
		pos.z += Random.Range (-1f, 1f);
		ret[1] = GameManager.World.GetComponent<ItemFactory>().GetSingleRandomWeapon(model.weapon_drop_rate, ItemFactory.RANDOM, pos); // drop item.
		return ret;
	}

	protected virtual bool tryAttack(GameObject target)
	{
		if(model.attack()) //update data on attack
		{
			GameManager.PlayerObject.GetComponent<ICCharacterBase> ().hit (model.getAttackDamage());
			return true;
		}
		return false;
	}

	protected virtual bool tryMove()
	{
		if (model.isWakeState && !model.isAttackState) 
		{
			view.moveToCharacter(model.getMoveSpeed());
			return true;
		}

		return false;
	}

	protected virtual bool tryProwling()
	{
		Vector3 v = model.prowling (Time.deltaTime);
		if(v != Vector3.zero)
		{
			view.prowling(v);
			return true;
		}
		return false;
	}
	
	// Wake Collider
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.tag == Common.TAG_PLAYER)
		{
			forceToWakeUp();
		}
	}

	protected virtual void OnTriggerStay(Collider other) { // make abstract
		// in wake state.
		if(other.tag == Common.TAG_PLAYER)
			model.updateAttackState(transform.position);
	}

	void OnTriggerExit(Collider col) 
	{
		if(col.gameObject.tag == Common.TAG_PLAYER)
		{
			model.isWakeState = false;
			model.isProwling = true;
		}
	}

	public void pushSufferedState(SufferedState s) { model.pushSufferedState (s); }
	
	public virtual int viewHP() { return model.getHP(); }
	public int getMaxHP() {return model.MAX_HP; }
	public float getHPRate() { return (float)model.getHP () / (float)model.getMaxHP(); }
	public void upgrade(float rate) { model.upgrade (rate); }
	public float getWakeDistance() {return wakeCollider.radius; }
	public void setMoveSpeed(float value) { model.setMoveSpeed (value); }
	public float getMoveSpeed() { return model.getMoveSpeed (); }
	public void initState() { model.initBehavioralState (); }
	public void setWakeState(bool value) { model.isWakeState = value; }
}