using System;
using UnityEngine;

public abstract class SpeedChanger : MonoBehaviour
{
	public event Action<float> SpeedChanged;

	protected void CallSpeedChangedEvent(float speed)
	{
		if (this.SpeedChanged != null)
		{
			this.SpeedChanged(speed);
		}
	}
}
