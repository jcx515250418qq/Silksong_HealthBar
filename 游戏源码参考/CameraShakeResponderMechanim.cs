using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class CameraShakeResponderMechanim : MonoBehaviour
{
	[Serializable]
	private class ShakeEvent
	{
		public AnimatorHashCache shakeAnim = new AnimatorHashCache("shake");

		public bool randomiseShakeOffset;

		public AnimatorHashCache idleAnim = new AnimatorHashCache("idle");

		public bool randomiseIdleOffset;

		public void PlayAnimation(Animator animator)
		{
			if (randomiseShakeOffset)
			{
				animator.Play(shakeAnim.Hash, 0, UnityEngine.Random.Range(0f, 1f));
			}
			else
			{
				animator.Play(shakeAnim.Hash);
			}
		}

		public void StopAnimation(Animator animator)
		{
			if (randomiseIdleOffset)
			{
				animator.Play(idleAnim.Hash, 0, UnityEngine.Random.Range(0f, 1f));
			}
			else
			{
				animator.Play(idleAnim.Hash);
			}
		}

		public void OnValidate()
		{
			shakeAnim.Dirty();
			idleAnim.Dirty();
		}
	}

	[Serializable]
	private enum ShakeEventType
	{
		Shaked = 0,
		Shaking = 1
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private List<ShakeEvent> shakeEvents = new List<ShakeEvent>();

	[SerializeField]
	private ShakeEventType shakeEventType;

	[Header("Shake Settings")]
	[SerializeField]
	private MinMaxFloat minDuration = new MinMaxFloat(0.25f, 0.3f);

	[SerializeField]
	private MinMaxFloat duration = new MinMaxFloat(0.025f, 0.075f);

	[SerializeField]
	private float radius = 20f;

	[Space]
	[SerializeField]
	private CameraShakeWorldForceIntensities minIntensity = CameraShakeWorldForceIntensities.Medium;

	[SerializeField]
	private CameraShakeWorldForceIntensities maxIntensity = CameraShakeWorldForceIntensities.Intense;

	private bool registeredEvent;

	private float timer;

	private float minTimer;

	private ShakeEvent currentShakeEvent;

	private bool calculatedRange;

	private CameraShakeWorldForceFlag validRange;

	private void Awake()
	{
		shakeEvents.RemoveAll((ShakeEvent o) => o == null);
		if (animator == null)
		{
			animator = GetComponent<Animator>();
			if (animator == null)
			{
				Debug.LogError($"{this} is missing an Animator component.", this);
				base.enabled = false;
			}
		}
	}

	private void OnValidate()
	{
		foreach (ShakeEvent shakeEvent in shakeEvents)
		{
			shakeEvent?.OnValidate();
		}
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
		calculatedRange = false;
	}

	private void LateUpdate()
	{
		if (!(timer > 0f))
		{
			return;
		}
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			if (animator != null)
			{
				currentShakeEvent?.StopAnimation(animator);
			}
			currentShakeEvent = null;
		}
	}

	private void OnEnable()
	{
		RegisterEvents();
	}

	private void OnDisable()
	{
		UnregisterEvents();
	}

	private void RegisterEvents()
	{
		if (!registeredEvent)
		{
			registeredEvent = true;
			switch (shakeEventType)
			{
			case ShakeEventType.Shaked:
				GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce += OnCameraShaked;
				break;
			case ShakeEventType.Shaking:
				GlobalSettings.Camera.MainCameraShakeManager.CameraShakingWorldForce += OnCameraShaking;
				break;
			}
		}
	}

	private void UnregisterEvents()
	{
		if (registeredEvent)
		{
			registeredEvent = false;
			switch (shakeEventType)
			{
			case ShakeEventType.Shaked:
				GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce -= OnCameraShaked;
				break;
			case ShakeEventType.Shaking:
				GlobalSettings.Camera.MainCameraShakeManager.CameraShakingWorldForce -= OnCameraShaking;
				break;
			}
		}
	}

	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (intensity >= minIntensity && intensity <= maxIntensity)
		{
			TryDoShake(cameraPosition);
		}
	}

	private void TryDoShake(Vector2 cameraPosition)
	{
		if (shakeEvents.Count == 0 || (radius > 0f && Vector2.SqrMagnitude((Vector2)base.transform.position - cameraPosition) > radius * radius))
		{
			return;
		}
		if (currentShakeEvent == null)
		{
			if (animator != null)
			{
				currentShakeEvent = shakeEvents[UnityEngine.Random.Range(0, shakeEvents.Count)];
				if (currentShakeEvent != null)
				{
					currentShakeEvent.PlayAnimation(animator);
					timer = minDuration.GetRandomValue();
				}
				else
				{
					Debug.LogError($"{this} has a null shake", this);
				}
			}
			else
			{
				base.enabled = false;
			}
		}
		else
		{
			float randomValue = duration.GetRandomValue();
			if (randomValue > timer)
			{
				timer = randomValue;
			}
		}
	}

	private void CalculateRange()
	{
		if (!calculatedRange)
		{
			validRange = CameraShakeWorldForceFlag.None;
			for (CameraShakeWorldForceIntensities cameraShakeWorldForceIntensities = minIntensity; cameraShakeWorldForceIntensities <= maxIntensity; cameraShakeWorldForceIntensities++)
			{
				validRange |= cameraShakeWorldForceIntensities.ToFlagMax();
			}
		}
	}

	private void OnCameraShaking(Vector2 cameraPosition, CameraShakeWorldForceFlag intensity)
	{
		CalculateRange();
		if ((intensity & validRange) != 0)
		{
			TryDoShake(cameraPosition);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
