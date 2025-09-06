using HutongGames.PlayMaker;
using UnityEngine;

public sealed class HudCanvas : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM targetFsm;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	[InspectorValidation("IsFsmBoolValid")]
	private string visibilityBool = "Is Visible";

	private static HudCanvas instance;

	private static FsmBool fsmBool;

	public static bool IsVisible
	{
		get
		{
			if (fsmBool != null)
			{
				return fsmBool.Value;
			}
			return true;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			if ((bool)targetFsm)
			{
				fsmBool = GetFsmBool();
			}
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
			fsmBool = null;
		}
	}

	private bool? IsFsmBoolValid(string boolName)
	{
		return GetFsmBool(boolName) != null;
	}

	private FsmBool GetFsmBool(string boolName)
	{
		if (!targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return targetFsm.FsmVariables.FindFsmBool(boolName);
	}

	private FsmBool GetFsmBool()
	{
		return GetFsmBool(visibilityBool);
	}
}
