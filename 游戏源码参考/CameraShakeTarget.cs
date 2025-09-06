using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CameraShakeTarget
{
	[SerializeField]
	[FormerlySerializedAs("Camera")]
	private CameraManagerReference camera;

	[SerializeField]
	[FormerlySerializedAs("Profile")]
	private CameraShakeProfile profile;

	[SerializeField]
	private bool doFreeze;

	[SerializeField]
	private bool vibrate = true;

	private bool cached;

	private bool isValid;

	public CameraManagerReference Camera => camera;

	public void DoShake(UnityEngine.Object source, bool shouldFreeze = true)
	{
		if (cached)
		{
			if (isValid)
			{
				camera.DoShake(profile, source, shouldFreeze && doFreeze, vibrate);
			}
		}
		else if ((bool)camera && (bool)profile)
		{
			camera.DoShake(profile, source, shouldFreeze && doFreeze, vibrate);
		}
	}

	public bool TryShake(UnityEngine.Object source, bool shouldFreeze = true)
	{
		if (cached)
		{
			if (isValid)
			{
				DoShake(source, shouldFreeze);
				return true;
			}
			return false;
		}
		if (!camera || !profile)
		{
			return false;
		}
		DoShake(source, shouldFreeze);
		return true;
	}

	public void DoShakeInRange(UnityEngine.Object source, Vector2 range, Vector2 sourcePos, bool shouldFreeze = true)
	{
		if ((bool)camera && (bool)profile)
		{
			camera.DoShakeInRange(profile, source, range, sourcePos, shouldFreeze && doFreeze, vibrate);
		}
	}

	public void CancelShake()
	{
		if (cached)
		{
			if (isValid)
			{
				camera.CancelShake(profile);
			}
		}
		else if ((bool)camera && (bool)profile)
		{
			camera.CancelShake(profile);
		}
	}

	public CameraShakeTarget Duplicate()
	{
		return (CameraShakeTarget)MemberwiseClone();
	}

	public void Cache()
	{
		cached = true;
		isValid = (bool)camera && (bool)profile;
	}
}
