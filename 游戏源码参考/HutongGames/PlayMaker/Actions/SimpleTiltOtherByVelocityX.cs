using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class SimpleTiltOtherByVelocityX : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		public FsmGameObject objectToTilt;

		[RequiredField]
		public FsmGameObject getSpeedFrom;

		public FsmFloat tiltFactor;

		public FsmBool reverseIfXScaleNegative;

		private Transform go_transform;

		private Rigidbody2D go_rb2d;

		public override void Reset()
		{
			objectToTilt = null;
			getSpeedFrom = null;
			tiltFactor = null;
			reverseIfXScaleNegative = null;
		}

		public override void OnEnter()
		{
			go_transform = objectToTilt.Value.transform;
			go_rb2d = getSpeedFrom.Value.GetComponent<Rigidbody2D>();
			DoTilt();
		}

		public override void OnUpdate()
		{
			DoTilt();
		}

		private void DoTilt()
		{
			float num = go_rb2d.linearVelocity.x * tiltFactor.Value;
			if (reverseIfXScaleNegative.Value && go_transform.lossyScale.x < 0f)
			{
				num *= -1f;
			}
			go_transform.localEulerAngles = new Vector3(go_transform.localEulerAngles.x, go_transform.localEulerAngles.y, num);
		}
	}
}
