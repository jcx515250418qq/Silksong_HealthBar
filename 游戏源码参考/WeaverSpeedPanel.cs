using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class WeaverSpeedPanel : MonoBehaviour
{
	private enum HeroStateSpeeds
	{
		Normal = 0,
		WithQuickening = 1,
		WithSprintCharm = 2,
		WithQuickeningAndSprintCharm = 3
	}

	[SerializeField]
	private NestedFadeGroupBase[] lights;

	[SerializeField]
	private float lightUpDelay;

	[SerializeField]
	private float lightUpDuration;

	[SerializeField]
	private float lightDownDelay;

	[SerializeField]
	private float lightDownDuration;

	[SerializeField]
	private float litHoldTime;

	[Space]
	[SerializeField]
	private AudioClip lightSound;

	[SerializeField]
	private float lightSoundPitchBase;

	[SerializeField]
	private float lightSoundPitchIncrease;

	[Space]
	[SerializeField]
	private float maxSpeed;

	[SerializeField]
	[ArrayForEnum(typeof(HeroStateSpeeds))]
	private int[] heroStateLightThresholds;

	private Coroutine lightRoutine;

	private float holdTimeLeft;

	private static List<WeaverSpeedPanel> _activePanels;

	public bool ForceStayLit { get; set; }

	public event Action<int> RecordedSpeedThreshold;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref heroStateLightThresholds, typeof(HeroStateSpeeds));
		for (int i = 0; i < heroStateLightThresholds.Length; i++)
		{
			int num = heroStateLightThresholds[i];
			if (num > lights.Length)
			{
				heroStateLightThresholds[i] = lights.Length;
			}
			else if (num < 0)
			{
				heroStateLightThresholds[i] = 0;
			}
		}
	}

	private void Awake()
	{
		OnValidate();
		NestedFadeGroupBase[] array = lights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].AlphaSelf = 0f;
		}
	}

	private void OnEnable()
	{
		if (_activePanels == null)
		{
			_activePanels = new List<WeaverSpeedPanel>();
		}
		_activePanels.Add(this);
	}

	private void OnDisable()
	{
		_activePanels.Remove(this);
		if (_activePanels.Count == 0)
		{
			_activePanels = null;
		}
	}

	public void RecordSpeed()
	{
		HeroController instance = HeroController.instance;
		Vector2 linearVelocity = instance.Body.linearVelocity;
		if (lightRoutine != null)
		{
			StopCoroutine(lightRoutine);
			NestedFadeGroupBase[] array = lights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FadeToZero(lightDownDuration);
			}
		}
		float num = Mathf.Abs(linearVelocity.x) / (maxSpeed - Mathf.Epsilon);
		if (num > 1f)
		{
			num = 1f;
		}
		int num2 = Mathf.FloorToInt(num * (float)lights.Length);
		bool isUsingQuickening = instance.IsUsingQuickening;
		bool isEquipped = Gameplay.SprintmasterTool.IsEquipped;
		HeroStateSpeeds heroStateSpeeds = ((isUsingQuickening && isEquipped) ? HeroStateSpeeds.WithQuickeningAndSprintCharm : (isUsingQuickening ? HeroStateSpeeds.WithQuickening : (isEquipped ? HeroStateSpeeds.WithSprintCharm : HeroStateSpeeds.Normal)));
		int num3 = heroStateLightThresholds[(int)heroStateSpeeds];
		if (num2 > num3)
		{
			num2 = num3;
		}
		else if (num2 < 1)
		{
			num2 = 1;
		}
		this.RecordedSpeedThreshold?.Invoke(num2);
		lightRoutine = StartCoroutine(Light(num2));
	}

	private IEnumerator Light(int threshold)
	{
		Vector3 pos = base.transform.position;
		pos.z = 0f;
		WaitForSeconds wait = new WaitForSeconds(lightUpDelay);
		for (int i = 0; i < threshold; i++)
		{
			NestedFadeGroupBase l = lights[i];
			yield return wait;
			l.FadeToOne(lightUpDuration);
			if ((bool)lightSound)
			{
				float num = lightSoundPitchBase + lightSoundPitchIncrease * (float)i;
				AudioEvent audioEvent = default(AudioEvent);
				audioEvent.Clip = lightSound;
				audioEvent.PitchMin = num;
				audioEvent.PitchMax = num;
				audioEvent.Volume = 1f;
				AudioEvent audioEvent2 = audioEvent;
				audioEvent2.SpawnAndPlayOneShot(pos);
			}
		}
		holdTimeLeft = litHoldTime;
		while (true)
		{
			yield return null;
			holdTimeLeft -= Time.deltaTime;
			if (holdTimeLeft > 0f || ForceStayLit)
			{
				continue;
			}
			bool flag = false;
			foreach (WeaverSpeedPanel activePanel in _activePanels)
			{
				if (!(activePanel.holdTimeLeft <= 0f))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		this.RecordedSpeedThreshold?.Invoke(0);
		wait = new WaitForSeconds(lightDownDelay);
		for (int i = threshold - 1; i >= 0; i--)
		{
			NestedFadeGroupBase l = lights[i];
			yield return wait;
			l.FadeToZero(lightDownDuration);
		}
		lightRoutine = null;
	}
}
