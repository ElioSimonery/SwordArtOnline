using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 사실 상속으로 구현하는게 맞음
public class BabelStreamReader
{
	TextAsset textAsset;
	string[] texts;
	int cursor;

	public BabelStreamReader(string readPath)
	{
		textAsset = Resources.Load(readPath) as TextAsset;
		texts = textAsset.text.Split('\n');
		for(int i = 0; i < texts.Length; i++)
		{
			if(texts[i].Length > 1)
				texts[i] = texts[i].Substring(0, texts[i].Length-1);
		}
		cursor = 0;
	}

	public string ReadLine ()
	{
		if(cursor < texts.Length)
		{
			if(texts[cursor] == "")
				return null;

			return texts [cursor++];
		}
		return null;
	}

	// For consistency, we make this simple function.
	public void Close()
	{

	}
}