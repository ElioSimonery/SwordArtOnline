using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// <<DAO>>
public class FileScriptReader
{
	// , -> #
	public static bool loading = false;
	public static List<SpeechController.Node> LoadFile(string pathWithFileName)
	{
		loading = true;
		List<SpeechController.Node> ret = new List<SpeechController.Node>();
		BabelStreamReader sr = new BabelStreamReader (pathWithFileName);

		string input = "";
		while (true)
		{
			input = sr.ReadLine();
			if (input == null) { break; }
			if(input.Contains("//"))
				continue;

			string[] values = split7 (input);
			SpeechController.Node.n_type nType = ReadType(values[0]);

			string nName = ReadString (values[1]);
			string nContent = ReadString (values[2]);

			string nPort_sprName = ReadString(values[4]);
			UIAtlas nPort_atlas = ReadAtlas ("portraits", values[3], nPort_sprName);

			string nBackground_sprName = ReadString(values[6]);
			UIAtlas nBackground_atlas = ReadAtlas ("stage", values[5], nBackground_sprName);

			SpeechController.Node newNode = new SpeechController.Node(nType, nName, nContent, nPort_atlas, nPort_sprName, nBackground_atlas, nBackground_sprName);
			ret.Add(newNode);
		}
		sr.Close();
		loading = false;
		return ret;
	}

	protected static string[] split7(string inputData)
	{
		string[] ret = inputData.Split(',');
		for(int i = 0; i < ret.Length; i++)
		{
			ret[i] = ret[i].Replace(@"#",@",");
			ret[i] = ret[i].Replace(@"\n",Environment.NewLine);
		}
		if (ret.Length > 7)
		{
			Debug.LogError ("[FileScriptReader] There is an inconsistency in ScriptFile.");
			Debug.LogError("Exception Log) Read : " + ret.Length);
			for(int i = 0; i < ret.Length; i++)
				Debug.LogError(ret[i]);
			Debug.LogError ("[FileScriptReader] Exception Log end.");
		}
		else if(ret.Length < 7)
		{
			Debug.LogError("[FileScriptReader] ScriptFile has repaired.");
			string[] tmp = new string[7];
			for(int i = 0; i < ret.Length; i++)
			{
				tmp[i] = ret[i];
			}
			return tmp;
		}
		return ret;
	}

	protected static SpeechController.Node.n_type ReadType(string t)
	{
		switch(t)
		{
		case "Normal":
			return SpeechController.Node.n_type.NORMAL;
		case "Narration":
			return SpeechController.Node.n_type.NARRATION;
		case "Message":
			return SpeechController.Node.n_type.MESSAGE;
		default:
			Debug.LogError("[FileScriptReader] Script File Error has occured in Type data.");
			return SpeechController.Node.n_type.NORMAL;
		}
	}

	protected static UIAtlas ReadAtlas(string prePath, string n, string spriteName)
	{
		if (string.IsNullOrEmpty (n))
			return null;
		GameObject atlasObj = Resources.Load (prePath + "/" + n + "/" + n) as GameObject;
		UIAtlas atlas = null;

		if (atlasObj == null)
			Debug.LogError ("[FileScriptReader] No such atlas path : " + prePath + "/" + n + "/" + n + ", spriteName="+spriteName);

		atlas = atlasObj.GetComponent<UIAtlas> ();
		for (int i = 1; atlas.GetSprite (spriteName) == null; i++)
		{
			atlasObj = Resources.Load (prePath+"/"+n+"/"+n+"."+i) as GameObject;
			if(atlasObj == null) 
			{
				Debug.LogError ("[FileScriptReader] No such atlas prefab path : " + prePath + "/" + n + "/" + n + "_"+i + ", spriteName="+spriteName);
				break;
			}
			atlas = atlasObj.GetComponent<UIAtlas> ();
		}

		return atlas;
	}

	protected static string ReadString(string n) { return n; }
}


