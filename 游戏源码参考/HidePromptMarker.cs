using HutongGames.PlayMaker;

public class HidePromptMarker : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmGameObject storedObject;

	public override void Reset()
	{
		storedObject = new FsmGameObject();
	}

	public override void OnEnter()
	{
		if ((bool)storedObject.Value)
		{
			PromptMarker component = storedObject.Value.GetComponent<PromptMarker>();
			if ((bool)component)
			{
				component.Hide();
				storedObject.Value = null;
			}
		}
		Finish();
	}
}
