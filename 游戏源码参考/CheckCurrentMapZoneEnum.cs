using GlobalEnums;
using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CheckCurrentMapZoneEnum : FSMUtility.CheckFsmStateAction
{
	[RequiredField]
	[ObjectType(typeof(MapZone))]
	public FsmEnum MapZone;

	public override bool IsTrue
	{
		get
		{
			if ((bool)GameManager.instance)
			{
				return (MapZone)(object)MapZone.Value == GameManager.instance.GetCurrentMapZoneEnum();
			}
			return false;
		}
	}

	public override void Reset()
	{
		base.Reset();
		MapZone = null;
	}
}
