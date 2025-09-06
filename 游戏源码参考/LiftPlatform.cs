using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class LiftPlatform : DebugDrawColliderRuntimeAdder
{
	private enum PartStates
	{
		Down = 0,
		Up = 1
	}

	private struct PartTimer
	{
		public float DelayLeft;

		public PartStates State;

		public float Timer;

		public Transform Part;

		public float Magnitude;

		public float InitialYPos;
	}

	[Serializable]
	private class LiftPart
	{
		public Transform Transform;

		public MinMaxFloat Delay;

		public MinMaxFloat Magnitude = new MinMaxFloat(0.75f, 0.75f);
	}

	private const float MIN_REACT_MAGNITUDE = 6f;

	[SerializeField]
	private LayerMask collisionMask = -1;

	[Space]
	[SerializeField]
	private List<LiftPart> parts;

	[SerializeField]
	private ParticleSystem dustParticle;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioEventRandom landAudio;

	[SerializeField]
	private RandomAudioClipTable[] landAudioTables;

	[Space]
	[SerializeField]
	private VibrationDataAsset bobVibration;

	[Space]
	public UnityEvent OnBob;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject part1;

	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private GameObject part2;

	private List<PartTimer> runningTimers;

	private bool isDisabled;

	private void OnValidate()
	{
		if ((bool)part1)
		{
			parts.Add(new LiftPart
			{
				Transform = part1.transform,
				Delay = new MinMaxFloat(0f, 0f),
				Magnitude = new MinMaxFloat(0.75f, 0.75f)
			});
			part1 = null;
		}
		if ((bool)part2)
		{
			parts.Add(new LiftPart
			{
				Transform = part2.transform,
				Delay = new MinMaxFloat(0f, 0f),
				Magnitude = new MinMaxFloat(0.75f, 0.75f)
			});
			part2 = null;
		}
	}

	private void Reset()
	{
		parts = new List<LiftPart>
		{
			new LiftPart()
		};
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
	}

	private void OnDisable()
	{
		if (runningTimers == null)
		{
			return;
		}
		foreach (PartTimer runningTimer in runningTimers)
		{
			runningTimer.Part.SetLocalPositionY(runningTimer.InitialYPos);
		}
		runningTimers.Clear();
	}

	private void LateUpdate()
	{
		if (runningTimers == null)
		{
			return;
		}
		for (int num = runningTimers.Count - 1; num >= 0; num--)
		{
			PartTimer value = runningTimers[num];
			if (value.DelayLeft > 0f)
			{
				value.DelayLeft -= Time.deltaTime;
			}
			else
			{
				switch (value.State)
				{
				case PartStates.Down:
					if (value.Timer < 0.12f)
					{
						value.Part.SetLocalPositionY(value.InitialYPos - value.Timer * value.Magnitude);
						value.Timer += Time.deltaTime;
					}
					else
					{
						value.Part.SetLocalPositionY(value.InitialYPos - 0.12f * value.Magnitude);
						value.State = PartStates.Up;
						value.Timer = 0.12f;
					}
					break;
				case PartStates.Up:
					if (value.Timer > 0f)
					{
						value.Part.SetLocalPositionY(value.InitialYPos - value.Timer * value.Magnitude);
						value.Timer -= Time.deltaTime;
						break;
					}
					value.Part.SetLocalPositionY(value.InitialYPos);
					runningTimers.RemoveAt(num);
					continue;
				}
			}
			runningTimers[num] = value;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		int layer = collision.collider.gameObject.layer;
		if (((1 << layer) & (int)collisionMask) != 0 && layer != 16 && layer != 18 && !(collision.GetSafeContact().Normal.y >= 0f) && (layer == 9 || !(collision.relativeVelocity.magnitude < 6f)))
		{
			DoBob();
		}
	}

	public void DoBob()
	{
		DoBobInternal(playSound: true, vibrate: true);
	}

	public void DoBob(bool vibrate)
	{
		DoBobInternal(playSound: true, vibrate);
	}

	public void DoBobSilent()
	{
		DoBobInternal(playSound: false, vibrate: false);
	}

	private void DoBobInternal(bool playSound, bool vibrate)
	{
		if (!base.enabled || isDisabled)
		{
			return;
		}
		if (playSound)
		{
			if ((bool)audioSource)
			{
				audioSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
				audioSource.Play();
			}
			landAudio.SpawnAndPlayOneShot(base.transform.position);
			RandomAudioClipTable[] array = landAudioTables;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SpawnAndPlayOneShot(base.transform.position);
			}
			if (vibrate)
			{
				VibrationManager.PlayVibrationClipOneShot(bobVibration, null);
			}
		}
		if ((bool)dustParticle)
		{
			dustParticle.Play();
		}
		if (runningTimers == null)
		{
			runningTimers = new List<PartTimer>(parts.Count);
		}
		else
		{
			foreach (PartTimer runningTimer in runningTimers)
			{
				runningTimer.Part.SetLocalPositionY(runningTimer.InitialYPos);
			}
			runningTimers.Clear();
		}
		foreach (LiftPart part in parts)
		{
			if ((bool)part.Transform)
			{
				runningTimers.Add(new PartTimer
				{
					DelayLeft = part.Delay.GetRandomValue(),
					Part = part.Transform,
					InitialYPos = part.Transform.localPosition.y,
					Magnitude = part.Magnitude.GetRandomValue()
				});
			}
		}
		OnBob.Invoke();
	}

	public void SetActive(bool active)
	{
		isDisabled = !active;
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider);
	}
}
