using UnityEngine;

public class DeactivateSavedItemCondition : MonoBehaviour
{
	[SerializeField]
	private SavedItem item;

	private bool hasStarted;

	private void Start()
	{
		hasStarted = true;
		Evaluate();
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			Evaluate();
		}
	}

	public void Evaluate()
	{
		base.gameObject.SetActive((bool)item && item.CanGetMore());
	}
}
