using UnityEngine;

public abstract class SuspendedPlatformBase : MonoBehaviour
{
	[SerializeField]
	private PersistentBoolItem persistent;

	protected bool activated;

	protected virtual void Awake()
	{
		if (!persistent)
		{
			return;
		}
		persistent.OnGetSaveState += delegate(out bool value)
		{
			value = activated;
		};
		persistent.OnSetSaveState += delegate(bool value)
		{
			activated = value;
			if (activated)
			{
				OnStartActivated();
			}
		};
	}

	protected virtual void OnStartActivated()
	{
	}

	public virtual void CutDown()
	{
		activated = true;
	}
}
