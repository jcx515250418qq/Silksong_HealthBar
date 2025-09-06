using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimelineGate : UnlockablePropBase
{
	[SerializeField]
	private PlayableDirector timeline;

	[SerializeField]
	private bool deactivateIfOpened;

	[SerializeField]
	private MinMaxFloat openDelay;

	[Space]
	[SerializeField]
	private UnityEvent onBeforeDelay;

	[SerializeField]
	private UnityEvent onOpen;

	[SerializeField]
	private UnityEvent onOpened;

	private bool activated;

	private void Start()
	{
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if ((bool)component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = activated;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				activated = value;
				if (activated)
				{
					Opened();
				}
			};
		}
		timeline.Stop();
		timeline.time = 0.0;
		timeline.Evaluate();
	}

	public override void Open()
	{
		if (!activated)
		{
			activated = true;
			float randomValue = openDelay.GetRandomValue();
			if (randomValue <= 0f)
			{
				onBeforeDelay.Invoke();
				DoOpen();
			}
			else
			{
				onBeforeDelay.Invoke();
				this.ExecuteDelayed(randomValue, DoOpen);
			}
		}
	}

	public override void Opened()
	{
		onOpened.Invoke();
		if (deactivateIfOpened)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		timeline.Stop();
		timeline.time = timeline.duration;
		timeline.Evaluate();
	}

	private void DoOpen()
	{
		onOpen.Invoke();
		timeline.Play();
	}
}
