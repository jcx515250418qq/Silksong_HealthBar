using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Camera Manager Reference")]
public class CameraManagerReference : ScriptableObject
{
	private class LoopingShake
	{
		public ICameraShake Shake;

		public UnityEngine.Object Source;

		public bool Vibrate;

		public bool SendWorldForce;
	}

	public delegate void CameraShakeWorldForceDelegate(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity);

	public delegate void CameraShakingWorldForceDelegate(Vector2 cameraPosition, CameraShakeWorldForceFlag intensity);

	[NonSerialized]
	private readonly List<CameraShakeManager> shakeManagers = new List<CameraShakeManager>();

	[NonSerialized]
	private readonly List<LoopingShake> loopingShakes = new List<LoopingShake>();

	public event CameraShakeWorldForceDelegate CameraShakedWorldForce;

	public event CameraShakingWorldForceDelegate CameraShakingWorldForce;

	public void Register(CameraShakeManager manager)
	{
		if (shakeManagers.Contains(manager))
		{
			return;
		}
		shakeManagers.Add(manager);
		foreach (LoopingShake loopingShake in loopingShakes)
		{
			manager.DoShake(loopingShake.Shake, loopingShake.Source, doFreeze: false, loopingShake.Vibrate, loopingShake.SendWorldForce);
		}
	}

	public void Deregister(CameraShakeManager manager)
	{
		if (shakeManagers.Contains(manager))
		{
			shakeManagers.Remove(manager);
		}
		if (shakeManagers.Count <= 0)
		{
			loopingShakes.Clear();
		}
	}

	public void DoShake(ICameraShake shake, UnityEngine.Object source, bool doFreeze = true, bool vibrate = true, bool sendWorldForce = true)
	{
		foreach (CameraShakeManager shakeManager in shakeManagers)
		{
			shakeManager.DoShake(shake, source, doFreeze, vibrate, sendWorldForce);
		}
		if (!shake.CanFinish)
		{
			loopingShakes.Add(new LoopingShake
			{
				Shake = shake,
				Source = source,
				Vibrate = vibrate,
				SendWorldForce = sendWorldForce
			});
		}
		if (sendWorldForce)
		{
			OnDidShake(shake);
		}
	}

	public void ApplyOffsets()
	{
		foreach (CameraShakeManager shakeManager in shakeManagers)
		{
			shakeManager.ApplyOffset();
		}
	}

	private void OnDidShake(ICameraShake shake)
	{
		SendWorldForce(shake.WorldForceOnStart);
	}

	public void SendWorldForce(CameraShakeWorldForceIntensities worldForce)
	{
		if (worldForce == CameraShakeWorldForceIntensities.None)
		{
			return;
		}
		if (this.CameraShakedWorldForce != null)
		{
			foreach (CameraShakeManager shakeManager in shakeManagers)
			{
				this.CameraShakedWorldForce(shakeManager.transform.position, worldForce);
			}
		}
		SendWorldShaking(worldForce);
	}

	public void SendWorldShaking(CameraShakeWorldForceIntensities worldForce)
	{
		if (worldForce != 0)
		{
			SendWorldShaking(worldForce.ToFlag());
		}
	}

	public void SendWorldShaking(CameraShakeWorldForceFlag worldForce)
	{
		if (worldForce == CameraShakeWorldForceFlag.None || this.CameraShakingWorldForce == null)
		{
			return;
		}
		foreach (CameraShakeManager shakeManager in shakeManagers)
		{
			this.CameraShakingWorldForce(shakeManager.transform.position, worldForce);
		}
	}

	public void CancelShake(ICameraShake shake)
	{
		if (shake == null)
		{
			foreach (CameraShakeManager shakeManager in shakeManagers)
			{
				shakeManager.CancelAllShakes();
			}
			loopingShakes.Clear();
			return;
		}
		foreach (CameraShakeManager shakeManager2 in shakeManagers)
		{
			shakeManager2.CancelShake(shake);
		}
		loopingShakes.RemoveAll((LoopingShake s) => s.Shake == shake);
	}

	public void DoShakeInRange(ICameraShake shake, UnityEngine.Object source, Vector2 range, Vector2 sourcePos, bool doFreeze = true, bool vibrate = true)
	{
		range.x = Mathf.Abs(range.x);
		range.y = Mathf.Abs(range.y);
		bool flag = false;
		foreach (CameraShakeManager shakeManager in shakeManagers)
		{
			Vector2 vector = (Vector2)shakeManager.transform.position - sourcePos;
			if ((!(range.x > 0f) || !(Mathf.Abs(vector.x) > range.x)) && (!(range.y > 0f) || !(Mathf.Abs(vector.y) > range.y)))
			{
				shakeManager.DoShake(shake, source, doFreeze, vibrate);
				flag = true;
			}
		}
		if (flag)
		{
			OnDidShake(shake);
		}
	}

	public IEnumerable<CameraShakeManager.CameraShakeTracker> EnumerateCurrentShakes()
	{
		foreach (CameraShakeManager shakeManager in shakeManagers)
		{
			foreach (CameraShakeManager.CameraShakeTracker item in shakeManager.EnumerateCurrentShakes())
			{
				yield return item;
			}
		}
	}
}
