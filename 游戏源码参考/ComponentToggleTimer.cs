using System.Collections;
using UnityEngine;

public class ComponentToggleTimer : MonoBehaviour
{
	[SerializeField]
	private Behaviour component;

	[SerializeField]
	private bool initialState;

	[SerializeField]
	private bool isPhysics;

	[SerializeField]
	private float betweenDelay;

	[SerializeField]
	private int totalCount;

	private void OnEnable()
	{
		component.enabled = initialState;
		StartCoroutine(Routine());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator Routine()
	{
		WaitForSeconds wait = new WaitForSeconds(betweenDelay);
		WaitForFixedUpdate frameWait = (isPhysics ? new WaitForFixedUpdate() : null);
		int countLeft = totalCount;
		while (countLeft > 0 || totalCount <= 0)
		{
			yield return wait;
			component.enabled = !initialState;
			yield return frameWait;
			component.enabled = initialState;
			countLeft--;
		}
	}
}
