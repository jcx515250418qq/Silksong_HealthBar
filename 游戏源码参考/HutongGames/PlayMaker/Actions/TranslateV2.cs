using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Translates a Game Object. Use a Vector3 variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class TranslateV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("A translation vector. NOTE: You can override individual axis below.")]
		public FsmVector3 vector;

		[Tooltip("Translation along x axis.")]
		public FsmFloat x;

		[Tooltip("Translation along y axis.")]
		public FsmFloat y;

		[Tooltip("Translation along z axis.")]
		public FsmFloat z;

		[Tooltip("Translate in local or world space.")]
		public Space space;

		[Tooltip("Translate over one second")]
		public bool perSecond;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Perform the translate in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		[Tooltip("Perform the translate in FixedUpdate. This is useful when working with rigid bodies and physics.")]
		public bool fixedUpdate;

		public bool alwaysOnStart;

		public FsmFloat yMin;

		public FsmFloat yMax;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			z = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.Self;
			perSecond = true;
			everyFrame = true;
			lateUpdate = false;
			fixedUpdate = false;
			alwaysOnStart = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			if (!everyFrame && !lateUpdate && !fixedUpdate)
			{
				DoTranslate();
				Finish();
			}
			if (alwaysOnStart)
			{
				DoTranslate();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate && !fixedUpdate)
			{
				DoTranslate();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoTranslate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			if (fixedUpdate)
			{
				DoTranslate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoTranslate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 vector = (this.vector.IsNone ? new Vector3(x.Value, y.Value, z.Value) : this.vector.Value);
				if (!x.IsNone)
				{
					vector.x = x.Value;
				}
				if (!y.IsNone)
				{
					vector.y = y.Value;
				}
				if (!z.IsNone)
				{
					vector.z = z.Value;
				}
				if (!perSecond)
				{
					ownerDefaultTarget.transform.Translate(vector, space);
				}
				else
				{
					ownerDefaultTarget.transform.Translate(vector * Time.deltaTime, space);
				}
				if (!yMin.IsNone && ownerDefaultTarget.transform.position.y < yMin.Value)
				{
					ownerDefaultTarget.transform.position = new Vector3(ownerDefaultTarget.transform.position.x, yMin.Value, ownerDefaultTarget.transform.position.z);
				}
				if (!yMax.IsNone && ownerDefaultTarget.transform.position.y > yMax.Value)
				{
					ownerDefaultTarget.transform.position = new Vector3(ownerDefaultTarget.transform.position.x, yMax.Value, ownerDefaultTarget.transform.position.z);
				}
			}
		}
	}
}
