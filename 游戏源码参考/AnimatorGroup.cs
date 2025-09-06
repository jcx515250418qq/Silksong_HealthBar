using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorGroup : MonoBehaviour
{
	private struct DelayedPlay
	{
		public float PlayDelayLeft;

		public Animator Animator;

		public Action<Animator> Action;
	}

	[SerializeField]
	private List<Animator> animators;

	[SerializeField]
	private bool useChildren;

	[Space]
	[SerializeField]
	private string startAnim;

	[SerializeField]
	private float delayVariance;

	[SerializeField]
	private float delayInSequence;

	private Dictionary<string, List<DelayedPlay>> delayedPlays;

	private void Awake()
	{
		if (useChildren)
		{
			animators.AddRange(GetComponentsInChildren<Animator>());
		}
	}

	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(startAnim))
		{
			Play(startAnim);
		}
	}

	private void Update()
	{
		if (delayedPlays == null)
		{
			return;
		}
		foreach (KeyValuePair<string, List<DelayedPlay>> delayedPlay in delayedPlays)
		{
			List<DelayedPlay> value = delayedPlay.Value;
			for (int num = value.Count - 1; num >= 0; num--)
			{
				DelayedPlay value2 = value[num];
				value2.PlayDelayLeft -= Time.deltaTime;
				if (value2.PlayDelayLeft > 0f)
				{
					value[num] = value2;
				}
				else
				{
					value2.Action(value2.Animator);
					value.RemoveAt(num);
				}
			}
		}
	}

	private void DoAction(string key, Action<Animator> action)
	{
		if (delayVariance <= 0f)
		{
			foreach (Animator animator in animators)
			{
				action(animator);
			}
			return;
		}
		if (delayedPlays == null)
		{
			delayedPlays = new Dictionary<string, List<DelayedPlay>>();
		}
		if (delayedPlays.TryGetValue(key, out var value))
		{
			value.Clear();
		}
		else
		{
			value = new List<DelayedPlay>(animators.Count);
			delayedPlays[key] = value;
		}
		float num = 0f;
		foreach (Animator animator2 in animators)
		{
			float playDelayLeft = UnityEngine.Random.Range(0f, delayVariance) + num;
			value.Add(new DelayedPlay
			{
				PlayDelayLeft = playDelayLeft,
				Animator = animator2,
				Action = action
			});
			num += delayInSequence;
		}
	}

	public void Play(string stateName)
	{
		int animHash = Animator.StringToHash(stateName);
		DoAction(stateName, delegate(Animator animator)
		{
			animator.Play(animHash, 0, 0f);
		});
	}

	public void SetBool(string boolName, bool value)
	{
		int boolHash = Animator.StringToHash(boolName);
		DoAction(boolName, delegate(Animator animator)
		{
			animator.SetBool(boolHash, value);
		});
	}
}
