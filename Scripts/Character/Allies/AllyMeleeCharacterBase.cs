using UnityEngine;

public class AllyMeleeCharacterBase : MeleeCharacterBase
{
	public float teleportDistance = 10f;
	public float minimumTrackingDistance = 3f;
	public float maximumTrackingDistance = 7f;

	public float attackDistance = 4f;
	public SphereCollider wakeCollider;
	public float BEHAVIOR_SENSITIVITY = 0.5f;

	private float sqrAttackDistance;
	private bool monsterDiscovered;

	private behaviorAI behavior;
	private float behaviorDecision_timer;


	protected override void Start()
	{
		base.Start ();
		sqrAttackDistance = attackDistance * attackDistance;
		behavior = idleBehaviorImp;
	}


	void Update()
	{
		Vector3 toPlayer = GameManager.PlayerObject.transform.position - transform.position;

		behavior (toPlayer);
		behaviorDecision_timer += Time.deltaTime;

		if(behaviorDecision_timer >= BEHAVIOR_SENSITIVITY)
		{
			behavior = makeBehaviorDecision(toPlayer);
			behaviorDecision_timer = 0f;
		}
	}

	private behaviorAI makeBehaviorDecision(Vector3 toPlayer)
	{
		if(toPlayer.sqrMagnitude > teleportDistance*teleportDistance)
			return teleportBehaviorImp;

		if (toPlayer.sqrMagnitude > maximumTrackingDistance*maximumTrackingDistance) 
		{
			return moveToBehaviorImp;
		}
		else if(monsterDiscovered)
		{
			return attackMonsterBehaviorImp;
		}
		else
		{
			if(toPlayer.sqrMagnitude > minimumTrackingDistance*minimumTrackingDistance)
				return moveToBehaviorImp;
			else 
				return idleBehaviorImp;
		}
	}

	private delegate void behaviorAI(Vector3 v);
	void moveToBehaviorImp(Vector3 dir) { move (true, dir.normalized); }
	void idleBehaviorImp(Vector3 noParam) { move(false, Vector3.zero); }
	void teleportBehaviorImp(Vector3 noParam) 
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		Vector3 pos = GameManager.PlayerObject.transform.position;

		pos.x += 3f;
		transform.position = pos;
	}
	void attackMonsterBehaviorImp(Vector3 noParam)
	{
		Vector3 toMonster = Vector3.zero;
		if (targetMonster != null)
			toMonster = targetMonster.transform.position - transform.position;

		if (targetMonster == null || (wakeCollider.radius < toMonster.magnitude)) 
		{
			initializeTrigger();
			return;
		}

		if(toMonster.sqrMagnitude <= sqrAttackDistance)
		{
			shoot (toMonster); // attack monster if I can, or
			return;
		}
		else
		{
			move (true, toMonster.normalized); // go to monster
		}
	}
	
	public override void move(bool isMove, Vector3 v)
	{
		shoot (Vector3.zero); 
		base.move (isMove, v);
	}

	// Wake UP ally character.
	GameObject targetMonster;
	void OnTriggerEnter(Collider col) { OnTriggerStay (col); }
	void OnTriggerStay(Collider col)
	{
		if(col.tag == Common.TAG_MONSTER)
		{
			monsterDiscovered = true;
			targetMonster = col.gameObject;
			return;
		}
	}
	void OnTriggerExit(Collider col) { initializeTrigger (); }

	void initializeTrigger()
	{
		monsterDiscovered = false;
		targetMonster = null;
	}
}
