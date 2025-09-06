using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class TranslateRandom : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		public FsmVector3 translateMin;

		public FsmVector3 translateMax;

		[Tooltip("Translate in local or world space.")]
		public Space space;

		public override void Reset()
		{
			gameObject = null;
			translateMin = null;
			translateMax = null;
			space = Space.Self;
		}

		public override void OnEnter()
		{
			DoTranslate();
			Finish();
		}

		private void DoTranslate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 translation = new Vector3(Random.Range(translateMin.Value.x, translateMax.Value.x), Random.Range(translateMin.Value.y, translateMax.Value.y), Random.Range(translateMin.Value.z, translateMax.Value.z));
				ownerDefaultTarget.transform.Translate(translation, space);
			}
		}
	}
}
