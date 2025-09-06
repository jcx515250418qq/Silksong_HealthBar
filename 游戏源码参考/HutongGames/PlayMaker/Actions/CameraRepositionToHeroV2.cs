using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class CameraRepositionToHeroV2 : FsmStateAction
	{
		public FsmBool forceDirect;

		public FsmFloat timeOut;

		public FsmEvent onFinish;

		private CameraController cameraController;

		private float timer;

		public override void Reset()
		{
			forceDirect = null;
			timeOut = null;
		}

		public override void OnEnter()
		{
			if ((bool)GameManager.instance && (bool)GameManager.instance.cameraCtrl)
			{
				cameraController = GameManager.instance.cameraCtrl;
				cameraController.PositionToHero(forceDirect.Value);
				timer = timeOut.Value;
				cameraController.PositionedAtHero += OnPositionedAtHero;
			}
			else
			{
				DoFinish();
			}
		}

		public override void OnExit()
		{
			if (cameraController != null)
			{
				cameraController.PositionedAtHero -= OnPositionedAtHero;
				cameraController = null;
			}
		}

		private void OnPositionedAtHero()
		{
			DoFinish();
		}

		public override void OnUpdate()
		{
			if (timer > 0f)
			{
				timer -= Time.deltaTime;
				if (timer <= 0f)
				{
					DoFinish();
				}
			}
		}

		private void DoFinish()
		{
			base.Fsm.Event(onFinish);
			Finish();
		}
	}
}
