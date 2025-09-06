using UnityEngine;

public static class ScreenFaderUtils
{
	public static void Fade(Color startColour, Color endColour, float duration)
	{
		GameManager instance = GameManager.instance;
		if (!(instance == null))
		{
			PlayMakerFSM screenFader_fsm = instance.screenFader_fsm;
			screenFader_fsm.FsmVariables.GetFsmColor("Start Colour").Value = startColour;
			screenFader_fsm.FsmVariables.GetFsmColor("End Colour").Value = endColour;
			screenFader_fsm.FsmVariables.GetFsmFloat("Duration").Value = duration;
			screenFader_fsm.SendEvent("CUSTOM FADE");
		}
	}

	public static void SetColour(Color colour)
	{
		Fade(colour, colour, 0f);
	}

	public static Color GetColour()
	{
		GameManager instance = GameManager.instance;
		if (instance == null)
		{
			Debug.Log("GameManager could not be found");
			return Color.clear;
		}
		return instance.screenFader_fsm.FsmVariables.GetFsmGameObject("Screen Fader").Value.GetComponent<SpriteRenderer>().color;
	}
}
