using UnityEngine;
using System.Collections;

public class FirePart: IPartItemBase {
	public GameObject auraObject;
	private const string AURA_OBJ = "FireAura";
	private string m_type = PartType.Magic;
	private string m_name = "Fire Elemental";
	private int m_rank = 0;
	private string m_description = "";
	
	public override string Type { get { return m_type; } set { m_type = value; } }
	public override string Name { get{return m_name;} set{ m_name = value; } }
	public override int Rank { get{return m_rank;} set{m_rank = value; } }
	public override string Description { get{return m_description;} set{ m_description = value; } }
	
	private int chance;
	private int fireDamage;
	private int duration;
	
	class FirePolicy : BulletBase.BulletTriggerCallback
	{
		public int fireDamage;
		public int duration;
		public FirePolicy(int _fireDamage, int _duration)
		{
			fireDamage = _fireDamage;
			duration = _duration;
		}
		
		public override void onTriggerEnter (Collider monster)
		{
			float rate = (float)fireDamage;
			rate /= 100f;
			SufferedState s = new FireState(monster.gameObject, rate, duration);
			monster.gameObject.GetComponent<IMonsterBase>().pushSufferedState(s);
		}

		public override BulletBase.BulletTriggerCallback Create ()
		{
			FirePolicy ret = new FirePolicy (fireDamage, duration);
			return ret;
		}
	}
	
	public override void initData(ItemRank rank)
	{
		m_rank = (int)rank;
		chance = 10 + m_rank * 6;
		fireDamage = 1 + m_rank;
		duration = 2 + m_rank * 1;
		m_description = "Bullets have a chance of " 
			+ chance + "% to burning damage of "
				+ fireDamage + "% maximum health for "
				+ duration + " seconds.";
		if(m_rank == (int)ItemRank.EPIC)
			m_name = "<HOT6>";
	}
	
	
	void Awake()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( 
		                  false, ItemPartsTable.FIRE_PART, gameObject);
		if(result)
			gameObject.SetActive (false);
		else
			GameManager.garbage.push (gameObject);
		
	}
	
	public override void AddedPartsCallback (GameObject character, Vector3 dir, GameObject bulletObj, BulletBase bulletData)
	{
		// add Effect to bulletObj
		int seed = Random.Range (0, 100);
		if(chance > seed)
		{
			if(auraObject == null)
				auraObject = GameObject.Find(AURA_OBJ);
			FirePolicy pol = new FirePolicy(fireDamage, duration);
			bulletData.pushCallback(pol);
			GameObject aura = (GameObject)Instantiate(auraObject);
			aura.SetActive(true);
			aura.transform.parent = bulletObj.transform;
			aura.GetComponent<Light>().intensity = 1f+(float)m_rank*0.5f;
			aura.transform.localPosition = Vector3.zero;
		}
	}
}
