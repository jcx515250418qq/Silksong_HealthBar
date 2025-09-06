using System.Collections.Generic;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;

public class EnviroRegion : DebugDrawColliderRuntimeAdder, ISceneLintUpgrader
{
	[SerializeField]
	private EnvironmentTypes environmentType;

	[SerializeField]
	private int priority;

	private readonly HashSet<EnviroRegionListener> insideListeners = new HashSet<EnviroRegionListener>();

	public EnvironmentTypes EnvironmentType
	{
		get
		{
			return environmentType;
		}
		set
		{
			environmentType = value;
		}
	}

	public int Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		OnSceneLintUpgrade(doUpgrade: true);
		if (base.gameObject.layer != 21)
		{
			Collider2D component = GetComponent<Collider2D>();
			component.includeLayers = (int)component.includeLayers | 0x80000;
			if (component.layerOverridePriority <= 0)
			{
				component.layerOverridePriority = 1;
			}
		}
	}

	private void OnDisable()
	{
		if (!GameManager.UnsafeInstance)
		{
			return;
		}
		foreach (EnviroRegionListener insideListener in insideListeners)
		{
			insideListener.RemoveInside(this);
		}
		insideListeners.Clear();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		EnviroRegionListener component = other.GetComponent<EnviroRegionListener>();
		if ((bool)component)
		{
			component.AddInside(this);
			insideListeners.Add(component);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		EnviroRegionListener component = other.GetComponent<EnviroRegionListener>();
		if ((bool)component)
		{
			component.RemoveInside(this);
			insideListeners.Remove(component);
		}
	}

	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Enviro Region");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "Enviro Region FSM needs upgrading to EnviroRegion script";
		}
		FsmInt fsmInt = playMakerFSM.FsmVariables.FindFsmInt("Enviro Type");
		environmentType = (EnvironmentTypes)fsmInt.Value;
		Object.DestroyImmediate(playMakerFSM);
		return "Enviro Region FSM was upgraded to EnviroRegion script";
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}
}
