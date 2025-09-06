using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Play a different anim clip depending on target direction")]
	public class FaceObjectAnim : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault turningObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmString clipL;

		public FsmString clipR;

		public bool startFacingRight = true;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private bool facingRight;

		private tk2dSpriteAnimator _sprite;

		private GameObject turner;

		public override void Reset()
		{
			turningObject = null;
			target = null;
			clipL = null;
			clipR = null;
			startFacingRight = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			turner = base.Fsm.GetOwnerDefaultTarget(turningObject);
			_sprite = turner.GetComponent<tk2dSpriteAnimator>();
			facingRight = startFacingRight;
			if (_sprite == null)
			{
				Finish();
			}
			DoFace();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFace();
		}

		private void DoFace()
		{
			bool flag = ((target.Value.gameObject.transform.position.x > turner.transform.position.x) ? true : false);
			if (!facingRight && flag)
			{
				_sprite.PlayFromFrame(clipR.Value, 0);
				facingRight = true;
			}
			else if (facingRight && !flag)
			{
				_sprite.PlayFromFrame(clipL.Value, 0);
				facingRight = false;
			}
		}
	}
}
