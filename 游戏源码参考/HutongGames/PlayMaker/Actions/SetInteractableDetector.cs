namespace HutongGames.PlayMaker.Actions
{
	public sealed class SetInteractableDetector : FSMUtility.GetComponentFsmStateAction<InteractableBase>
	{
		public enum DetectorType
		{
			Enter = 0,
			Exit = 1
		}

		public FsmGameObject detector;

		public FsmBool allowNullDetector;

		[Tooltip("Optional storage of previous detector")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject previousDetector;

		protected override bool LogMissingComponent => true;

		public override void Reset()
		{
			base.Reset();
			detector = null;
			allowNullDetector = null;
			previousDetector = null;
		}

		protected override void DoAction(InteractableBase component)
		{
			TriggerEnterEvent safe = detector.GetSafe<TriggerEnterEvent>();
			if (allowNullDetector.Value || !(safe == null))
			{
				if (!previousDetector.IsNone && component.EnterDetector != null && component.EnterDetector != safe)
				{
					previousDetector.Value = component.EnterDetector.gameObject;
				}
				component.EnterDetector = safe;
			}
		}
	}
}
