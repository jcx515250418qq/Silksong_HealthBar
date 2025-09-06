using UnityEngine;

public class MenuButtonHideInDemoMode : MenuButtonListCondition
{
	[SerializeField]
	private bool onlyExhibitionMode;

	public override bool IsFulfilled()
	{
		if (onlyExhibitionMode)
		{
			return !DemoHelper.IsExhibitionMode;
		}
		return !DemoHelper.IsDemoMode;
	}
}
