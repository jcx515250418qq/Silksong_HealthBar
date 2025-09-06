using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
	[Serializable]
	public struct CameraShakeTracker
	{
		public ICameraShake Shake;

		public float ElapsedTime;

		public UnityEngine.Object Source;

		public StackTrace StartStackTrace;

		public bool IsVibrating;

		public VibrationEmission Emission;

		public bool PersistThroughScenes;

		public bool SendWorldForce;

		public bool IsDone => Shake.IsDone(ElapsedTime);

		public CameraShakeWorldForceFlag ShakeFlag => Shake.WorldForceOnStart.ToFlag();

		public Vector2 GetOffset()
		{
			UpdateShake();
			return Shake.GetOffset(ElapsedTime);
		}

		public void StartVibration(bool isRealtime)
		{
			if (Shake.CameraShakeVibration != null)
			{
				Emission = Shake.CameraShakeVibration.PlayVibration(isRealtime);
				IsVibrating = Emission != null;
			}
		}

		public void StopVibration()
		{
			if (IsVibrating)
			{
				IsVibrating = false;
				Emission.Stop();
			}
		}

		public void StopLoop()
		{
			if (IsVibrating)
			{
				Emission.IsLooping = false;
			}
		}

		public void UpdateShake()
		{
			if (IsVibrating)
			{
				float vibrationStrength = Shake.CameraShakeVibration.GetVibrationStrength(ElapsedTime);
				Emission.SetStrength(vibrationStrength);
			}
		}
	}

	public enum ShakeSettings
	{
		On = 0,
		Reduced = 1,
		Off = 2
	}

	private const float SHAKE_DISABLED_TIME = 0.5f;

	[SerializeField]
	private CameraManagerReference cameraTypeReference;

	[SerializeField]
	private Transform overrideTransform;

	[SerializeField]
	private bool isRealtime;

	private Vector3 initialPosition;

	private bool wasOffsetApplied;

	private readonly List<CameraShakeTracker> currentShakes = new List<CameraShakeTracker>();

	private Coroutine evaluateShakesRoutine;

	private Vector3 currentOffset;

	private Coroutine freezeFrameRoutine;

	private Action onFreezeEnd;

	private static int _freezesRunning;

	private double cameraShakeStartTime;

	public static ShakeSettings ShakeSetting { get; set; }

	public static float ShakeMultiplier => ShakeSetting switch
	{
		ShakeSettings.On => 1f, 
		ShakeSettings.Reduced => ConfigManager.ReducedCameraShake, 
		ShakeSettings.Off => 0f, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	private Transform CurrentTransform
	{
		get
		{
			if (!overrideTransform)
			{
				return base.transform;
			}
			return overrideTransform;
		}
	}

	private void OnEnable()
	{
		evaluateShakesRoutine = StartCoroutine(EvaluateShakesTimed());
		if ((bool)cameraTypeReference)
		{
			cameraTypeReference.Register(this);
		}
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			instance.NextSceneWillActivate += CancelAllShakes;
		}
		CancelAllShakes();
	}

	private void OnDisable()
	{
		if ((bool)cameraTypeReference)
		{
			cameraTypeReference.Deregister(this);
		}
		StopCoroutine(evaluateShakesRoutine);
		currentOffset = Vector3.zero;
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if ((bool)unsafeInstance)
		{
			unsafeInstance.NextSceneWillActivate -= CancelAllShakes;
		}
		CancelAllShakes();
	}

	private IEnumerator EvaluateShakesTimed()
	{
		double lastEvalTime = (isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble);
		WaitForSeconds wait = new WaitForSeconds(1f / 60f);
		WaitForSecondsRealtime realtimeWait = new WaitForSecondsRealtime(1f / 60f);
		while (true)
		{
			double num = (isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble);
			float timeSinceLastEvaluation = (float)(num - lastEvalTime);
			currentOffset = EvaluateShakes(timeSinceLastEvaluation);
			lastEvalTime = num;
			if (isRealtime)
			{
				yield return realtimeWait;
			}
			else
			{
				yield return wait;
			}
		}
	}

	private void OnPreCull()
	{
		if (isRealtime || !(Time.timeScale <= Mathf.Epsilon))
		{
			if (wasOffsetApplied)
			{
				ClearOffset();
			}
			initialPosition = CurrentTransform.localPosition;
			ApplyOffset();
		}
	}

	private void OnPostRender()
	{
		if (isRealtime || !(Time.timeScale <= Mathf.Epsilon))
		{
			ClearOffset();
		}
	}

	public void ApplyOffset()
	{
		if (freezeFrameRoutine == null)
		{
			CurrentTransform.localPosition += currentOffset;
		}
		wasOffsetApplied = true;
	}

	private void ClearOffset()
	{
		CurrentTransform.localPosition = initialPosition;
		wasOffsetApplied = false;
	}

	public void DoShake(ICameraShake shake, UnityEngine.Object source, bool doFreeze = true, bool vibrate = true, bool sendWorldForce = true)
	{
		if (shake == null || ((isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble) < cameraShakeStartTime && shake.CanFinish))
		{
			return;
		}
		bool flag = false;
		foreach (CameraShakeTracker currentShake in currentShakes)
		{
			if (currentShake.Shake == shake)
			{
				flag = true;
				break;
			}
		}
		if (!shake.CanFinish && flag)
		{
			return;
		}
		CameraShakeTracker cameraShakeTracker = default(CameraShakeTracker);
		cameraShakeTracker.Shake = shake;
		cameraShakeTracker.Source = source;
		cameraShakeTracker.StartStackTrace = (CheatManager.IsStackTracesEnabled ? new StackTrace() : null);
		cameraShakeTracker.PersistThroughScenes = shake.PersistThroughScenes;
		cameraShakeTracker.SendWorldForce = sendWorldForce;
		CameraShakeTracker item = cameraShakeTracker;
		if (vibrate)
		{
			item.StartVibration(isRealtime);
		}
		currentShakes.Add(item);
		if (!doFreeze || shake.FreezeFrames <= 0)
		{
			return;
		}
		if (freezeFrameRoutine != null)
		{
			StopCoroutine(freezeFrameRoutine);
			if (onFreezeEnd != null)
			{
				onFreezeEnd();
			}
		}
		freezeFrameRoutine = StartCoroutine(FreezeFrames(shake.FreezeFrames));
	}

	public void CancelShake(ICameraShake profile)
	{
		for (int num = currentShakes.Count - 1; num >= 0; num--)
		{
			if (currentShakes[num].Shake == profile)
			{
				currentShakes[num].StopVibration();
				currentShakes.RemoveAt(num);
			}
		}
	}

	public void CancelAllShakes()
	{
		for (int num = currentShakes.Count - 1; num >= 0; num--)
		{
			CameraShakeTracker cameraShakeTracker = currentShakes[num];
			if (!cameraShakeTracker.PersistThroughScenes)
			{
				cameraShakeTracker.StopVibration();
				currentShakes.RemoveAt(num);
			}
		}
		double num2 = (isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble);
		cameraShakeStartTime = num2 + 0.5;
	}

	private Vector2 EvaluateShakes(float timeSinceLastEvaluation)
	{
		if ((isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble) < cameraShakeStartTime)
		{
			return Vector2.zero;
		}
		Vector2 vector = Vector2.zero;
		CameraShakeWorldForceFlag cameraShakeWorldForceFlag = CameraShakeWorldForceFlag.None;
		float num = 0f;
		for (int i = 0; i < currentShakes.Count; i++)
		{
			CameraShakeTracker value = currentShakes[i];
			value.ElapsedTime += timeSinceLastEvaluation;
			vector += value.GetOffset();
			currentShakes[i] = value;
			if (value.SendWorldForce)
			{
				cameraShakeWorldForceFlag |= value.ShakeFlag;
			}
			float magnitude = value.Shake.Magnitude;
			if (magnitude > num)
			{
				num = magnitude;
			}
		}
		if (vector.magnitude > num)
		{
			vector = vector.normalized * num;
		}
		if (cameraShakeWorldForceFlag != 0)
		{
			cameraTypeReference.SendWorldShaking(cameraShakeWorldForceFlag);
		}
		for (int num2 = currentShakes.Count - 1; num2 >= 0; num2--)
		{
			CameraShakeTracker cameraShakeTracker = currentShakes[num2];
			if (cameraShakeTracker.IsDone)
			{
				cameraShakeTracker.StopLoop();
				currentShakes.RemoveAt(num2);
			}
		}
		return vector * ShakeMultiplier;
	}

	private IEnumerator FreezeFrames(int frameCount)
	{
		if (_freezesRunning <= 0)
		{
			TimeManager.CameraShakeTimeScale = 0f;
		}
		_freezesRunning++;
		onFreezeEnd = delegate
		{
			_freezesRunning--;
			if (_freezesRunning <= 0)
			{
				TimeManager.CameraShakeTimeScale = 1f;
			}
		};
		yield return new WaitForSecondsRealtime(1f / 60f * (float)frameCount);
		onFreezeEnd();
		onFreezeEnd = null;
		freezeFrameRoutine = null;
	}

	public IEnumerable<CameraShakeTracker> EnumerateCurrentShakes()
	{
		foreach (CameraShakeTracker currentShake in currentShakes)
		{
			yield return currentShake;
		}
	}

	public void CopyTo(GameObject other)
	{
		CameraShakeManager cameraShakeManager = other.GetComponent<CameraShakeManager>();
		if (!cameraShakeManager)
		{
			cameraShakeManager = other.AddComponent<CameraShakeManager>();
		}
		if ((bool)cameraShakeManager.cameraTypeReference)
		{
			cameraShakeManager.cameraTypeReference.Deregister(cameraShakeManager);
		}
		cameraShakeManager.cameraTypeReference = cameraTypeReference;
		if ((bool)cameraShakeManager.cameraTypeReference)
		{
			cameraShakeManager.cameraTypeReference.Register(cameraShakeManager);
		}
	}
}
