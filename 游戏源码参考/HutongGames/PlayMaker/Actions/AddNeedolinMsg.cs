namespace HutongGames.PlayMaker.Actions
{
	public class AddNeedolinMsg : FsmStateAction
	{
		[ObjectType(typeof(LocalisedTextCollection))]
		public FsmObject Msg;

		[UIHint(UIHint.Variable)]
		public FsmBool DidAddTracker;

		public override void Reset()
		{
			Msg = null;
			DidAddTracker = null;
		}

		public override void OnEnter()
		{
			if (!DidAddTracker.IsNone)
			{
				if (DidAddTracker.Value)
				{
					Finish();
					return;
				}
				DidAddTracker.Value = true;
			}
			LocalisedTextCollection localisedTextCollection = Msg.Value as LocalisedTextCollection;
			if ((bool)localisedTextCollection)
			{
				NeedolinMsgBox.AddText(localisedTextCollection);
			}
			Finish();
		}
	}
}
