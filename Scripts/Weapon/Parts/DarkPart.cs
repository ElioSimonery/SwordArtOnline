using UnityEngine;
using System.Collections;

public class DarkPart: IPartItemBase {
	public GameObject auraObject;
	private const string AURA_OBJ = "DarkAura";
	private string m_type = PartType.Magic;
	private string m_name = "Dark Elemental";
	private int m_rank = 0;
	private string m_description = "";

	public override string Type { get { return m_type; } set { m_type = value; } }
	public override string Name { get{return m_name;} set{ m_name = value; } }
	public override int Rank { get{return m_rank;} set{m_rank = value; } }
	public override string Description { get{return m_description;} set{ m_description = value; } }
	
	private int chance;
	private float duration;
	private float lifeStealrate;

	class DarkPolicy : BulletBase.BulletTriggerCallback
	{
		public float lifeStealRate;
		public float duration;

		public DarkPolicy(float _lifeStealRate, float _duration)
		{
			lifeStealRate = _lifeStealRate;
			duration = _duration;
		}
		
		public override void onTriggerEnter (Collider monster)
		{
			float rate = lifeStealRate;
			rate /= 100f;
			SufferedState s = new DarkState(monster.gameObject, rate, duration); // animation duration.
			monster.gameObject.GetComponent<IMonsterBase>().pushSufferedState(s);
		}
		
		public override BulletBase.BulletTriggerCallback Create ()
		{
			DarkPolicy ret = new DarkPolicy (lifeStealRate, duration);
			return ret;
		}
	}
	
	public override void initData(ItemRank rank)
	{
		m_rank = (int)rank;
		chance = 10 + m_rank * 6;
		duration = 2 + m_rank * 0.5f;
		lifeStealrate = 10f + (float)m_rank * 3f;
		m_description = "Bullets have a chance of " + chance + "% to cast a curse of the " 
			+ lifeStealrate + "% of life stealing for " + duration +" seconds.";
		if(m_rank == (int)ItemRank.EPIC)
			m_name = "<BlackWidow>";
	}

	void Awake()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( 
		                 false, ItemPartsTable.DARK_PART, gameObject);
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
			DarkPolicy pol = new DarkPolicy(lifeStealrate, duration);
			bulletData.pushCallback(pol);
			GameObject aura = (GameObject)Instantiate(auraObject);
			aura.SetActive(true);
			aura.transform.parent = bulletObj.transform;
			aura.GetComponent<Light>().intensity = 1f+(float)m_rank*0.5f;
			aura.transform.localPosition = Vector3.zero;
		}
	}
}
