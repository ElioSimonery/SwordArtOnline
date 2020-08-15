using UnityEngine;
using System.Collections;

public abstract class SufferedState {
	public string stateName = "none state";
	public float maxLastingTime;
	public GameObject targetObj;

	protected float currentTimer;
	private bool suffering = false;
	private bool finished = false; 

	public SufferedState(GameObject obj, float duration)
	{
		targetObj = obj;
		maxLastingTime = duration;
	}

	// This calls only once at start.
	public virtual bool OnSufferedStart () // false false
	{	// Must call Parent start first.
		if(finished) return false; 	
		if(suffering) return false;
		else 
			suffering = true;			//make false true once.
		return true;
	}

	public virtual bool SufferingUpdate () // false true
	{
		if(finished) return false;
		if(!suffering) return false;
		currentTimer += Time.deltaTime;
		if(currentTimer > maxLastingTime)
		{
			finished = true; // make true true once.
		}
		return true;
	}

	protected virtual void OnSufferedFinish () // true true
	{

	}

	public void FinishOnDie()
	{
		if(!isFinished())
			OnSufferedFinish ();
	}
	public void OnSufferedStop() // false false
	{
		if(!finished) return;
		if(!suffering) return;
		OnSufferedFinish(); // callback
		// set true false
		suffering = false;
	}

	public bool isFinished()
	{
		return finished && !suffering;
	}
}
