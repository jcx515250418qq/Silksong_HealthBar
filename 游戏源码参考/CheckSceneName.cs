using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class CheckSceneName : FsmStateAction
{
	[RequiredField]
	public FsmString sceneName;

	public FsmEvent equalEvent;

	public FsmEvent notEqualEvent;

	private int sceneNameHash;

	public override void Reset()
	{
		sceneName = null;
		equalEvent = null;
		notEqualEvent = null;
	}

	public override void Awake()
	{
		sceneNameHash = sceneName.Value.GetHashCode();
	}

	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			bool flag = (sceneName.UsesVariable ? (sceneName.Value == instance.GetSceneNameString()) : (sceneNameHash == instance.sceneNameHash));
			base.Fsm.Event(flag ? equalEvent : notEqualEvent);
		}
		Finish();
	}
}
