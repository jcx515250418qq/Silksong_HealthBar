using HutongGames.PlayMaker;

public class CheckHasVisitedScene : FSMUtility.CheckFsmStateAction
{
	[RequiredField]
	public FsmString SceneName;

	public override bool IsTrue
	{
		get
		{
			if (string.IsNullOrEmpty(SceneName.Value))
			{
				return false;
			}
			return PlayerData.instance.scenesVisited.Contains(SceneName.Value);
		}
	}

	public override void Reset()
	{
		base.Reset();
		SceneName = null;
	}
}
