using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialDungeon : DungeonBase {
	public GameObject pcController;
	public GameObject characterLocationObj;
	public GameObject controllerObj;
	public GameObject shooterObj;
	public GameObject hudObj;

	private float timer;
	private List<MissionBase> missions;
	private int currentMission;

	protected override void Start()
	{
		init ();
	}

	public override void init()
	{

		controllerObj.SetActive (false);
		shooterObj.SetActive (false);
		hudObj.SetActive (false);

		if(Application.platform == RuntimePlatform.WindowsEditor ||
		   Application.platform == RuntimePlatform.WindowsPlayer)
		{
			controllerObj = null;
			shooterObj = null;
			Debug.LogWarning("This is Tutorial stage in PC version. So tutorial controller will be disappeared.");
		}

		missions = new List<MissionBase> ();
		missions.Add (new ControlMission (controllerObj));
		missions.Add (new OpenInventoryMission (hudObj));
		missions.Add (new EquipMission ());
		missions.Add (new HuntMission (this, shooterObj));

		GameManager.missionCleared = false;
		NotificationManager.GetInstance ().toast ("Sword Art Online ~ Babel of dungeon ~");

		SpawnAt (GameManager.PlayerObject, characterLocationObj.transform.position);
	}

	/* Tutorial Missions */
	class ControlMission : MissionBase
	{
		private readonly string[,] Name = {
			{"개발자", "Developer"},
			{}
		};
		private readonly string[,] Desc = {
		{
			"안녕하세요. \n바벨오브던전에 오신 것을 환영합니다.\n게임을 시작하기에 앞서 간단한 튜토리얼을 진행해보고자 합니다.",
			"Thank you for playing the Babel of dungeon. I'm \nBefore starting a game, \nWe want to proceed brief tutorial."
		},
		{
			"먼저,\n좌측 하단에 조이스틱을 움직여 보도록 합시다.\n그럼 당신의 케릭터를 움직일 수 있을 거에요.",
			"First of all,\nlet's move the joystick on the bottom left now.\n then you'll be able to move your character."
		},
		{
			"잘하셨습니다!\n그럼 이번엔 근처 바닥에 드랍된 무기를 주워볼게요.\n아이템에 가까이 가면 획득할 수 있어요.",
			"Well done!\nAnd let's pick up a dropped weapon on the floor.\nYou can go close to the item to be acquired."
		}
		};

		private GameObject controllerObj;
		private Vector3 pos;
		private bool dropped = false;

		public ControlMission(GameObject controller) { startTime = 2f; controllerObj = controller; }

		public override void start()
		{
			base.start ();
			if(controllerObj != null)
				controllerObj.SetActive(true);
			SpeechController.GetInstance ().skipAvailable (false);
			SpeechController.GetInstance ().pushPrint(
				Name[0,Configuration.LanguageSelect],
				Desc[0,Configuration.LanguageSelect], 
				(Resources.Load("portraits/DeveloperPortraits") as GameObject).GetComponent<UIAtlas>(),
				"1");
			                    
			SpeechController.GetInstance ().pushPrint (null,
			    Desc [1, Configuration.LanguageSelect]);

			pos = GameManager.PlayerObject.transform.position;
		}
		private GameObject weapon;
		public override bool checkMission()
		{
			if(dropped)
			{
				if(!weapon.activeSelf)
					return true;
			}
			else if((pos - GameManager.PlayerObject.transform.position).sqrMagnitude > 10f)
			{
				SpeechController.GetInstance ().pushPrint (null,
					Desc [2, Configuration.LanguageSelect]);
				// 무기 아이템 지급
				pos.y += 1f;
				switch(GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.iCharacterClass)
				{
				case CharacterModel.MyClass.Gunner:
					weapon = GameManager.World.GetComponent<ItemFactory>().GetSingleRandomWeapon(1.0f, ItemWeaponsTable.BRANCH_GUN, pos); // drop item.
					break;
				case CharacterModel.MyClass.Warrior:
					weapon = GameManager.World.GetComponent<ItemFactory>().GetSingleRandomWeapon(1.0f, ItemWeaponsTable.SHORT_SWORD, pos); // drop item.
					break;
				case CharacterModel.MyClass.Magician:
					weapon = null;
					break;
				default:
					break;
				}

				weapon.GetComponent<ItemCube>().weapon.initData(ItemRank.NORMAL);
				dropped = true;
			}

			return false;
		}
	}

	class OpenInventoryMission : MissionBase
	{
		private readonly string[,] Desc = {
			{
				"잘하셨습니다! \n획득한 아이템은 인벤토리 가방에 저장됩니다. \n당신이 죽기 전까지는 가방에 계속 남아있을 거에요.",
				"Well done!\nThe items acquired is stored in the inventory bag.\n It will be in the bag until you die."
			},
			{
				"이번엔, 좌측 하단의 가방 아이콘을 눌러서 인벤토리를 열어보세요.",
				"This time,\n try opening your inventory now by pressing the icon at the bottom left of the bag."
			}
		};
		private GameObject hudObj;
		public OpenInventoryMission(GameObject hud) { startTime = 1f; hudObj = hud; }
		public override void start()
		{
			base.start ();
			SpeechController.GetInstance ().skipAvailable (false);
			SpeechController.GetInstance ().pushPrint (null,
			                                           Desc [0, Configuration.LanguageSelect]);
			SpeechController.GetInstance ().pushPrint (null,
			                                           Desc [1, Configuration.LanguageSelect]);
			hudObj.SetActive (true);
		}
		
		public override bool checkMission()
		{
			return GameManager.inventoryObject.activeInHierarchy;
		}
	}

	class EquipMission : MissionBase
	{
		private readonly string[,] Desc = {
			{
				"아이템을 터치하면 옵션을 확인할 수 있습니다.\n그리고 약 1초 동안 터치를 대기하면 장비를 착용 합니다. \n좌측 아래의 버튼을 눌러 버리기 모드로 전환할 수도 있습니다.",
				"When you touch an item, you can see the options. \nAnd if you wait for about 1 second at the state, wear the equipment. \nIf you press the button below, you can switch to discard mode."
			},
			{
				"인벤토리는 가방 아이콘을 다시 터치하거나 X버튼을 누르면 닫을 수 있습니다. \n다음엔 사냥을 알려드\t릴테니 무기를 착용하고 인벤토리를 닫으세요.",
				"Now wear and close the inventory. \nTouch the bag icon or press the X button again to close. \nNext time I'll tell you hunt."
			}
		};
		public EquipMission() { startTime = 1f; }
		public override void start()
		{
			base.start ();
			SpeechController.GetInstance ().skipAvailable (false);
			SpeechController.GetInstance ().pushPrint (null,
			                                           Desc [0, Configuration.LanguageSelect]);
			SpeechController.GetInstance ().pushPrint (null,
			                                           Desc [1, Configuration.LanguageSelect]);
		}
		
		public override bool checkMission()
		{
			if(LabelChange.getInstance().mode == LabelChange.DISCARD)
			{
				LabelChange.getInstance().setMode(LabelChange.EQUIP);
				NotificationManager.GetInstance().toast("아직 버릴 수 없어요. 다시는 시도하지 마세요.");
			}

			return ((!GameManager.inventoryObject.activeInHierarchy) && 
			        (GameManager.PlayerObject.GetComponent<CharacterControlHelper>().c.getCurrentWeapon() != null));
		}
	}

	class HuntMission : MissionBase
	{
		private int spawn = 7;
		private GameObject shooterObj;
		private List<GameObject> monList;
		private TutorialDungeon td;
		private readonly string[,] Desc = {
			{
				"우측의 조이스틱으로는 공격을 할 수 있습니다. \n7 마리의 몬스터를 소환해드렸으니, \n5 마리 이상 처치해보세요.",
				"You can attack through the joystick to the right. \nI summoned the five monster. Try aid."
			},
			{
				"사살 : ",
				"Killed : "
			},
			{
				"긴 튜토리얼을 따라 오시느라 고생하셨네요. \n보상으로 이걸 선물로 드리지요. \n사용방법은 차츰 알게 되실겁니다.",
				"you have suffered a long tutorial. good job. \nI'll reward as a gift here. take it. \nyou will gradually learn how to use this."
			},
			{
				"제가 도울 수 있는 것은 여기까지 입니다. \n계단을 찾아 '소트아트온라인'으로 접속할 준비를 하세요.",
				"The tutorial is over. \nFind the stairs to start to the 'sword art online'."
			}
		};
		public HuntMission(TutorialDungeon d, GameObject shooter) 
		{ 
			td = d; 
			shooterObj = shooter; 
		}
		public override void start()
		{
			base.start ();
			monList = new List<GameObject> ();
			SpeechController.GetInstance ().skipAvailable (false);
			SpeechController.GetInstance ().pushPrint (null,
			                                           Desc [0, Configuration.LanguageSelect]);
			for(int i = 0; i < spawn; i++)
				monList.Add (td.SpawnMonster (td.Monsters[0]));
			if(shooterObj != null)
				shooterObj.SetActive(true);
		}

		public override bool checkMission()
		{
			int count = 0;
			foreach(GameObject m in monList)
			{
				if(m == null)
					count++;
			}
			if(!NotificationManager.GetInstance().isToasting())
				NotificationManager.GetInstance().toast ( Desc [1, Configuration.LanguageSelect] + count.ToString());

			if (count+2 == spawn)
			{
				Vector3 pos = GameManager.PlayerObject.transform.position;
				pos.y += 1f;
				pos.x += 2f;
				GameObject item = GameManager.World.GetComponent<ItemFactory>().GetSingleRandomPart(1.0f, ItemFactory.RANDOM, pos); // drop item.
				item.GetComponent<ItemCube>().part.initData(ItemRank.NORMAL);
				SpeechController.GetInstance ().pushPrint (null,
				                                           Desc [2, Configuration.LanguageSelect]);
				SpeechController.GetInstance ().pushPrint (null,
				                                           Desc [3, Configuration.LanguageSelect]);
				GameManager.missionCleared = true;
				return true;
			}
			else return false;
		}
	}

	public sealed override void Update ()
	{
		base.Update ();
		if (missions.Count <= currentMission)
			return;
		if(!missions[currentMission].isInProgress)
		{
			timer += Time.deltaTime;
			if (timer >= missions[currentMission].startTime) 
			{
				timer = 0;
				missions[currentMission].start ();
			}
		}
		else // mission check.
		{
			if(missions[currentMission].checkMission())
				currentMission++;
		}
	}
}
