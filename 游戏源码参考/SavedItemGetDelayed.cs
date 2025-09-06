using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SavedItemGetDelayed : FsmStateAction
	{
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;

		public FsmFloat Delay;

		private float timer;

		public override void Reset()
		{
			Item = null;
			Delay = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
			if (Delay.Value <= 0f)
			{
				DoGet();
			}
		}

		public override void OnUpdate()
		{
			timer += Time.deltaTime;
			if (timer >= Delay.Value)
			{
				DoGet();
			}
		}

		private void DoGet()
		{
			SavedItem savedItem = Item.Value as SavedItem;
			if (savedItem != null)
			{
				savedItem.Get();
			}
			Finish();
		}

		public override float GetProgress()
		{
			if (Delay.Value <= 0f)
			{
				return 0f;
			}
			return timer / Delay.Value;
		}
	}
}
