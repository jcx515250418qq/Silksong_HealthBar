using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public class FlingChildrenOnStart : MonoBehaviour
{
	[SerializeField]
	private FlingUtils.ChildrenConfig config = new FlingUtils.ChildrenConfig
	{
		SpeedMin = 15f,
		SpeedMax = 20f,
		AngleMin = 60f,
		AngleMax = 120f
	};

	[SerializeField]
	private MinMaxFloat randomiseZLocal = new MinMaxFloat(0f, 0.001f);

	private bool hasStarted;

	private bool checkedParentParent;

	private Transform parentParent;

	private void Start()
	{
		DoFling();
		hasStarted = true;
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			DoFling();
		}
	}

	public Transform GetParentParent()
	{
		if (checkedParentParent)
		{
			return parentParent;
		}
		if (!config.Parent)
		{
			config.Parent = base.gameObject;
		}
		if (!checkedParentParent)
		{
			checkedParentParent = true;
			parentParent = config.Parent.transform.parent;
		}
		return parentParent;
	}

	private void DoFling()
	{
		if (!config.Parent)
		{
			config.Parent = base.gameObject;
		}
		if (!checkedParentParent)
		{
			checkedParentParent = true;
			parentParent = config.Parent.transform.parent;
		}
		config.Parent.transform.SetParent(null, worldPositionStays: true);
		if (Math.Abs(randomiseZLocal.Start) > Mathf.Epsilon || Math.Abs(randomiseZLocal.End) > Mathf.Epsilon)
		{
			FlingUtils.FlingChildren(config, config.Parent.transform, Vector3.zero, randomiseZLocal);
		}
		else
		{
			FlingUtils.FlingChildren(config, config.Parent.transform, Vector3.zero, null);
		}
	}
}
