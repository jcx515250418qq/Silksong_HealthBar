using System;

namespace InControl
{
	public class PlayerTwoAxisAction : TwoAxisInputControl
	{
		private PlayerAction negativeXAction;

		private PlayerAction positiveXAction;

		private PlayerAction negativeYAction;

		private PlayerAction positiveYAction;

		public BindingSourceType LastInputType;

		public bool InvertXAxis { get; set; }

		public bool InvertYAxis { get; set; }

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

		internal PlayerTwoAxisAction(PlayerAction negativeXAction, PlayerAction positiveXAction, PlayerAction negativeYAction, PlayerAction positiveYAction)
		{
			this.negativeXAction = negativeXAction;
			this.positiveXAction = positiveXAction;
			this.negativeYAction = negativeYAction;
			this.positiveYAction = positiveYAction;
			InvertXAxis = false;
			InvertYAxis = false;
			Raw = true;
		}

		internal void Update(ulong updateTick, float deltaTime)
		{
			ProcessActionUpdate(negativeXAction);
			ProcessActionUpdate(positiveXAction);
			ProcessActionUpdate(negativeYAction);
			ProcessActionUpdate(positiveYAction);
			float x = Utility.ValueFromSides(negativeXAction, positiveXAction, InvertXAxis);
			float y = Utility.ValueFromSides(negativeYAction, positiveYAction, InputManager.InvertYAxis || InvertYAxis);
			UpdateWithAxes(x, y, updateTick, deltaTime);
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
