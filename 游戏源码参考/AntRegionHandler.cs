using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;

public class AntRegionHandler : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM pickedUpBoolFsm;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("pickedUpBoolFsm", true, false, false)]
	[InspectorValidation("IsPickedUpBoolValid")]
	private string pickedUpBoolName;

	[UsedImplicitly]
	private bool? IsPickedUpBoolValid(string boolName)
	{
		if (!pickedUpBoolFsm)
		{
			return null;
		}
		return pickedUpBoolFsm.FsmVariables.FindFsmBool(boolName) != null;
	}

	public void SetPickedUp(bool value)
	{
		if ((bool)pickedUpBoolFsm)
		{
			FsmBool fsmBool = pickedUpBoolFsm.FsmVariables.FindFsmBool(pickedUpBoolName);
			if (fsmBool != null)
			{
				fsmBool.Value = value;
			}
		}
	}
}
