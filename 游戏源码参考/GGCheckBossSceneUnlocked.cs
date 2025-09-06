using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class GGCheckBossSceneUnlocked : FSMUtility.CheckFsmStateAction
{
	[ObjectType(typeof(BossScene))]
	public FsmObject bossScene;

	public BossSceneCheckSource checkSource;

	public override bool IsTrue
	{
		get
		{
			if (!bossScene.IsNone)
			{
				return ((BossScene)bossScene.Value).IsUnlocked(checkSource);
			}
			return false;
		}
	}

	public override void Reset()
	{
		base.Reset();
		bossScene = null;
	}
}
