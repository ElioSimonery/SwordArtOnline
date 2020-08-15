using UnityEngine;
// Simple Script read mission.
public class ScriptMission : MissionBase
{
	private string scriptFileWithPath;
	private bool skipAvailable;
	private bool screenActivate;
	private bool handlingCheck;
	public ScriptMission(string _scriptFilePath, bool _skipAvailable, bool _screenActivate/*delete this*/, bool _handlingCheck = true)
	{
		scriptFileWithPath = _scriptFilePath;
		skipAvailable = _skipAvailable;
		screenActivate = _screenActivate;
		handlingCheck = _handlingCheck;
	}

	public override void start()
	{
		base.start ();
	}
	
	public override bool checkMission()
	{
		if(handlingCheck)
		{
			if(!VFloatingController.GetAgileInstance(true).isHandling() && 
			   !VFloatingController.GetAgileInstance(false).isHandling())
			{
				flushScript();
				return true;
			}
		}
		else
		{
			flushScript();
			return true;
		}

		return false;
	}

	protected void flushScript()
	{
		GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.move (false, Vector3.zero);
		GameManager.PlayerObject.GetComponent<CharacterControlHelper> ().c.shoot (Vector3.zero);

		VFloatingController.GetAgileInstance(true).forceToInitializeController();
		VFloatingController.GetAgileInstance(false).forceToInitializeController();

		GameUIPanelManager.GetInstance ().animActivation (true);
		SpeechController.GetInstance ().skipAvailable (false);
		SpeechController.GetInstance ().pushScript (FileScriptReader.LoadFile (scriptFileWithPath));
		SpeechController.GetInstance ().skipAvailable (true);
	}
}
