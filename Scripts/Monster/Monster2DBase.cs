using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster2DBase : IMonsterBase
{
	protected override void OnTriggerStay(Collider other) {
		// in wake state.
		if(other.tag == Common.TAG_PLAYER)
		{
			model.updateAttackState(transform.position);
			view.turn2D(); // if 3d, do not call.
		}
	}
}
