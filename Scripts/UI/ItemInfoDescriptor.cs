using UnityEngine;
using System.Collections;

public class ItemInfoDescriptor : MonoBehaviour {

	public GameObject nameObj;
	public Color NormalItemNameColor;
	public Color MagicItemNameColor;
	public Color RareItemNameColor;
	public Color EpicItemNameColor;

	public GameObject rankObj;
	public GameObject optionObj;
	public Color RelicPartsColor;
	public Color MagicPartsColor;
	public Color MechaPartsColor;

	public GameObject descObj;
	public Color DescriptionColor;

	protected static Color GoldColor = Color.yellow;
	// rank option
	private const string RANK = "Rank : ";
	private const string RANK_NORMAL = "normal";
	private const string RANK_MAGIC = "magic";
	private const string RANK_RARE = "rare";
	private const string RANK_EPIC = "epic";

	// parts option
	private const string RELIC = "(Relic)";
	private const string MAGIC = "(Magic)";
	private const string MECHA = "(Mechanic)";

	// base option
	private const string DMG = "\nDamage : ";
	private const string FIRE_RATE = "\nFire rate : ";
	private const string DMG_CRT = "\nEnhanced Critical Dmg : ";
	private const string CRT = "\nEnhanced Critical rate : ";
	private const string WSPD = "\nWeapon Speed : ";

	private const string GOLD = " GOLD"; 
	void Awake()
	{
	}
	// Use this for initialization
	void Start () {
		init ();
	}
	void init()
	{

		NormalItemNameColor.a = 1.0f;
		MagicItemNameColor.a = 1.0f;
		RareItemNameColor.a = 1.0f;
		EpicItemNameColor.a = 1.0f;
		
		RelicPartsColor.a = 1.0f;
		MagicPartsColor.a = 1.0f;
		MechaPartsColor.a = 1.0f;
		
		DescriptionColor.a = 1.0f;
		gameObject.SetActive (false);
	}

	public void displayItem(GameObject obj) //any sprite obj
	{
		gameObject.SetActive (true); // 
		GameObject itemCubeObj = obj.GetComponentInParent<SecondParent> ().virtualParent;
		IPartItemBase part = itemCubeObj.GetComponent<ItemCube>().part;
		WeaponBase weapon = itemCubeObj.GetComponent<ItemCube>().weapon;

		/** Common item data. **/
		UILabel label_name = nameObj.GetComponent<UILabel> ();

		string target_name = "void";
		int target_rank = 0;
		int target_damage = 0;
		float target_fire_rate = 0;
		float target_critical_rate = 0;
		int target_critical_damage = 0;
		float target_speed = 0;
		string target_description = "void";

		if(part != null)
		{
			target_name = part.Name;
			target_rank = part.Rank;
			target_damage = part.Damage;
			target_fire_rate = part.FireRate;
			target_critical_rate = part.CriticalRate;
			target_critical_damage = part.CriticalDamage;
			target_speed = part.Speed;
			target_description = part.Description;
		}
		else if(weapon != null)
		{
			target_name = weapon.Name;
			target_rank = weapon.Rank;
			target_damage = weapon.baseDamage;
			if(weapon is RangedWeaponBase)
			{
				RangedWeaponBase w = (RangedWeaponBase)weapon;
				target_fire_rate = w.baseFireRate;
				target_speed = w.baseBulletSpeed;
			}
			target_description = weapon.Description;		
		}
		// 1. Name & Rank
		label_name.text = target_name;
		string buffer = RANK;

		switch(target_rank)
		{
		case (int)ItemRank.NORMAL:
			buffer += RANK_NORMAL;
			label_name.color = NormalItemNameColor;
			break;
		case (int)ItemRank.MAGIC:
			buffer += RANK_MAGIC;
			label_name.color = MagicItemNameColor;
			break;
		case (int)ItemRank.RARE:
			buffer += RANK_RARE;
			label_name.color = RareItemNameColor;
			break;
		case (int)ItemRank.EPIC:
			buffer += RANK_EPIC;
			label_name.color = EpicItemNameColor;
			break;
		}
		if(weapon != null)
		{
			if(weapon.relic_parts.max_size != 0)
				buffer += " ("+weapon.relic_parts.getCount()+"/"+weapon.relic_parts.max_size+") ";
			else buffer += " _ ";

			if(weapon.magic_parts.max_size != 0)
				buffer += "("+weapon.magic_parts.getCount()+"/"+weapon.magic_parts.max_size+") ";
			else buffer += "_ ";

			if(weapon.mecha_parts.max_size != 0)
				buffer += weapon.mecha_parts.getCount()+"/"+weapon.mecha_parts.max_size+")";
			else buffer += "_";
		}

		rankObj.GetComponent<UILabel>().text = buffer;
		rankObj.GetComponent<UILabel>().color = label_name.color;
		buffer = "";

		// 2. Common Item Options
		UILabel label_option = optionObj.GetComponent<UILabel>();

		// if parts, then push Parts Type first.
		if(part != null)
		{
			switch(part.Type)
			{
			case IPartItemBase.PartType.Relic:
				buffer = RELIC;
				label_option.color = RelicPartsColor;
				break;
			case IPartItemBase.PartType.Magic:
				buffer = MAGIC;
				label_option.color = MagicPartsColor;
				break;
			case IPartItemBase.PartType.Mecha:
				buffer = MECHA;
				label_option.color = MechaPartsColor;
				break;
			}
		}
		else if(weapon != null)
		{
			label_option.color = label_name.GetComponent<UILabel>().color;
		}

		if(target_damage != 0)
		{
			buffer += DMG;
			buffer += target_damage;
		}
		if(target_fire_rate != 0)
		{
			buffer += FIRE_RATE;
			buffer += target_fire_rate;
		}
		if(target_critical_damage != 0)
		{
			buffer += DMG_CRT;
			buffer += target_critical_damage;
		}
		if(target_critical_rate != 0)
		{
			buffer += CRT;
			buffer += target_critical_rate;
		}
		if(target_speed != 0)
		{
			buffer += WSPD;
			buffer += target_speed;
		}
		label_option.text = buffer;


		// 3. Item Description
		descObj.GetComponent<UILabel>().text = target_description;
		descObj.GetComponent<UILabel> ().color = DescriptionColor;

	}
}
