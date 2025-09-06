using UnityEngine;

public class TriggerActivateGameObject : TrackTriggerObjects
{
	[Space]
	[SerializeField]
	private bool deactivateObjectOnLoad;

	[SerializeField]
	private GameObject gameObjectToSet;

	[SerializeField]
	private PersistentBoolItem activateOncePersistent;

	[SerializeField]
	private bool invertActivation;

	private bool hasActivated;

	protected override void Awake()
	{
		base.Awake();
		if (deactivateObjectOnLoad && (bool)gameObjectToSet)
		{
			gameObjectToSet.SetActive(invertActivation);
		}
		if ((bool)activateOncePersistent)
		{
			activateOncePersistent.OnGetSaveState += delegate(out bool value)
			{
				value = hasActivated;
			};
			activateOncePersistent.OnSetSaveState += delegate(bool value)
			{
				hasActivated = value;
			};
		}
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		if ((!hasActivated || !activateOncePersistent) && isInside)
		{
			if ((bool)gameObjectToSet)
			{
				gameObjectToSet.SetActive(!invertActivation);
			}
			hasActivated = true;
		}
	}
}
