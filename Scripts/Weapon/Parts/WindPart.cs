using UnityEngine;
using System.Collections;

public class WindPart: IPartItemBase {
	public GameObject auraObject;
	private const string AURA_OBJ = "WindAura";
	private string m_type = PartType.Magic;
	private string m_name = "Wind Elemental";
	private int m_rank = 0;
	private string m_description = "";
	
	public override string Type { get { return m_type; } set { m_type = value; } }
	public override string Name { get{return m_name;} set{ m_name = value; } }
	public override int Rank { get{return m_rank;} set{m_rank = value; } }
	public override string Description { get{return m_description;} set{ m_description = value; } }
	
	private int chance;
	private int windDamage;
	private float duration;
	
	class WindPolicy : BulletBase.BulletTriggerCallback
	{
		public int windDamage;
		public float duration;
		public WindPolicy(int _windDamage, float _duration)
		{
			windDamage = _windDamage;
			duration = _duration;
		}
		
		public override void onTriggerEnter (Collider monster)
		{
			float rate = (float)windDamage;
			//rate /= 100f;
			SufferedState s = new WindState(monster.gameObject, rate, duration);
			monster.gameObject.GetComponent<IMonsterBase>().pushSufferedState(s);
		}

		public override BulletBase.BulletTriggerCallback Create ()
		{
			WindPolicy ret = new WindPolicy (windDamage, duration);
			return ret;
		}
	}
	
	public override void initData(ItemRank rank)
	{
		m_rank = (int)rank;
		chance = 10 + m_rank * 6;
		windDamage = 2 + (int)(m_rank * 0.5f);
		duration = 0.5f;
		m_description = "Bullets have a chance of " 
			+ chance + "% to knockback monster by " + windDamage + " with 0.5 second stun.";
		if(m_rank == (int)ItemRank.EPIC)
			m_name = "<Breeze>";
	}

	void Awake()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( 
		                  false, ItemPartsTable.WIND_PART, gameObject);
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
			WindPolicy pol = new WindPolicy(windDamage, duration);
			bulletData.pushCallback(pol);
			GameObject aura = (GameObject)Instantiate(auraObject);
			aura.SetActive(true);
			aura.transform.parent = bulletObj.transform;
			aura.GetComponent<Light>().intensity = 1f+(float)m_rank*0.5f;
			aura.transform.localPosition = Vector3.zero;
		}
	}
}
