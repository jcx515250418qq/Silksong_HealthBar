using System.Collections.Generic;
using UnityEngine;

public sealed class SandRegionCollisionBreakable : MonoBehaviour
{
	[SerializeField]
	private PlayMakerFSM fsm;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string breakEvent = "BREAK";

	private HashSet<SandRegion> sandRegions = new HashSet<SandRegion>();

	private bool isInsideSandRegion;

	private bool isBroken;

	private bool? IsFsmEventValidRequired(string eventName)
	{
		return fsm.IsEventValid(eventName, isRequired: true);
	}

	private void OnEnable()
	{
		isBroken = false;
		if (fsm == null)
		{
			Object.Destroy(this);
		}
	}

	private void OnDisable()
	{
		sandRegions.Clear();
		isInsideSandRegion = false;
	}

	public void AddSandRegion(SandRegion sandRegion)
	{
		sandRegions.Add(sandRegion);
		isInsideSandRegion = true;
	}

	public void RemoveSandRegion(SandRegion sandRegion)
	{
		if (sandRegions.Remove(sandRegion) && sandRegions.Count == 0)
		{
			isInsideSandRegion = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!isBroken && isInsideSandRegion && other.gameObject.layer == 8)
		{
			Break();
		}
	}

	private void Break()
	{
		if ((bool)fsm)
		{
			fsm.SendEvent(breakEvent);
		}
	}
}
