using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Stores the result as + or - a target distance depending on if on the right or left.")]
	public class GetClosestTargetDistanceX : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault Target;

		private GameObject target;

		[RequiredField]
		public FsmGameObject FromGameObject;

		private GameObject from;

		[RequiredField]
		public FsmFloat TargetDistanceX;

		[Space]
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDistanceX;

		[UIHint(UIHint.Variable)]
		public FsmFloat StorePositionX;

		public bool EveryFrame;

		public override void Reset()
		{
			Target = null;
			FromGameObject = null;
			TargetDistanceX = null;
			StoreDistanceX = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			target = Target.GetSafe(this);
			from = FromGameObject.Value;
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			float num = Mathf.Abs(TargetDistanceX.Value);
			StoreDistanceX.Value = ((from.transform.position.x < target.transform.position.x) ? num : (0f - num));
			StorePositionX.Value = from.transform.position.x + StoreDistanceX.Value;
			if (StoreDistanceX.IsNone && StorePositionX.IsNone)
			{
				Debug.LogWarning("No values are being stored!", base.Owner);
			}
		}
	}
}
