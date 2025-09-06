using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ActiveRegion : MonoBehaviour
{
	private bool verboseMode;

	[Tooltip("Activate FSMs immediately or over multiple frames.")]
	[HideInInspector]
	public bool staggeredActivation = true;

	private BoxCollider2D activeRegion;

	private void OnTriggerEnter2D(Collider2D col)
	{
		FSMActivator component = col.GetComponent<FSMActivator>();
		if ((bool)component)
		{
			if (staggeredActivation)
			{
				StartCoroutine(component.ActivateStaggered());
			}
			else
			{
				component.Activate();
			}
		}
	}
}
