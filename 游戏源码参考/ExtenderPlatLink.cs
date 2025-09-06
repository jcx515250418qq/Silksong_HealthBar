using System;
using UnityEngine;
using UnityEngine.Events;

public class ExtenderPlatLink : MonoBehaviour
{
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}

	[SerializeField]
	private float initialLinkRotation;

	[SerializeField]
	private float extendedLinkRotation;

	[SerializeField]
	private AnimationCurve linkExtendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AudioEventRandom linkUnfoldSound;

	[SerializeField]
	private Transform platform;

	[SerializeField]
	private float initialPlatRotation;

	[SerializeField]
	private float extendedPlatRotation;

	[SerializeField]
	private AnimationCurve platExtendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private AudioEventRandom platUnfoldSound;

	[SerializeField]
	private float inactivePlatZ;

	[SerializeField]
	private CameraShakeTarget platActivateCamShake;

	[Space]
	public UnityBoolEvent OnSetActive;

	public UnityEvent OnActivate;

	public UnityEvent OnDeactive;

	private float activePlatZ;

	private TiltPlat tiltPlat;

	public bool IsPlatformActive
	{
		get
		{
			if ((bool)platform)
			{
				return platform.gameObject.activeSelf;
			}
			return false;
		}
	}

	private void Awake()
	{
		if (platform != null)
		{
			activePlatZ = platform.transform.localPosition.z;
		}
		tiltPlat = GetFirstTiltPlat(base.transform);
	}

	private static TiltPlat GetFirstTiltPlat(Transform transform)
	{
		TiltPlat component = transform.GetComponent<TiltPlat>();
		if ((bool)component)
		{
			return component;
		}
		foreach (Transform item in transform)
		{
			if (!item.GetComponent<ExtenderPlatLink>())
			{
				TiltPlat firstTiltPlat = GetFirstTiltPlat(item);
				if ((bool)firstTiltPlat)
				{
					return firstTiltPlat;
				}
			}
		}
		return null;
	}

	public void LinkRotationStarted()
	{
		linkUnfoldSound.SpawnAndPlayOneShot(base.transform.position);
	}

	public void UpdateLinkRotation(float time)
	{
		time = Mathf.Clamp01(time);
		time = linkExtendCurve.Evaluate(time);
		float num = Mathf.LerpUnclamped(initialLinkRotation, extendedLinkRotation, time);
		if (platform.localScale.x < 0f)
		{
			num *= -1f;
		}
		base.transform.SetLocalRotation2D(num);
	}

	public void PlatRotationStarted()
	{
		platUnfoldSound.SpawnAndPlayOneShot(base.transform.position);
	}

	public void UpdatePlatRotation(float time)
	{
		time = Mathf.Clamp01(time);
		time = platExtendCurve.Evaluate(time);
		float num = Mathf.LerpUnclamped(initialPlatRotation, extendedPlatRotation, time);
		if (platform.localScale.x < 0f)
		{
			num *= -1f;
		}
		platform.SetLocalRotation2D(num);
	}

	public void SetActive(bool value, bool isInstant)
	{
		if (platform != null)
		{
			platform.SetLocalPositionZ(value ? activePlatZ : inactivePlatZ);
		}
		OnSetActive.Invoke(value);
		if (value)
		{
			OnActivate.Invoke();
			if ((bool)tiltPlat)
			{
				tiltPlat.ActivateTiltPlat(isInstant);
			}
		}
		else
		{
			OnDeactive.Invoke();
		}
		if (!isInstant)
		{
			platActivateCamShake.DoShake(this);
		}
	}
}
