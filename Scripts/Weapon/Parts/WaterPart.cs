using UnityEngine;
using System.Collections;

public class WaterPart : IPartItemBase {
	public GameObject auraObject;
	private const string AURA_OBJ = "WaterAura";
	private string m_type = PartType.Magic;
	private string m_name = "Water Elemental";
	private int m_rank = 0;
	private string m_description = "";
	
	public override string Type { get { return m_type; } set { m_type = value; } }
	public override string Name { get{return m_name;} set{ m_name = value; } }
	public override int Rank { get{return m_rank;} set{m_rank = value; } }
	public override string Description { get{return m_description;} set{ m_description = value; } }

	private int chance;
	private int slow;
	private int duration;

	class WaterPolicy : BulletBase.BulletTriggerCallback
	{
		public int slow;
		public int duration;
		public WaterPolicy(int _slow, int _duration)
		{
			slow = _slow;
			duration = _duration;
		}

		public override void onTriggerEnter (Collider monster)
		{
			SufferedState s = new ColdState(monster.gameObject, (float)slow/100f, duration);

			monster.gameObject.GetComponent<IMonsterBase>().pushSufferedState(s);
		}

		public override BulletBase.BulletTriggerCallback Create ()
		{
			WaterPolicy ret = new WaterPolicy (slow, duration);
			return ret;
		}
	}

	public override void initData(ItemRank rank)
	{
		m_rank = (int)rank;
		chance = 10 + m_rank * 6;
		slow = 10 + m_rank * 10;
		duration = 2 + m_rank * 1;
		m_description = "Bullets have a chance of " 
			+ chance + "% to slow the monster's speed by "
				+ slow + "% for "
				+ duration + " seconds.";
		if(m_rank == (int)ItemRank.EPIC)
			m_name = "<Poseidon>";
	}


	void Awake()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( 
		              				false, ItemPartsTable.WATER_PART, gameObject);
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
			WaterPolicy pol = new WaterPolicy(slow, duration);
			bulletData.pushCallback(pol);
			GameObject aura = (GameObject)Instantiate(auraObject);
			aura.SetActive(true);
			aura.transform.parent = bulletObj.transform;
			aura.GetComponent<Light>().intensity = 1f+(float)m_rank*0.5f;
			aura.transform.localPosition = Vector3.zero;
		}
	}
}
