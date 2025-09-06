using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("iTween")]
	[Tooltip("CSimilar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
	public class iTweenScaleUpdate : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[Tooltip("Scale To a transform scale.")]
		public FsmGameObject transformScale;

		[Tooltip("A scale vector the GameObject will animate To.")]
		public FsmVector3 vectorScale;

		[Tooltip("The time in seconds the animation will take to complete. If transformScale is set, this is used as an offset.")]
		public FsmFloat time;

		private Hashtable hash;

		private GameObject go;

		public override void Reset()
		{
			transformScale = new FsmGameObject
			{
				UseVariable = true
			};
			vectorScale = new FsmVector3
			{
				UseVariable = true
			};
			time = 1f;
		}

		public override void OnEnter()
		{
			hash = new Hashtable();
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				Finish();
				return;
			}
			if (transformScale.IsNone)
			{
				hash.Add("scale", vectorScale.IsNone ? Vector3.zero : vectorScale.Value);
			}
			else if (vectorScale.IsNone)
			{
				hash.Add("scale", transformScale.Value.transform);
			}
			else
			{
				hash.Add("scale", transformScale.Value.transform.localScale + vectorScale.Value);
			}
			hash.Add("time", time.IsNone ? 1f : time.Value);
			DoiTween();
		}

		public override void OnExit()
		{
		}

		public override void OnUpdate()
		{
			hash.Remove("scale");
			if (transformScale.IsNone)
			{
				hash.Add("scale", vectorScale.IsNone ? Vector3.zero : vectorScale.Value);
			}
			else if (vectorScale.IsNone)
			{
				hash.Add("scale", transformScale.Value.transform);
			}
			else
			{
				hash.Add("scale", transformScale.Value.transform.localScale + vectorScale.Value);
			}
			DoiTween();
		}

		private void DoiTween()
		{
			iTween.ScaleUpdate(go, hash);
		}
	}
}
