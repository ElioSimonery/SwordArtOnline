using UnityEngine;
using System.Collections.Generic;

public class PlayerStageIO
{
	public static string[] Stage = {
		"Story1_Aincrad_1floor_Scene",
		"Story2_Treasure_27floor_Scene",
		"Story3_FlowerGarden_47floor_Scene",
		"Story4_Dragon_55floor_Scene",
		"Story5_Betray_55floor_Scene",
		"Story6_Yui_Scene",
		"Story7_YuiAI_1floor_Scene",
		"Story8_YuiDead_1floor_Scene"
	};
	private static PlayerStageIO mInstance;
	public List<string> stages;
	PlayerStageIO()
	{
		load ();
	}

	public List<string> load()
	{
		stages = new List<string> ();
		
		for (int i = 0; i < Stage.Length; i++) 
		{
			if(!string.IsNullOrEmpty(PlayerPrefs.GetString (Stage[i])))
			{
				Debug.Log("[PlayerStageIO] available stage : " + Stage[i]);
				stages.Add(Stage[i]);
			}
		}
		return stages;
	}

	public void save(string sceneName)
	{
		PlayerPrefs.SetString (Common.DATA_STORY_SCENE, sceneName);
		PlayerPrefs.SetString (sceneName, "true");
	}


	
	public static PlayerStageIO GetInstance()
	{
		if(mInstance == null)
		{
			mInstance = new PlayerStageIO();
		}
		return mInstance;
	}

}
