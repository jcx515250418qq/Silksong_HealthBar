using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;

public class EndingSequence : MonoBehaviour, GameManager.ISkippable
{
	[SerializeField]
	private ChainSequence chainSequence;

	[SerializeField]
	private AudioMixerSnapshot endBlankSnapshot;

	private void Awake()
	{
		chainSequence.EndBlankerFade += OnEndBlankerFade;
		chainSequence.SequenceComplete += OnSequenceComplete;
	}

	private void Start()
	{
		GameManager instance = GameManager.instance;
		instance.inputHandler.SetSkipMode(SkipPromptMode.SKIP_INSTANT);
		instance.RegisterSkippable(this);
		chainSequence.Begin();
	}

	private void OnDestroy()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.DeregisterSkippable(this);
		}
	}

	public IEnumerator Skip()
	{
		chainSequence.SkipSingle();
		while (chainSequence.IsCurrentSkipped)
		{
			yield return null;
		}
	}

	private void OnEndBlankerFade()
	{
		if ((bool)endBlankSnapshot)
		{
			endBlankSnapshot.TransitionTo(1f);
		}
	}

	private static void OnSequenceComplete()
	{
		GameManager instance = GameManager.instance;
		instance.StartCoroutine(instance.ReturnToMainMenu(willSave: true, null, isEndGame: true));
	}
}
