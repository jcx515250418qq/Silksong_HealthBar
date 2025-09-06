using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

public class DarknessRegion : TrackTriggerObjects, ISceneLintUpgrader
{
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShouldHideDarknessLevel", false, true, false)]
	private int darknessLevel;

	[SerializeField]
	private int priority;

	private static readonly List<DarknessRegion> _insideRegions = new List<DarknessRegion>();

	private bool ShouldHideDarknessLevel()
	{
		return FSMUtility.LocateFSM(base.gameObject, "Darkness Region");
	}

	protected override void Awake()
	{
		base.Awake();
		OnSceneLintUpgrade(doUpgrade: true);
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		_insideRegions.Remove(this);
		if (isInside)
		{
			_insideRegions.Add(this);
		}
		int num;
		if (_insideRegions.Count == 0)
		{
			num = (silentInstance ? silentInstance.sm.darknessLevel : 0);
		}
		else
		{
			List<DarknessRegion> insideRegions = _insideRegions;
			DarknessRegion darknessRegion = insideRegions[insideRegions.Count - 1];
			for (int num2 = _insideRegions.Count - 1; num2 >= 0; num2--)
			{
				DarknessRegion darknessRegion2 = _insideRegions[num2];
				if (darknessRegion.priority > darknessRegion2.priority)
				{
					return;
				}
				darknessRegion = darknessRegion2;
			}
			num = darknessRegion.darknessLevel;
		}
		SetDarknessLevel(num);
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Darkness Region");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "Darkness Region FSM needs upgrading to DarknessRegion script";
		}
		FsmInt fsmInt = playMakerFSM.FsmVariables.FindFsmInt("Darkness");
		darknessLevel = fsmInt.Value;
		Object.DestroyImmediate(playMakerFSM);
		return "Darkness Region FSM was upgraded to DarknessRegion script";
	}

	public static int GetDarknessLevel()
	{
		return HeroController.instance.vignetteFSM.FsmVariables.GetFsmInt("Darkness Level").Value;
	}

	public static void SetDarknessLevel(int darknessLevel)
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			PlayMakerFSM vignetteFSM = silentInstance.vignetteFSM;
			FsmInt fsmInt = vignetteFSM.FsmVariables.GetFsmInt("Darkness Level");
			if (fsmInt.Value != darknessLevel)
			{
				silentInstance.SetDarkness(darknessLevel);
				fsmInt.Value = darknessLevel;
				vignetteFSM.SendEvent("SCENE RESET");
			}
		}
	}
}
