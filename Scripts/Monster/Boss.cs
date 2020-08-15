using UnityEngine;
using System.Collections;

public class Boss : IMonsterBase {
	public float remainingCorpseTime;
	protected float dead_timer;
	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		if(model.isDead)
		{
			anim.SetBool("isDead",true);
			if(dead_timer > remainingCorpseTime)
			{
				Destroy(gameObject);
			}
			dead_timer += Time.deltaTime;
			return;
		}
		base.Update ();
	}
}
