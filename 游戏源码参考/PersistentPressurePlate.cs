using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class PersistentPressurePlate : PressurePlateBase
{
	[Space]
	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	[Space]
	[SerializeField]
	private UnlockablePropBase[] connectedGates;

	public PlayMakerFSM[] fsmGates;

	[Space]
	public UnityEvent OnActivate;

	public UnityEvent OnActivated;

	private bool activated;

	protected override bool CanDepress => !activated;

	private void Start()
	{
		if (!string.IsNullOrEmpty(playerDataBool))
		{
			activated = PlayerData.instance.GetVariable<bool>(playerDataBool);
			if (activated)
			{
				StartActivated();
			}
		}
		else
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
					StartActivated();
				}
			};
		}
	}

	private void StartActivated()
	{
		SetDepressed();
		UnlockablePropBase[] array = connectedGates;
		foreach (UnlockablePropBase unlockablePropBase in array)
		{
			if ((bool)unlockablePropBase)
			{
				unlockablePropBase.Opened();
			}
		}
		PlayMakerFSM[] array2 = fsmGates;
		foreach (PlayMakerFSM playMakerFSM in array2)
		{
			if ((bool)playMakerFSM)
			{
				playMakerFSM.SendEvent("ACTIVATE");
			}
		}
		OnActivated.Invoke();
	}

	protected override void Activate()
	{
		activated = true;
		if ((bool)persistent)
		{
			persistent.SaveState();
		}
		UnlockablePropBase[] array = connectedGates;
		foreach (UnlockablePropBase unlockablePropBase in array)
		{
			if ((bool)unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
		PlayMakerFSM[] array2 = fsmGates;
		foreach (PlayMakerFSM playMakerFSM in array2)
		{
			if ((bool)playMakerFSM)
			{
				playMakerFSM.SendEvent("OPEN");
			}
		}
		if (!string.IsNullOrEmpty(playerDataBool))
		{
			PlayerData.instance.SetVariable(playerDataBool, value: true);
		}
		OnActivate.Invoke();
	}
}
