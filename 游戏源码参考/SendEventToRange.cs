using HutongGames.PlayMaker;
using UnityEngine;

public class SendEventToRange : FsmStateAction
{
	[CheckForComponent(typeof(PlayMakerEventRange))]
	public FsmOwnerDefault Range;

	public FsmString EventName;

	public FsmBool ExcludeThis;

	public override void Reset()
	{
		Range = null;
		EventName = null;
		ExcludeThis = null;
	}

	public override void OnEnter()
	{
		GameObject safe = Range.GetSafe(this);
		if ((bool)safe)
		{
			PlayMakerEventRange component = safe.GetComponent<PlayMakerEventRange>();
			if ((bool)component)
			{
				component.SendEvent(EventName.Value, ExcludeThis.Value);
			}
		}
		Finish();
	}
}
