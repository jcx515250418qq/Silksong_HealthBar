using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class FireGrimmBall : FsmStateAction
{
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject storedObject;

	public FsmFloat tweenY;

	public FsmFloat force;

	public override void Reset()
	{
		storedObject = null;
		tweenY = null;
		force = null;
	}

	public override void OnEnter()
	{
		if ((bool)storedObject.Value)
		{
			GrimmballControl component = storedObject.Value.GetComponent<GrimmballControl>();
			if ((bool)component)
			{
				component.TweenY = tweenY.Value;
				component.Force = force.Value;
				component.Fire();
			}
		}
		Finish();
	}
}
