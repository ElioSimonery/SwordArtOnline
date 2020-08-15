using UnityEngine;
using System.Collections;

public class CinemaManager : MonoBehaviour {
	private static CinemaManager mInstance;

	// enum string
	public class CINEMA
	{
		public static string Stage7_YUI = "stage7_yui.mp4";
		public static string Stage8_ENDING = "stage8_ending.mp4";
	}

	void Awake()
	{
		mInstance = this;
	}
	/*
	public static void GetInstance(string path)
	{
		if (mInstance == null)
			Debug.LogError ("[CinemaManager] hasn't been created!");
		return mInstance;
	}*/

	public static void PlayCinema(string CINEMA_fileName)
	{
		Debug.Log ("[CinemaManager] Called play cinema :" + CINEMA_fileName);

		if(!Handheld.PlayFullScreenMovie (CINEMA_fileName, Color.black, FullScreenMovieControlMode.CancelOnInput))
		{
			Debug.LogError("[CinemaManager] Couldn't play the cinema : " + CINEMA_fileName);
		}
	}
}
