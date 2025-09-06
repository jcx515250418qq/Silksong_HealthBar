using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class ChainSequence : SkippableSequence
{
	[SerializeField]
	private SkippableSequence[] sequences;

	[SerializeField]
	private bool keepLastActive;

	[SerializeField]
	private NestedFadeGroupBase endBlanker;

	private int currentSequenceIndex = -1;

	private float fadeByController;

	private bool isSkipped;

	public SkippableSequence CurrentSequence
	{
		get
		{
			if (currentSequenceIndex < 0 || currentSequenceIndex >= sequences.Length)
			{
				return null;
			}
			return sequences[currentSequenceIndex];
		}
	}

	public bool IsCurrentSkipped
	{
		get
		{
			if (CurrentSequence != null)
			{
				return CurrentSequence.IsSkipped;
			}
			return false;
		}
	}

	public bool CanSkipCurrent
	{
		get
		{
			if (CurrentSequence != null)
			{
				return CurrentSequence.CanSkip;
			}
			return false;
		}
	}

	public override bool IsSkipped => isSkipped;

	public override bool IsPlaying
	{
		get
		{
			if (currentSequenceIndex < sequences.Length - 1)
			{
				return true;
			}
			if (CurrentSequence != null)
			{
				return CurrentSequence.IsPlaying;
			}
			return false;
		}
	}

	public override float FadeByController
	{
		get
		{
			return fadeByController;
		}
		set
		{
			fadeByController = Mathf.Clamp01(value);
			SkippableSequence[] array = sequences;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FadeByController = fadeByController;
			}
		}
	}

	public event Action TransitionedToNextSequence;

	public event Action EndBlankerFade;

	public event Action SequenceComplete;

	protected void Awake()
	{
		fadeByController = 1f;
		if ((bool)endBlanker)
		{
			endBlanker.AlphaSelf = 0f;
		}
	}

	protected void Update()
	{
		if (!isSkipped && !(CurrentSequence == null) && !CurrentSequence.IsPlaying && (!CurrentSequence.WaitForSkip || CurrentSequence.IsSkipped))
		{
			Next();
		}
	}

	public override void Begin()
	{
		isSkipped = false;
		currentSequenceIndex = -1;
		Next();
	}

	private void Next()
	{
		SkippableSequence currentSequence = CurrentSequence;
		currentSequenceIndex++;
		if (currentSequence != null && (!keepLastActive || (bool)CurrentSequence))
		{
			currentSequence.SetActive(value: false);
		}
		if (isSkipped)
		{
			return;
		}
		if (CurrentSequence != null)
		{
			if (!CurrentSequence.ShouldShow)
			{
				Next();
				return;
			}
			CurrentSequence.SetActive(value: true);
			CurrentSequence.Begin();
			this.TransitionedToNextSequence?.Invoke();
		}
		else if ((bool)endBlanker)
		{
			this.EndBlankerFade?.Invoke();
			this.StartTimerRoutine(0f, 1f, delegate(float t)
			{
				endBlanker.AlphaSelf = t;
			}, null, this.SequenceComplete);
		}
		else
		{
			this.SequenceComplete?.Invoke();
		}
	}

	public override void Skip()
	{
		isSkipped = true;
		SkippableSequence[] array = sequences;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Skip();
		}
	}

	public void SkipSingle()
	{
		if (!(CurrentSequence == null) && CurrentSequence.CanSkip)
		{
			CurrentSequence.Skip();
		}
	}
}
