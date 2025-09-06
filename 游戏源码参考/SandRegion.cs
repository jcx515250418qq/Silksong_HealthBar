using HutongGames.PlayMaker;
using UnityEngine;

public class SandRegion : DebugDrawColliderRuntimeAdder
{
	[SerializeField]
	private bool breakCollisionBreakables = true;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (breakCollisionBreakables)
		{
			SandRegionCollisionBreakable component = other.GetComponent<SandRegionCollisionBreakable>();
			if (component != null)
			{
				component.AddSandRegion(this);
			}
		}
		FsmBool inSandBool = GetInSandBool(other);
		if (inSandBool != null)
		{
			inSandBool.Value = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (breakCollisionBreakables)
		{
			SandRegionCollisionBreakable component = other.GetComponent<SandRegionCollisionBreakable>();
			if (component != null)
			{
				component.RemoveSandRegion(this);
			}
		}
		FsmBool inSandBool = GetInSandBool(other);
		if (inSandBool != null)
		{
			inSandBool.Value = false;
		}
	}

	private static FsmBool GetInSandBool(Component other)
	{
		PlayMakerFSM component = other.GetComponent<PlayMakerFSM>();
		if (!component)
		{
			return null;
		}
		return component.FsmVariables.FindFsmBool("In Sand");
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.SandRegion);
	}
}
