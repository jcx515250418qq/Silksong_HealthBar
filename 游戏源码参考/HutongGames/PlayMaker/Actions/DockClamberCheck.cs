using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class DockClamberCheck : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject hero;

		[UIHint(UIHint.Variable)]
		public FsmGameObject clamberRayL;

		[UIHint(UIHint.Variable)]
		public FsmGameObject clamberRayR;

		public FsmBool noClamber;

		public FsmEvent clamberEvent;

		private const float jumpHeightMax = 10f;

		private const float platThickMax = 10f;

		private const float clamberClearance = 4.5f;

		private Transform selfTransform;

		private Transform heroTransform;

		private Transform rayLTransform;

		private Transform rayRTransform;

		private HeroController hc;

		public override void Reset()
		{
			hero = null;
			clamberRayL = null;
			clamberRayR = null;
			clamberEvent = null;
		}

		public override void OnEnter()
		{
			selfTransform = base.Owner.transform;
			heroTransform = hero.Value.transform;
			rayLTransform = clamberRayL.Value.transform;
			rayRTransform = clamberRayR.Value.transform;
			hc = hero.Value.GetComponent<HeroController>();
		}

		public override void OnUpdate()
		{
			if (noClamber.Value)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (heroTransform.position.y > selfTransform.position.y)
			{
				flag = true;
			}
			if (flag)
			{
				flag2 = hc.GetState("onGround");
			}
			if (flag2)
			{
				Vector2 origin = rayLTransform.position;
				Vector2 origin2 = rayRTransform.position;
				int mask = LayerMask.GetMask("Terrain");
				RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, Vector2.up, 10f, mask);
				RaycastHit2D raycastHit2D2 = Helper.Raycast2D(origin2, Vector2.up, 10f, mask);
				bool num = raycastHit2D.collider != null;
				bool flag4 = raycastHit2D2.collider != null;
				if (num && flag4 && raycastHit2D.point.y == raycastHit2D2.point.y && raycastHit2D.collider.gameObject.tag == "Platform" && raycastHit2D2.collider.gameObject.tag == "Platform")
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				FSMUtility.SendEventToGameObject(base.Owner, clamberEvent);
			}
		}
	}
}
