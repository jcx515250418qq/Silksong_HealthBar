using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class TakeSilkDelayed : FsmStateAction
	{
		public FsmInt Amount;

		public FsmFloat Delay;

		[ObjectType(typeof(SilkSpool.SilkTakeSource))]
		public FsmEnum TakeSource;

		private float timer;

		public override void Reset()
		{
			Amount = null;
			Delay = null;
			TakeSource = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
			if (Delay.Value <= 0f)
			{
				TakeSilk();
			}
		}

		public override void OnUpdate()
		{
			timer += Time.deltaTime;
			if (timer >= Delay.Value)
			{
				TakeSilk();
			}
		}

		public void TakeSilk()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.TakeSilk(Amount.Value, (SilkSpool.SilkTakeSource)(object)TakeSource.Value);
			}
			Finish();
		}
	}
}
