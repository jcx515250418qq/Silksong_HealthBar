using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	public class GetRandomLocalisedString : FsmStateAction
	{
		[ObjectType(typeof(LocalisedTextCollection))]
		public FsmObject Collection;

		[HideIf("IsUsingCollection")]
		public LocalisedFsmString Template;

		[UIHint(UIHint.Variable)]
		public FsmString StoreString;

		public bool IsUsingCollection()
		{
			return Collection.Value;
		}

		public override void Reset()
		{
			Collection = null;
			Template = null;
			StoreString = null;
		}

		public override void OnEnter()
		{
			LocalisedTextCollection localisedTextCollection = Collection.Value as LocalisedTextCollection;
			ILocalisedTextCollection localisedTextCollection2 = ((!localisedTextCollection) ? ((ILocalisedTextCollection)new LocalisedTextCollectionData(Template)) : ((ILocalisedTextCollection)localisedTextCollection));
			StoreString.Value = localisedTextCollection2.GetRandom(default(LocalisedString));
			Finish();
		}
	}
}
