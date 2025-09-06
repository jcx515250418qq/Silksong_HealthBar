using HutongGames.PlayMaker;
using UnityEngine;

public sealed class PauseAudioListener : FsmStateAction
{
	public FsmBool pause;

	public FsmBool unpauseOnExit;

	public override void Reset()
	{
		pause = null;
		unpauseOnExit = null;
	}

	public override void OnEnter()
	{
		AudioListener.pause = pause.Value;
		Finish();
	}

	public override void OnExit()
	{
		if (unpauseOnExit.Value)
		{
			AudioListener.pause = false;
		}
	}
}
