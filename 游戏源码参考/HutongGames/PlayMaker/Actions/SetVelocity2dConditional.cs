using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the 2d Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody 2D.")]
	public sealed class SetVelocity2dConditional : ComponentAction<Rigidbody2D>
	{
		public class Condition
		{
			public FsmFloat value;

			public ComparisonType comparisonType;
		}

		public enum ComparisonType
		{
			None = 0,
			GreaterThan = 1,
			LessThan = 2,
			EqualTo = 3,
			GreaterThanOrEqualTo = 4,
			LessThanOrEqualTo = 5
		}

		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		[Tooltip("A Vector2 value for the velocity")]
		public FsmVector2 vector;

		[Tooltip("The x value of the velocity. Overrides 'Vector' x value if set")]
		public FsmFloat x;

		[Tooltip("The y value of the velocity. Overrides 'Vector' y value if set")]
		public FsmFloat y;

		[Tooltip("Conditions for the x value")]
		public Condition xCondition;

		[Tooltip("Conditions for the y value")]
		public Condition yCondition;

		[Tooltip("Both conditions must be true")]
		public FsmBool bothConditionsMustBeTrue;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

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
			xCondition = new Condition
			{
				comparisonType = ComparisonType.None
			};
			yCondition = new Condition
			{
				comparisonType = ComparisonType.None
			};
			bothConditionsMustBeTrue = null;
			everyFrame = false;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoSetVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 linearVelocity = ((!vector.IsNone) ? vector.Value : base.rigidbody2d.linearVelocity);
			bool flag = CheckCondition(linearVelocity.x, xCondition);
			bool flag2 = CheckCondition(linearVelocity.y, yCondition);
			if (bothConditionsMustBeTrue.Value)
			{
				if (flag && flag2)
				{
					if (!x.IsNone)
					{
						linearVelocity.x = x.Value;
					}
					if (!y.IsNone)
					{
						linearVelocity.y = y.Value;
					}
				}
			}
			else
			{
				if (flag && !x.IsNone)
				{
					linearVelocity.x = x.Value;
				}
				if (flag2 && !y.IsNone)
				{
					linearVelocity.y = y.Value;
				}
			}
			base.rigidbody2d.linearVelocity = linearVelocity;
		}

		private bool CheckCondition(float currentValue, Condition condition)
		{
			return condition.comparisonType switch
			{
				ComparisonType.GreaterThan => currentValue > condition.value.Value, 
				ComparisonType.LessThan => currentValue < condition.value.Value, 
				ComparisonType.EqualTo => Mathf.Approximately(currentValue, condition.value.Value), 
				ComparisonType.GreaterThanOrEqualTo => currentValue >= condition.value.Value, 
				ComparisonType.LessThanOrEqualTo => currentValue <= condition.value.Value, 
				_ => true, 
			};
		}
	}
}
