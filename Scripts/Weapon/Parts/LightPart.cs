using UnityEngine;
using System.Collections;

public class LightPart: IPartItemBase {
	public GameObject auraObject;
	private const string AURA_OBJ = "HolyAura";
	private string m_type = PartType.Magic;
	private string m_name = "Light Elemental";
	private int m_rank = 0;
	private string m_description = "";
	
	public override string Type { get { return m_type; } set { m_type = value; } }
	public override string Name { get{return m_name;} set{ m_name = value; } }
	public override int Rank { get{return m_rank;} set{m_rank = value; } }
	public override string Description { get{return m_description;} set{ m_description = value; } }
	
	private int chance;
	private int holyDamage;
	private const float duration = 0.3f;
	
	class HolyPolicy : BulletBase.BulletTriggerCallback
	{
		public int holyDamage;
		public const float duration = 0.3f;
		public HolyPolicy(int _holyDamage)
		{
			holyDamage = _holyDamage;
		}
		
		public override void onTriggerEnter (Collider monster)
		{
			SufferedState s = new HolyState(monster.gameObject, (float)holyDamage, duration);
			monster.gameObject.GetComponent<IMonsterBase>().pushSufferedState(s);
		}
		
		public override BulletBase.BulletTriggerCallback Create ()
		{
			HolyPolicy ret = new HolyPolicy (holyDamage);
			return ret;
		}
	}
	
	public override void initData(ItemRank rank)
	{
		m_rank = (int)rank;
		chance = 10 + m_rank * 6;
		holyDamage = 10 + m_rank*5;
		m_description = "Bullets have a chance of " 
			+ chance + "% to get additional holy damage increased by "
				+ holyDamage + "%";
		if(m_rank == (int)ItemRank.EPIC)
			m_name = "<Heaven>";
	}

	void Awake()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( 
		       false, ItemPartsTable.HOLY_PART, gameObject);
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
			HolyPolicy pol = new HolyPolicy(holyDamage);
			bulletData.pushCallback(pol);
			GameObject aura = (GameObject)Instantiate(auraObject);
			aura.SetActive(true);
			aura.transform.parent = bulletObj.transform;
			aura.GetComponent<Light>().intensity = 1f+(float)m_rank*0.5f;
			aura.transform.localPosition = Vector3.zero;
		}
	}
}
