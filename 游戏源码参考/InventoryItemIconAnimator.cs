using UnityEngine;

public class InventoryItemIconAnimator : CustomInventoryItemCollectableDisplay
{
	private static readonly int _isSelectedProp = Animator.StringToHash("Is Selected");

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private AudioSource selectedAudio;

	[SerializeField]
	private string selectedEvent;

	[SerializeField]
	private string deselectedEvent;

	public override void OnSelect()
	{
		animator.SetBool(_isSelectedProp, value: true);
		if ((bool)selectedAudio)
		{
			selectedAudio.Play();
			selectedAudio.timeSamples = Random.Range(0, selectedAudio.clip.samples);
		}
		EventRegister.SendEvent(selectedEvent);
	}

	public override void OnDeselect()
	{
		animator.SetBool(_isSelectedProp, value: false);
		if ((bool)selectedAudio)
		{
			selectedAudio.Stop();
		}
		EventRegister.SendEvent(deselectedEvent);
	}

	public override void OnPrePaneEnd()
	{
		OnDeselect();
	}
}
