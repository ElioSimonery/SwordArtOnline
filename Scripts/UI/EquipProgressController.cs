using UnityEngine;
using System.Collections;

public class EquipProgressController : MonoBehaviour {
	public enum MODE
	{
		Equip = 0,
		UnEquip,
		Discard
	}
	public GameObject labelObj;
	public GameObject progressObj;
	public GameObject percent_label;

	public Color equipColor;
	public Color unEquipColor;
	public Color discardColor;

	public AudioClip[] equipSound;
	public AudioClip unequipSound;
	public AudioClip trashSound;
	public AudioClip socketSound;

	protected MODE current;
	protected string currentSound;

	public readonly string[] EquipString = {"장착 시도", "Try Equip" };
	public readonly string[] UnEquipString = {"탈착 시도", "Try Unequip"};
	public readonly string[] DiscardString = {"버리는 중", "Try Discard"};

	public static EquipProgressController instance;
	void Awake()
	{
		instance = this;
		equipColor.a = 1f;
		unEquipColor.a = 1f;
		discardColor.a = 1f;
		
		SoundEffectManager.GetInstance ().register (unequipSound.name, unequipSound);
		SoundEffectManager.GetInstance ().register (trashSound.name, trashSound);
		SoundEffectManager.GetInstance ().register (socketSound.name, socketSound);
		
		foreach (AudioClip audio in equipSound)
			SoundEffectManager.GetInstance ().register (audio.name, audio);
	}
	void Start()
	{

	}
	
	public static EquipProgressController getInstance()
	{
		if (instance == null)
			Debug.LogWarning ("EquipProgressController hasn't been instantiated.");
		return instance;
	}

	public void setMode(MODE mode, WeaponBase.WeaponType type)
	{
		current = mode;
		switch(current)
		{
		case MODE.Equip:
			labelObj.GetComponent<UILabel>().text = 
				EquipString[Configuration.LanguageSelect];
			labelObj.GetComponent<UILabel>().color = equipColor;
			progressObj.GetComponent<UISprite>().color = equipColor;
			switch(type)
			{
			case WeaponBase.WeaponType.Gun:
				currentSound = equipSound[0].name;
				break;
			case WeaponBase.WeaponType.Sword:
				currentSound = equipSound[1].name;
				break;
			case WeaponBase.WeaponType.Dragon:
				currentSound = equipSound[2].name;
				break;
			default:
				currentSound = socketSound.name;
				break;
			}
			break;
		case MODE.UnEquip:
			labelObj.GetComponent<UILabel>().text = 
				UnEquipString[Configuration.LanguageSelect];
			labelObj.GetComponent<UILabel>().color = unEquipColor;
			progressObj.GetComponent<UISprite>().color = unEquipColor;
			currentSound = unequipSound.name;
			break;
		case MODE.Discard:
			labelObj.GetComponent<UILabel>().text = 
				DiscardString[Configuration.LanguageSelect];
			labelObj.GetComponent<UILabel>().color = discardColor;
			progressObj.GetComponent<UISprite>().color = discardColor;
			currentSound = trashSound.name;
			break;
		}

	}

	public void setProgress(float rate)
	{
		if (rate > 1 || rate < 0)
			return;

		int data = (int)(rate * 100);
		percent_label.GetComponent<UILabel>().text = data+"%";
		progressObj.GetComponent<UISprite> ().fillAmount = rate;
	}

	public void playSound()
	{
		SoundEffectManager.GetInstance().play(currentSound);
	}

	public void stopSound()
	{
		SoundEffectManager.GetInstance().stop(currentSound);
	}
}
