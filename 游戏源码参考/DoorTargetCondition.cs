using UnityEngine;

public class DoorTargetCondition : MonoBehaviour
{
	[SerializeField]
	[InspectorValidation]
	private TransitionPoint door;

	[SerializeField]
	private PlayerDataTest condition;

	[SerializeField]
	private string targetIfTrue;

	[SerializeField]
	private string targetIfFalse;

	private void Reset()
	{
		door = GetComponent<TransitionPoint>();
	}

	private void Start()
	{
		door.SetTargetScene(condition.IsFulfilled ? targetIfTrue : targetIfFalse);
	}
}
