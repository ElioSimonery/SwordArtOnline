using UnityEngine;


public class PlayCinemaMission : MissionBase
{
	string cine;
	public PlayCinemaMission(string cinema_fileName)
	{ cine = cinema_fileName; }
	public override bool checkMission()
	{
		if (SpeechController.GetInstance ().noSpeech) 
		{
			CinemaManager.PlayCinema (cine);
			return true;
		}

		return false;
	}
}