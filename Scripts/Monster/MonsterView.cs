using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MonsterView : MonoBehaviour
{
	public bool is3D = false;
	public bool applyHitEffect = true;
	// Unity Interface
	public GameObject	this_character;
	public GameObject	miniHudObject;

	public IEnumerator hitEffect = null; // Use only get outside

	private SpriteRenderer[] sprites;
	private MeshRenderer[] renderers3D;
	private SkinnedMeshRenderer[] s_renderers3D;
	private Color[] prev_colors;

	void Awake()
	{
		if (is3D && applyHitEffect)
		{
			renderers3D = GetComponentsInChildren<MeshRenderer> ();
			s_renderers3D = GetComponentsInChildren<SkinnedMeshRenderer> ();
			prev_colors = new Color[renderers3D.Length+s_renderers3D.Length];
			for(int i = 0; i < renderers3D.Length; i++)
				prev_colors[i] = renderers3D[i].material.color;
			for(int i = renderers3D.Length; i < renderers3D.Length+s_renderers3D.Length; i++)
				prev_colors[i] = s_renderers3D[i-renderers3D.Length].material.color;
		}
		else
			sprites = GetComponentsInChildren<SpriteRenderer>();

		if (miniHudObject == null) // minihud can be inactive state by itself at this moment. so you input directly here the object.
		{
			Debug.LogWarning("minihud auto finder activated.");
			if(!(miniHudObject = transform.FindChild("MiniHud").gameObject))
				Debug.LogError(gameObject.name + " monster couldn't find a minihud component.");
		}
		miniHudObject.SetActive(true);
		if(this_character == null)
		{
			Debug.LogWarning("character auto finder activated.");
			if(!(this_character = transform.FindChild("Character").gameObject))
				Debug.LogError(gameObject.name + " monster couldn't find its monster character.");
		}
	}

	public void freezeVelocity()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	public void moveToCharacter(float speed)
	{
		Vector3 dir = (GameManager.PlayerObject.transform.position-this.transform.position);
		Vector3 v = speed*dir.normalized;
		Vector3 acc = (v - GetComponent<Rigidbody>().velocity) / Time.fixedDeltaTime;
		GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().mass * acc);
	}

	public void prowling(Vector3 v)
	{
		Vector3 acc = (v - GetComponent<Rigidbody>().velocity) / Time.fixedDeltaTime;
		GetComponent<Rigidbody>().AddForce(GetComponent<Rigidbody>().mass * acc);
	}

	public void OnHitEffect(float hpRate)
	{
		miniHudObject.GetComponent<MiniHud>().setGauge (true, hpRate);
		if (!applyHitEffect)
			return;
		if(hitEffect != null)
			StopCoroutine(hitEffect);
		if(is3D)
			StartCoroutine(hitEffect = HitEffect3D());
		else
			StartCoroutine(hitEffect = HitEffect());
	}

	public void OnDestroyView()
	{
		if(hitEffect != null)
			StopCoroutine(hitEffect);
	}

	protected IEnumerator HitEffect()
	{
		// Set Red
		foreach (SpriteRenderer sprite in sprites)
			sprite.material.color = Color.red;
		yield return new WaitForSeconds(0.03f);

		// Flash white
		for(float gb = 0f; gb < 1f; gb += 0.1f)
		{
			foreach(SpriteRenderer sprite in sprites)
				sprite.material.color = new Color(sprite.material.color.r, gb, gb, 1f);

			yield return new WaitForSeconds(0.02f);
		}

		// Recovery
		foreach (SpriteRenderer sprite in sprites)
			sprite.material.color = Color.white;
		yield return new WaitForSeconds (0.03f);
	}

	protected IEnumerator HitEffect3D()
	{
		// Set Red
		foreach (MeshRenderer render in renderers3D)
			render.material.color = Color.red;
		foreach (SkinnedMeshRenderer render in s_renderers3D)
			render.material.color = Color.red;
		yield return new WaitForSeconds(0.03f);
		
		// Flash white
		for(float gb = 0f; gb < 1f; gb += 0.1f)
		{
			foreach (MeshRenderer render in renderers3D)
				render.material.color = new Color(render.material.color.r, gb, gb, 1f);
			foreach (SkinnedMeshRenderer render in s_renderers3D)
				render.material.color = new Color(render.material.color.r, gb, gb, 1f);

			yield return new WaitForSeconds(0.02f);
		}
		
		// Recovery
		for (int i = 0; i < renderers3D.Length; i++)
			renderers3D [i].material.color = prev_colors [i];
		for(int i = 0; i < s_renderers3D.Length; i++)
			s_renderers3D [i].material.color = prev_colors [i+renderers3D.Length];

		yield return new WaitForSeconds (0.03f);
	}

	public void turn2D()
	{
		float side = transform.position.x - GameManager.PlayerObject.transform.position.x;
		Vector3 _scale = this_character.transform.localScale;
		if(side > 0) // turn left
		{
			if(_scale.x > 0)
				_scale.x = -_scale.x;
			this_character.transform.localScale = _scale;
		}
		else // turn right
		{
			if(_scale.x < 0)
				_scale.x = -_scale.x;
			this_character.transform.localScale = _scale;
		}
	}

	public void gazePlayer3D()
	{
		Vector3 look = transform.position - GameManager.PlayerObject.transform.position;
		look.y = 0f;
		this_character.transform.rotation = Quaternion.LookRotation(look);
	}
}