using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RendererIsVisible : ComponentAction<Renderer>
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault Target;

		[HideIf("IsNotEveryFrame")]
		public FsmFloat LostVisibilityTime;

		public FsmEvent VisibleEvent;

		public FsmEvent NotVisibleEvent;

		public bool EveryFrame;

		private float lostVisibilityTimeLeft;

		private new Renderer renderer;

		public bool IsNotEveryFrame()
		{
			return !EveryFrame;
		}

		public override void Reset()
		{
			Target = null;
			LostVisibilityTime = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe == null)
			{
				Finish();
				return;
			}
			if (UpdateCache(safe))
			{
				renderer = cachedComponent;
				lostVisibilityTimeLeft = (EveryFrame ? LostVisibilityTime.Value : 0f);
				Evaluate();
			}
			else
			{
				Finish();
			}
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			Evaluate();
		}

		private void Evaluate()
		{
			if (renderer.isVisible)
			{
				base.Fsm.Event(VisibleEvent);
				lostVisibilityTimeLeft = LostVisibilityTime.Value;
				return;
			}
			lostVisibilityTimeLeft -= Time.deltaTime;
			if (lostVisibilityTimeLeft <= 0f)
			{
				base.Fsm.Event(NotVisibleEvent);
			}
		}
	}
}
