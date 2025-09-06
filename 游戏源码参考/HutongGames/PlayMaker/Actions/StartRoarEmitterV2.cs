using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public sealed class StartRoarEmitterV2 : FsmStateAction
	{
		public FsmOwnerDefault spawnPoint;

		public FsmFloat delay;

		public FsmBool stunHero;

		public FsmBool roarBurst;

		public FsmBool isSmall;

		public FsmBool noVisualEffect;

		public FsmBool forceThroughBind;

		public FsmBool noRecoil;

		public bool stopOnExit;

		private float timer;

		private bool didRoar;

		private GameObject roarEmitter;

		private GameObject spawnPointGo;

		public override void Reset()
		{
			spawnPoint = null;
			delay = null;
			didRoar = false;
			noVisualEffect = false;
			isSmall = false;
			timer = 0f;
			roarBurst = null;
			stunHero = null;
			forceThroughBind = null;
		}

		public override void OnEnter()
		{
			string n = (isSmall.Value ? "Roar Wave Emitter Small" : "Roar Wave Emitter");
			roarEmitter = GameManager.instance.gameCams.gameObject.transform.Find(n).gameObject;
			if (roarEmitter == null)
			{
				Finish();
				return;
			}
			spawnPointGo = base.Fsm.GetOwnerDefaultTarget(spawnPoint);
			if (spawnPointGo == null)
			{
				Finish();
				return;
			}
			roarEmitter.transform.position = spawnPointGo.transform.position;
			GameObject gameObject = GameManager.instance.hero_ctrl.gameObject;
			if (gameObject != null && stunHero.Value)
			{
				FSMUtility.SendEventToGameObject(gameObject, "ROAR WOUND CANCEL");
			}
			if (delay.Value < 0.001f)
			{
				StartRoar();
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
					StartRoar();
				}
			}
		}

		public override void OnExit()
		{
			if (stopOnExit)
			{
				FSMUtility.SendEventToGameObject(roarEmitter, "END");
				HeroController silentInstance = HeroController.SilentInstance;
				if ((bool)silentInstance)
				{
					FSMUtility.SendEventToGameObject(silentInstance.gameObject, "ROAR EXIT");
				}
			}
		}

		private void StartRoar()
		{
			Vector3 position = spawnPointGo.transform.position;
			roarEmitter.transform.position = new Vector3(position.x, position.y, 0f);
			if (!noVisualEffect.Value)
			{
				FSMUtility.SendEventToGameObject(roarEmitter, roarBurst.Value ? "BURST" : "START");
			}
			if (stunHero.Value)
			{
				GameObject gameObject = GameManager.instance.hero_ctrl.gameObject;
				if (noRecoil.Value)
				{
					PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "Roar and Wound States");
					if ((bool)playMakerFSM)
					{
						playMakerFSM.FsmVariables.FindFsmFloat("Push Strength").Value = 0f;
					}
				}
				if (forceThroughBind.Value)
				{
					FSMUtility.SendEventToGameObject(gameObject, roarBurst.Value ? "ROAR BURST ENTER FORCED" : "ROAR ENTER FORCED");
				}
				else
				{
					FSMUtility.SendEventToGameObject(gameObject, roarBurst.Value ? "ROAR BURST ENTER" : "ROAR ENTER");
				}
			}
			didRoar = true;
			Finish();
		}
	}
}
