using UnityEngine;
using System.Collections;

public class Mantis3D : Monster3DBase {
	public float malfunctionRate = 0.5f;
	protected bool malfunctioned = false;

	public override bool hit(int damage)
	{
		if(base.hit(damage))
		{
			float rate = getHPRate();
			if(rate < malfunctionRate)
			{
				view.freezeVelocity();
				malfunctioned = true;
				anim.SetBool("armAttacked", true);
			}
			return true;
		}
		return false;
	}

	protected override void Update ()
	{
		// malfunctioned.
		if(malfunctioned)
			model.UpdateSufferedState ();
		else
			base.Update();
	}
}
