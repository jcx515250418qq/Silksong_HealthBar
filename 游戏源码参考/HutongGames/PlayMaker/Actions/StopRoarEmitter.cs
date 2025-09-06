using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class StopRoarEmitter : FsmStateAction
	{
		public FsmFloat delay;

		private float timer;

		private bool didRoar;

		public override void Reset()
		{
			delay = null;
			didRoar = false;
			timer = 0f;
		}

		public override void OnEnter()
		{
			if (delay.Value < 0.001f)
			{
				StopRoar();
			}
		}

		public override void OnUpdate()
		{
			if (!didRoar)
			{
				if (timer < delay.Value)
				{
					timer += Time.deltaTime;
				}
				if (timer >= delay.Value)
				{
					StopRoar();
				}
			}
		}

		private void StopRoar()
		{
			StopEmitter("Roar Wave Emitter");
			StopEmitter("Roar Wave Emitter Small");
			FSMUtility.SendEventToGameObject(GameManager.instance.hero_ctrl.gameObject, "ROAR EXIT");
			didRoar = true;
			Finish();
		}

		private void StopEmitter(string name)
		{
			FSMUtility.SendEventToGameObject(GameManager.instance.gameCams.gameObject.transform.Find(name).gameObject, "END");
		}
	}
}
