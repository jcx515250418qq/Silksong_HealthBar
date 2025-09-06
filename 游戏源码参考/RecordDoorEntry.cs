using TeamCherry.SharedUtils;
using UnityEngine;

public class RecordDoorEntry : MonoBehaviour
{
	[SerializeField]
	[InspectorValidation]
	private TransitionPoint door;

	[SerializeField]
	[PlayerDataField(typeof(string), true)]
	private string pdFromSceneName;

	[SerializeField]
	private bool isMazeEntrance;

	private void Reset()
	{
		door = GetComponent<TransitionPoint>();
	}

	private void Awake()
	{
		door.OnBeforeTransition += delegate
		{
			PlayerData instance = PlayerData.instance;
			instance.SetVariable(pdFromSceneName, base.gameObject.scene.name);
			if (isMazeEntrance)
			{
				instance.PreviousMazeScene = base.gameObject.scene.name;
				instance.PreviousMazeDoor = door.gameObject.name;
				instance.PreviousMazeTargetDoor = door.entryPoint;
			}
		};
	}
}
