using HutongGames.PlayMaker;
using UnityEngine;

public class SetSpriteRendererColor : FSMUtility.GetComponentFsmStateAction<SpriteRenderer>
{
	public FsmColor TintColor;

	public override void Reset()
	{
		base.Reset();
		TintColor = null;
	}

	protected override void DoAction(SpriteRenderer spriteRenderer)
	{
		spriteRenderer.color = TintColor.Value;
	}
}
