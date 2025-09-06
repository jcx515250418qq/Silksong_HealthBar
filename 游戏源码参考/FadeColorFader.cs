using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Hollow Knight")]
public class FadeColorFader : FsmStateAction
{
	public enum FadeType
	{
		UP = 0,
		DOWN = 1
	}

	public FsmOwnerDefault target;

	[ObjectType(typeof(FadeType))]
	public FsmEnum fadeType;

	public FsmBool useChildren;

	public override void Reset()
	{
		target = null;
		fadeType = null;
		useChildren = new FsmBool(true);
	}

	public override void OnEnter()
	{
		GameObject safe = target.GetSafe(this);
		if ((bool)safe)
		{
			ColorFader[] array = ((!useChildren.Value) ? new ColorFader[1] { safe.GetComponent<ColorFader>() } : safe.GetComponentsInChildren<ColorFader>());
			ColorFader[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Fade((FadeType)(object)fadeType.Value == FadeType.UP);
			}
		}
		Finish();
	}
}
