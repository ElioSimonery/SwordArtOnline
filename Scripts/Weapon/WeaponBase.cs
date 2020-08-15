using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class WeaponBase : MonoBehaviour {
	public const int INSTALL_FALSE = -1;
	public string weaponName = "nothing weapon";
	public string description = "nothing Description";
	public WeaponType type = WeaponType.Gun;
	public enum WeaponType
	{
		Sword,
		Gun,
		Dragon,
		NULL
	};
	/* Use Only Unity Interface */
	public int baseDamage;
	/* End weapon data */

	public PartList relic_parts;
	public PartList magic_parts;
	public PartList mecha_parts;

	protected int rank;
	protected float fire_timer;


	public string Name {get {return weaponName;} set{weaponName = value;}}
	public string Description {get { return description; } }
	public int Rank { get {return rank;} set{rank = value;}}

	public abstract void onEquipApplyData(ICCharacterBase character);
	public abstract void unEquipApplyData(ICCharacterBase character);

	protected struct WeaponBullet
	{
		public GameObject obj;
		public BulletBase data;
	}
	protected WeaponBullet lastBullet;

	public struct PartList
	{
		public IPartItemBase[] list;
		public int max_size;

		public PartList(int sz)
		{
			max_size = Random.Range (0, sz + 1);
			if(max_size == 0) 
			{
				list = null;
				return;
			}
			list = new IPartItemBase[max_size];
		}

		public int pushPart(IPartItemBase item)
		{
			for(int i = 0; i < max_size; i++)
			{
				if(list[i] == null)
				{
					list[i] = item;
					return i;
				}
			}
			return INSTALL_FALSE;
		}

		public void popPartAt(int idx)
		{
			list [idx] = null;
		}

		public void recoveryCall()
		{
			for(int i = 0; i < max_size; i++)
				if(list[i] != null)
					list[i].gameObject.GetComponent<ItemCube>().recovery();
		}

		public void onInitItemCall(GameObject inventoryObject)
		{
			for(int i = 0; i < max_size; i++)
				if(list[i] != null)
					list[i].gameObject.GetComponent<ItemCube>().onInitItem(inventoryObject);
		}

		public int getCount()
		{
			int count = 0;
			if (list == null)
				return 0;

			foreach(IPartItemBase p in list)
				if(p!= null) count++;
			return count;
		}
	}

	public abstract void init();
	// init base data.
	public virtual void initData(ItemRank _rank)
	{
		rank = (int)_rank;
		relic_parts = new PartList (rank);
		magic_parts = new PartList (rank);
		mecha_parts = new PartList (rank);
	}

	public void onInitParts(GameObject inventoryObject)
	{
		relic_parts.onInitItemCall (inventoryObject);
		magic_parts.onInitItemCall (inventoryObject);
		mecha_parts.onInitItemCall (inventoryObject);
	}

	public void recoveryParts()
	{
		relic_parts.recoveryCall ();
		magic_parts.recoveryCall ();
		mecha_parts.recoveryCall ();
	}

	public virtual bool fire(GameObject character, Vector3 dir)
	{
		if(mecha_parts.max_size != 0)
		foreach (IPartItemBase p in mecha_parts.list) 
		{
			if(p != null) // part have inserted
			p.AddedPartsCallback(character,dir, lastBullet.obj, lastBullet.data);
		}
		if(magic_parts.max_size != 0)
		foreach (IPartItemBase p in magic_parts.list) 
		{
			if(p != null) // part have inserted
			p.AddedPartsCallback(character,dir, lastBullet.obj, lastBullet.data);
		}
		if(relic_parts.max_size != 0)
		foreach (IPartItemBase p in relic_parts.list) 
		{
			if(p != null) // part have inserted
			p.AddedPartsCallback(character,dir, lastBullet.obj, lastBullet.data);
		}
		return true;
	}

	public virtual int installPart(IPartItemBase newPart)
	{
		int ret = INSTALL_FALSE;
		switch (newPart.Type) 
		{
		case IPartItemBase.PartType.Relic:
			ret = relic_parts.pushPart(newPart);
			break;
		case IPartItemBase.PartType.Magic://magic
			ret = magic_parts.pushPart(newPart);
			break;
		case IPartItemBase.PartType.Mecha:
			ret = mecha_parts.pushPart(newPart);
			break;
		}
		return ret;
	}

	public virtual void uninstallPart(IPartItemBase rmvPart, int idx)
	{
		switch (rmvPart.Type) 
		{
		case IPartItemBase.PartType.Relic:
			relic_parts.popPartAt(idx);
			break;
		case IPartItemBase.PartType.Magic://magic
			relic_parts.popPartAt(idx);
			break;
		case IPartItemBase.PartType.Mecha:
			mecha_parts.popPartAt(idx);
			break;
		}
	}

	public bool isInstalledPart(IPartItemBase part)
	{
		for(int i = 0; i < relic_parts.max_size; i++)
			if(relic_parts.list[i] == part) return true;
		for(int i = 0; i < magic_parts.max_size; i++)
			if(magic_parts.list[i] == part) return true;
		for(int i = 0; i < mecha_parts.max_size; i++)
			if(mecha_parts.list[i] == part) return true;
		return false;
	}

}
