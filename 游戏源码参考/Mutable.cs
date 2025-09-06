using UnityEngine;

public abstract class Mutable : MonoBehaviour, IMutable
{
	[SerializeField]
	private bool muted;

	public bool Muted => muted;

	protected virtual void Start()
	{
		OnMuteStateChanged(muted);
	}

	public void SetMute(bool muted)
	{
		if (this.muted != muted)
		{
			this.muted = muted;
			OnMuteStateChanged(muted);
		}
	}

	public abstract void OnMuteStateChanged(bool muted);
}
