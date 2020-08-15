using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffectManager : MonoBehaviour {
	public readonly string DROPPED_ITEM = "droppedItem";
	Dictionary<string, AudioSource> sounds;
	private static SoundEffectManager mInstance;

	// Use this for initialization
	void Awake () {
		sounds = new Dictionary<string, AudioSource> ();
		AudioSource[] audio = GetComponents <AudioSource> ();
		foreach(AudioSource a in audio)
		{
			sounds.Add (a.clip.name, a);
		}
		mInstance = GetComponent<SoundEffectManager> ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public static SoundEffectManager GetInstance()
	{
		if(mInstance == null)
			Debug.LogError("[SoundEffectManager] hasn't created.");
		return mInstance;
	}
	public bool register(string name, AudioSource audio)
	{
		if (sounds.ContainsKey (name))
		{
			Debug.LogWarning("[SoundEffectManager] Already contained sound : " + name);
			return false;
		}
		AudioSource newAudio = gameObject.AddComponent<AudioSource> ();
		newAudio.playOnAwake = false;
		newAudio.clip = audio.clip;
		sounds.Add (name, newAudio);
		return true;
	}

	public bool register(string name, AudioClip clip)
	{
		if (sounds.ContainsKey (name))
		{
			Debug.LogWarning("[SoundEffectManager] Already contained the sound : " + name);
			return false;
		}
		AudioSource newAudio = gameObject.AddComponent<AudioSource> ();
		newAudio.playOnAwake = false;
		newAudio.clip = clip;
		sounds.Add (name, newAudio);
		return true;
	}

	public bool contains(string name)
	{
		return sounds.ContainsKey (name);
	}
	public void play(string name)
	{
		sounds [name].Play ();
	}
	public void stop(string name)
	{
		sounds [name].Stop ();
	}
}
