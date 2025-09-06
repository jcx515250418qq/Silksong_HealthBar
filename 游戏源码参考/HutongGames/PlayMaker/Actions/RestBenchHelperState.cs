using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RestBenchHelperState : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RestBenchHelper))]
		public FsmOwnerDefault Target;

		public FsmBool HeroOnBench;

		public override void Reset()
		{
			Target = null;
			HeroOnBench = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if (safe != null)
			{
				RestBenchHelper restBenchHelper = safe.AddComponentIfNotPresent<RestBenchHelper>();
				if (restBenchHelper != null)
				{
					restBenchHelper.SetOnBench(HeroOnBench.Value);
				}
			}
			Finish();
		}
	}
}
