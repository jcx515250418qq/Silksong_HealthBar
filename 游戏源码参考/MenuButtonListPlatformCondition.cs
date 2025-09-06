using System;
using UnityEngine;

public class MenuButtonListPlatformCondition : MenuButtonListCondition
{
	[Serializable]
	public struct PlatformBoolPair
	{
		public RuntimePlatform platform;

		public bool activate;
	}

	public PlatformBoolPair[] platforms;

	[Space]
	public bool defaultActivation = true;

	public override bool IsFulfilled()
	{
		RuntimePlatform platform = Application.platform;
		bool activate = defaultActivation;
		PlatformBoolPair[] array = platforms;
		for (int i = 0; i < array.Length; i++)
		{
			PlatformBoolPair platformBoolPair = array[i];
			if (platformBoolPair.platform == platform)
			{
				activate = platformBoolPair.activate;
				break;
			}
		}
		return activate;
	}
}
