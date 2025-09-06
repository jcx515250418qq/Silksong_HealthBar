using System;

namespace InControl
{
	public class BindingListenOptions
	{
		public bool IncludeControllers = true;

		public bool IncludeUnknownControllers;

		public bool IncludeNonStandardControls = true;

		public bool IncludeMouseButtons;

		public bool IncludeMouseScrollWheel;

		public bool IncludeKeys = true;

		public bool IncludeModifiersAsFirstClassKeys;

		public uint MaxAllowedBindings;

		public uint MaxAllowedBindingsPerType;

		public bool AllowDuplicateBindingsPerSet;

		public bool UnsetDuplicateBindingsOnSet;

		public bool RejectRedundantBindings;

		public BindingSource ReplaceBinding;

		public Func<PlayerAction, BindingSource, bool> OnBindingFound;

		public Action<PlayerAction, BindingSource> OnBindingAdded;

		public Action<PlayerAction, BindingSource, BindingSourceRejectionType> OnBindingRejected;

		public Action<PlayerAction> OnBindingEnded;

		public bool CallOnBindingFound(PlayerAction playerAction, BindingSource bindingSource)
		{
			if (OnBindingFound != null)
			{
				return OnBindingFound(playerAction, bindingSource);
			}
			return true;
		}

		public void CallOnBindingAdded(PlayerAction playerAction, BindingSource bindingSource)
		{
			if (OnBindingAdded != null)
			{
				OnBindingAdded(playerAction, bindingSource);
			}
		}

		public void CallOnBindingRejected(PlayerAction playerAction, BindingSource bindingSource, BindingSourceRejectionType bindingSourceRejectionType)
		{
			if (OnBindingRejected != null)
			{
				OnBindingRejected(playerAction, bindingSource, bindingSourceRejectionType);
			}
		}

		public void CallOnBindingEnded(PlayerAction playerAction)
		{
			if (OnBindingEnded != null)
			{
				OnBindingEnded(playerAction);
			}
		}
	}
}
