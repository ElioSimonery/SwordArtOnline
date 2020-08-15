using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GMAccMonsterComponent : GameManagerComponent
{
	public static string COMPONENT_NAME = "MonsterOnDead";

	private int count = 0;
	public GMAccMonsterComponent()
	{
		_this = this;
		ComponentName = COMPONENT_NAME;
	}

	public override void callbackMethod ()
	{
		CountOnMonsterDead ();
	}

	public void CountOnMonsterDead()
	{
		count++;
	}

	public int getCount() { return count; }
}


