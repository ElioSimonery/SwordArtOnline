using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletBase : MonoBehaviour, IWeaponInfo {
	private int damage;
	private float speed;
	private float ttl;
	private float ttl_timer;
	public int bullet_type;
	public const int BULLET = 0;
	public const int MELEE = 1;
	public const int MAGIC = 2;

	public int Damage { get{return damage;} set{ damage = value; } }
	public float Speed{get{ return speed; } set{ speed = value; } }
	public float TTL{get{ return ttl; } set{ ttl = value; } }
	public float TTL_Timer{ get{ return ttl_timer; } set{ ttl_timer = value; } }

	public abstract class BulletTriggerCallback
	{
		public abstract void onTriggerEnter (Collider monster);
		public abstract BulletTriggerCallback Create ();
	}
	public List<BulletTriggerCallback> trigger_callbacks;

	public void init(int _damage, float _speed, float _ttl, int _bullet_type, List<BulletTriggerCallback> _target_trigger_callbacks)
	{
		set (_damage, _speed, _ttl, _bullet_type);
		copyCallback (_target_trigger_callbacks);
	}

	
	public void set(int _damage, float _speed, float _ttl, int _bullet_type)
	{
		damage = _damage;
		speed = _speed;
		ttl = _ttl;
		bullet_type = _bullet_type;
	}

	void Start()
	{
		if(trigger_callbacks == null)
			trigger_callbacks = new List<BulletTriggerCallback> ();
	}
	
	void Update()
	{
		ttl_timer += Time.deltaTime;
		
		if (ttl_timer >= ttl) 
		{
			DestroyBullet();
		}
	}

	protected void copyCallback(List<BulletTriggerCallback> src)
	{
		if (src == null)
			return;
		if(trigger_callbacks == null)
			trigger_callbacks = new List<BulletTriggerCallback> ();

		foreach(BulletTriggerCallback cb in src)
		{
			if(cb != null)
				trigger_callbacks.Add(cb.Create());
		}
	}

	public void pushCallback(BulletTriggerCallback tcb)
	{
		if (trigger_callbacks == null) //addcomponent start() function is lazy.
			trigger_callbacks = new List<BulletTriggerCallback> ();
		trigger_callbacks.Add (tcb);
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.tag == Common.TAG_OBJECT && !col.isTrigger)
		{
			DestroyBullet();
		}

		if (col.gameObject.tag == Common.TAG_MONSTER &&
		    !col.isTrigger) // trigger is wake collider.
		{
			IMonsterBase mon = col.gameObject.GetComponent<IMonsterBase>();
			mon.hit(damage);
			if(trigger_callbacks != null)
				foreach(BulletTriggerCallback callback in trigger_callbacks)
					callback.onTriggerEnter(col);

			DestroyBullet();
		}
	}

	private void DestroyBullet()
	{
		switch(bullet_type)
		{
		case BULLET:
			Destroy (gameObject);
			break;
		case MELEE:
			Destroy(gameObject.transform.parent.gameObject);
			break;
		default:
			Debug.LogError("[BulletBase] error");
			break;
		}
	}
}
