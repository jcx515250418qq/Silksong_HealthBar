using System;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class SetBlackThreadCoreColor : FsmStateAction
	{
		[Serializable]
		private enum LerpType
		{
			TargetColor = 0,
			OriginalColor = 1
		}

		[RequiredField]
		[CheckForComponent(typeof(BlackThreadCore))]
		public FsmOwnerDefault Target;

		[ObjectType(typeof(LerpType))]
		public FsmEnum lerpType;

		[HideIf("IsOriginal")]
		public FsmColor targetColor;

		[Tooltip("Applies instantly if duration <= 0")]
		public FsmFloat duration;

		public bool IsOriginal()
		{
			return (LerpType)(object)lerpType.Value == LerpType.OriginalColor;
		}

		public override void Reset()
		{
			Target = null;
			lerpType = null;
			targetColor = null;
			duration = null;
		}

		public override void OnEnter()
		{
			BlackThreadCore safe = Target.GetSafe<BlackThreadCore>(this);
			if (safe != null)
			{
				switch ((LerpType)(object)lerpType.Value)
				{
				case LerpType.TargetColor:
					safe.LerpToColor(targetColor.Value, duration.Value);
					break;
				case LerpType.OriginalColor:
					safe.LerpToOriginal(duration.Value);
					break;
				}
			}
			Finish();
		}
	}
}
