using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuidePart : IPartItemBase {
	private string m_type = PartType.Relic;
	private const string TRK_OBJ = "GuideTracker";
	private string m_name = "Guide Bullet";
	private int m_rank = 0;
	private string m_description = "";
	public GameObject trackerObj;
	public override string Type { get { return m_type; } set { m_type = value; } }
	public override string Name { get{return m_name;} set{ m_name = value; } }
	public override int Rank { get{return m_rank;} set{m_rank = value; } }
	public override string Description { get{return m_description;} set{ m_description = value; } }
	
	public override void initData(ItemRank rank)
	{
		m_rank = (int)rank;
		m_description = "Bullet tracks ("+m_rank+") times its near monsters in the radius of (" + m_rank+1 + ") cell.";
		if(m_rank == (int)ItemRank.EPIC)
			m_name = "<STALKER>";
	}
	
	void Awake()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( 
		                                                                         false, ItemPartsTable.GUIDE_PART, gameObject);
		if(result)
			gameObject.SetActive (false);
		else
			GameManager.garbage.push (gameObject);
	}
	
	public override void AddedPartsCallback (GameObject character, Vector3 dir, GameObject bulletObj, BulletBase bulletData)
	{
		if (trackerObj == null)
			trackerObj = GameObject.Find (TRK_OBJ);
		GameObject tracker = (GameObject)Instantiate (trackerObj);
		tracker.SetActive (true);
		tracker.transform.parent = bulletObj.transform;
		tracker.GetComponent<GuideBulletTracker> ().max_count = Rank + 1;
		tracker.GetComponent<SphereCollider> ().radius = Rank + 1;
		tracker.transform.localPosition = Vector3.zero;
		//bulletObj
	}
}
