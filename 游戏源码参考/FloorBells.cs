using UnityEngine;

public class FloorBells : MonoBehaviour
{
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private TrackTriggerObjects tracker;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private BasicSpriteAnimator animator;

	private void Awake()
	{
		tracker.InsideStateChanged += delegate(bool value)
		{
			if (value)
			{
				OnEntered();
			}
			else
			{
				OnExited();
			}
		};
	}

	private void OnEntered()
	{
		animator.Play();
	}

	private void OnExited()
	{
		animator.QueueStop();
	}
}
