using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Component callback Interface & Component unity helper
public abstract class GameManagerComponent
{
	public string ComponentName;
	public GameManagerComponent _this;
	// can be implemented bundles.

	public abstract void callbackMethod();

}


