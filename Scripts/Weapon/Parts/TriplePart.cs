using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriplePart : IPartItemBase {
	private string m_type = PartType.Relic;
	private string m_name = "Triple Bullet";
	private int m_rank = 0;
	private string m_description = "";

	public override string Type { get { return m_type; } set { m_type = value; } }
	public override string Name { get{return m_name;} set{ m_name = value; } }
	public override int Rank { get{return m_rank;} set{m_rank = value; } }
	public override string Description { get{return m_description;} set{ m_description = value; } }

	public override void initData(ItemRank rank)
	{
		m_rank = (int)rank;
		m_description = "Fires additional ("+(int)m_rank*2+") small bullets spreading out per hit.";
		if(m_rank == (int)ItemRank.EPIC)
			m_name = "<Centipede Legs>";
	}

	void Awake()
	{
		// regist this object to a factory.
		bool result = GameManager.World.GetComponent<ItemFactory> ().registItem ( 
		                    false, ItemPartsTable.TRIPLE_PART, gameObject);
		if(result)
			gameObject.SetActive (false);
		else
			GameManager.garbage.push (gameObject);
	}
	
	public override void AddedPartsCallback (GameObject character, Vector3 dir, GameObject bulletObj, BulletBase bulletData)
	{

		for(int i = 0 ; i < (int)Rank*2; i++)
		{
			float mark = (i%2 == 0) ? 1.0f:-1.0f;
			GameObject new_bullet = GameManager.CopyObjects (bulletObj);

			new_bullet.transform.localScale = new Vector3(new_bullet.transform.localScale.x / 1.5f,
			                                              new_bullet.transform.localScale.y / 1.5f,
			                                              new_bullet.transform.localScale.z / 1.5f);
			new_bullet.GetComponentInChildren<BulletBase>().init(bulletData.Damage/2, bulletData.Speed, bulletData.TTL, bulletData.bullet_type,
			                                           bulletData.trigger_callbacks);

			Vector3 startPos = character.transform.position;
			startPos.y += character.transform.lossyScale.y;

			Vector3 frontPos = bulletObj.transform.position - character.transform.position;
			new_bullet.transform.position = startPos + BabelUtils.RotateXZ (frontPos, mark*((i/2)+1) * BabelUtils.PI / 12.0f);
			new_bullet.GetComponent<Rigidbody>().velocity = BabelUtils.RotateXZ (dir.normalized, mark*((i/2)+1) * BabelUtils.PI / 12.0f) * bulletData.Speed;
		}

	}
}
