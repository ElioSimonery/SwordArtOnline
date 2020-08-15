using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//아이템 자동생성은 팩토리에서 구현해야 할 것.
public abstract class IPartItemBase : MonoBehaviour
{
	// Use this as a template param.
	public class PartType
	{
		public const string Relic = "Relic";		//유물
		public const string Mecha = "Mechanic";	//공학부품

		public const string Magic = "Magic";	// 마법
	}

	/* Enhanced option */
	public virtual int Damage { get; set; }
	public virtual float Speed{ get; set; }
	public virtual float FireRate { get; set; }
	public virtual float CriticalRate { get; set; }
	public virtual int CriticalDamage { get; set; }

	public abstract string Name {get;set;}
	public abstract int Rank { get; set; }
	public abstract string Description {get; set; }
	public abstract string Type { get; set;}
	public abstract void initData(ItemRank rank);
	public abstract void AddedPartsCallback 
		(GameObject character, Vector3 dir, GameObject bulletObj, BulletBase bulletData);
}
