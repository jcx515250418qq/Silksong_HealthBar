using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class EnviroRegionListener : MonoBehaviour
{
	public Action<EnvironmentTypes> CurrentEnvironmentTypeChanged;

	private readonly List<EnviroRegion> insideRegions = new List<EnviroRegion>();

	private HeroController hc;

	private bool overrideEnvironment;

	private EnvironmentTypes overrideEnvironmentType;

	private EnvironmentTypes currentEnvironmentType;

	public bool IsSprinting
	{
		get
		{
			if ((bool)hc)
			{
				return hc.cState.isSprinting;
			}
			return true;
		}
	}

	public EnvironmentTypes CurrentEnvironmentType
	{
		get
		{
			if (!overrideEnvironment)
			{
				return currentEnvironmentType;
			}
			return overrideEnvironmentType;
		}
	}

	public string CurrentEnvironmentTypeString
	{
		get
		{
			if (!overrideEnvironment)
			{
				return currentEnvironmentType.ToString();
			}
			return overrideEnvironmentType.ToString();
		}
	}

	private void Awake()
	{
		hc = GetComponent<HeroController>();
	}

	private void Start()
	{
		Refresh();
	}

	public void AddInside(EnviroRegion region)
	{
		insideRegions.AddIfNotPresent(region);
		Refresh();
	}

	public void RemoveInside(EnviroRegion region)
	{
		insideRegions.Remove(region);
		Refresh();
	}

	public void Refresh(bool fixRecursion = false)
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		CustomSceneManager sm = silentInstance.sm;
		if (!sm)
		{
			silentInstance.GetSceneManager();
			sm = silentInstance.sm;
			if (!sm)
			{
				return;
			}
		}
		EnvironmentTypes environmentType = sm.environmentType;
		int num = int.MinValue;
		for (int num2 = insideRegions.Count - 1; num2 >= 0; num2--)
		{
			EnviroRegion enviroRegion = insideRegions[num2];
			if (enviroRegion.Priority > num)
			{
				num = enviroRegion.Priority;
				environmentType = enviroRegion.EnvironmentType;
			}
		}
		currentEnvironmentType = environmentType;
		if ((bool)hc)
		{
			PlayerData.instance.environmentType = environmentType;
			if (!fixRecursion)
			{
				hc.checkEnvironment();
			}
		}
		EventRegister.SendEvent(EventRegisterEvents.EnviroUpdate);
		CurrentEnvironmentTypeChanged?.Invoke(currentEnvironmentType);
	}

	public void SetOverride(EnvironmentTypes type)
	{
		overrideEnvironment = true;
		overrideEnvironmentType = type;
		Refresh();
	}

	public void ClearOverride()
	{
		overrideEnvironment = false;
		Refresh();
	}

	public string GetCurrentEnvironmentTypeString()
	{
		Refresh();
		return CurrentEnvironmentTypeString;
	}
}
