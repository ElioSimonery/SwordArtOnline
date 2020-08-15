using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterModel
{
	public enum MonsterType
	{
		Normal = 0,
		Rare,
		Boss
	}
	public MonsterType type = MonsterType.Normal;

	public int MAX_HP = 50;
	public int MAX_MP = 50;
	protected int hp;
	protected int mp;
	public bool isDead = false;

	protected List<SufferedState> suffState;
	// Attack Policies
	public int attack_damage = 10;
	public float attack_range = 3.0f;
	public float attack_cooltime = 1.0f;
	public float attack_postDelay = 0.3f;
	// Idletime Prowling Policies
	public bool canProwling = true;
	public float prowlingInverval = 5.0f;
	public float prowlingDeviation = 2.0f;
	public float prowlingDuration = 0.5f; 
	// Speed
	public float move_speed = 1.0f;
	// Item Drop Rate
	public float weapon_drop_rate = 1.0f;
	public float part_drop_rate = 1.0f;
	
	// Internal data, but it can be accessed from states.
	public bool isWakeState = false;
	public bool isAttackState = false;
	public bool isProwling = false;
	public bool hasPostDelay = false;

	protected float attack_timer = 0.0f;
	protected float postDelay_timer = 0.0f;
	protected float prowling_timer = 0.0f;
	protected float prowlEnd_timer = 0.0f;

	protected float prowlRndDev = 0f;
	protected Vector3 prowlRndDir;

	public MonsterModel(int iHP, int iMP, 
	                    int iAttackDamage, float iAttackRange, float iAttackCooltime, float iAttackPostDelay,
	                    bool iCanProwling, float iProwlingInterval, float iProwlingDeviation, float iProwlingDuration,
	                    float iMoveSpeed, float iWeaponDropRate, float iPartDropRate)
	{
		hp = MAX_HP = iHP;
		mp = MAX_MP = iMP;
		// Attack Policies
		attack_damage = iAttackDamage;
		attack_range = iAttackRange;
		attack_cooltime = iAttackCooltime;
		attack_postDelay = iAttackPostDelay;
		// Idletime Prowling Policies
		canProwling = iCanProwling;
		prowlingInverval = iProwlingInterval;
		prowlingDeviation = iProwlingDeviation;
		prowlingDuration = iProwlingDuration;
		// Speed
		move_speed = iMoveSpeed;
		// Item Drop Rate
		weapon_drop_rate = iWeaponDropRate;
		part_drop_rate = iPartDropRate;

		suffState = new List<SufferedState> ();
	}

	public virtual void UpdateData(float duration)
	{
		attack_timer += duration; // attack timer doesn't have to be checked any condition.
		if(hasPostDelay)
		{
			postDelay_timer += duration;
			if(postDelay_timer > attack_postDelay)
			{
				postDelay_timer = 0f;
				hasPostDelay = false;
			}
		}
	}

	public void pushSufferedState(SufferedState s)
	{
		suffState.Add (s);
	}

	// Function returns
	// true if attacked
	// else false
	public virtual bool attack()
	{
		if (isWakeState && isAttackState) 
		{
			if(attack_cooltime <= attack_timer)
			{
				hasPostDelay = true;
				attack_timer = 0f;
				return true;
			}
		}
		
		return false;
	}

	// Function returns
	// prowl Vector3 if prowling
	// else Vector3.zero
	public virtual Vector3 prowling(float duration)
	{
		if (!canProwling)
			return Vector3.zero;
		if(!isWakeState && !isAttackState) // Idling -> prowling start.
		{
			if(isProwling) //now prowling.
			{
				prowlEnd_timer += duration;
				if(prowlEnd_timer > prowlingDuration)
				{
					isProwling = false;
					prowlEnd_timer = 0f;
				}
			}
			else // monster doesn't do anything
				prowling_timer += duration;
			
			if(prowling_timer > prowlingInverval+prowlRndDev)
			{
				prowling_timer = 0f;
				prowlRndDev = Random.Range(-prowlingDeviation, prowlingDeviation+1);
				prowlRndDir = new Vector3((float)Random.Range(-10,10),0f,(float)Random.Range(-10,10));
				prowlRndDir.Normalize();
				Vector3 v = move_speed*prowlRndDir;
				isProwling = true;
				return v;
			}
		}
		return Vector3.zero;
	}

	// Function returns
	// true when live
	// else false
	public virtual bool getDamage(int damage)
	{
		if (hp - damage > 0)
		{
			hp -= damage;
			return true;
		}
		else 
		{
			hp = 0;
			isDead = true;

			foreach (SufferedState s in suffState)
				s.FinishOnDie();
		}
		return false;
	}
	
	public virtual bool updateAttackState(Vector3 myPos)
	{
		return updateStateByDistance (out isAttackState, ref attack_range, ref myPos);
	}
	
	public void initBehavioralState()
	{
		isWakeState = false;
		isAttackState = false;
	}

	public virtual void UpdateSufferedState()
	{
		foreach(SufferedState s in suffState)
		{
			s.OnSufferedStart();
			s.SufferingUpdate();
			s.OnSufferedStop();
		}
		suffState.RemoveAll(s => s.isFinished());
	}

	public void upgrade(float rate) 
	{
		upgradeMaxHP (rate);
		upgradeAttackDamage (rate);
		upgradeMoveSpeed (rate);
	}

	protected void upgradeMaxHP(float rate) { hp = MAX_HP = (int)(MAX_HP * rate); }
	protected void upgradeAttackDamage(float rate){attack_damage = (int)(attack_damage * rate); }
	protected void upgradeMoveSpeed(float rate) { move_speed *= rate; }

	public MonsterType getMonsterType() { return type; }
	public void setHP(int _hp) { hp = _hp; }
	public int getMaxHP() { return MAX_HP; }
	public int getHP() { return hp; }
	public int getAttackDamage() { return attack_damage; }
	public float getAttackRange() { return attack_range; }
	public float getMoveSpeed() { return move_speed; }
	public void setMoveSpeed(float value) { move_speed = value; }
	protected static bool updateStateByDistance(out bool state, ref float cond_distance, ref Vector3 pos)
	{
		float distance = Vector3.Distance(GameManager.PlayerObject.transform.position, pos);
		if (distance <= cond_distance) 
			state = true;
		else
			state = false;
		
		return state;
	}
}
