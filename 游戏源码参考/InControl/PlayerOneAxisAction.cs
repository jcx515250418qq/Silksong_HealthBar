using System;

namespace InControl
{
	public class PlayerOneAxisAction : OneAxisInputControl
	{
		private PlayerAction negativeAction;

		private PlayerAction positiveAction;

		public BindingSourceType LastInputType;

		public object UserData { get; set; }

		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float LowerDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float UpperDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public event Action<BindingSourceType> OnLastInputTypeChanged;

		internal PlayerOneAxisAction(PlayerAction negativeAction, PlayerAction positiveAction)
		{
			this.negativeAction = negativeAction;
			this.positiveAction = positiveAction;
			Raw = true;
		}

		internal void Update(ulong updateTick, float deltaTime)
		{
			ProcessActionUpdate(negativeAction);
			ProcessActionUpdate(positiveAction);
			float value = Utility.ValueFromSides(negativeAction, positiveAction);
			CommitWithValue(value, updateTick, deltaTime);
		}

		private void ProcessActionUpdate(PlayerAction action)
		{
			BindingSourceType lastInputType = LastInputType;
			if (action.UpdateTick > base.UpdateTick)
			{
				base.UpdateTick = action.UpdateTick;
				lastInputType = action.LastInputType;
			}
			if (LastInputType != lastInputType)
			{
				LastInputType = lastInputType;
				if (this.OnLastInputTypeChanged != null)
				{
					this.OnLastInputTypeChanged(lastInputType);
				}
			}
		}
	}
}
