using UnityEngine;

public abstract class SkippableSequence : MonoBehaviour
{
	[SerializeField]
	private PlayerDataTest condition;

	[Space]
	[SerializeField]
	private bool forceFullResolution;

	[SerializeField]
	private bool canSkip = true;

	[SerializeField]
	private bool waitForSkip;

	private CameraRenderScaled scaler;

	public bool CanSkip
	{
		get
		{
			return canSkip;
		}
		protected set
		{
			canSkip = value;
		}
	}

	public bool WaitForSkip => waitForSkip;

	public virtual bool ShouldShow => condition.IsFulfilled;

	public abstract bool IsPlaying { get; }

	public abstract bool IsSkipped { get; }

	public abstract float FadeByController { get; set; }

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
		if (scaler == null)
		{
			scaler = GameCameras.instance.tk2dCam.GetComponent<CameraRenderScaled>();
			if (!scaler)
			{
				return;
			}
		}
		if (value)
		{
			scaler.ForceFullResolution = forceFullResolution;
		}
	}

	[ContextMenu("Allow Skip")]
	public virtual void AllowSkip()
	{
		canSkip = true;
	}

	public abstract void Begin();

	public abstract void Skip();
}
