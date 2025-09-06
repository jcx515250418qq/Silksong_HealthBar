using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Checks the current contacts of a Rigidbody2D and sends an FSM event if a specific contact is met.")]
	public sealed class CheckContacts2D : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject with the Rigidbody2D.")]
		public FsmOwnerDefault gameObject;

		public FsmBool useRB2D;

		[UIHint(UIHint.Tag)]
		[Tooltip("Optional tag to filter contacts.")]
		public FsmString tagFilter;

		[UIHint(UIHint.Layer)]
		[Tooltip("Optional layer to filter contacts.")]
		public FsmInt layerFilter;

		public FsmBool includeTriggers;

		[Tooltip("The event to send if a specific contact is met.")]
		public FsmEvent contactEvent;

		private Rigidbody2D rigidbody2D;

		private Collider2D collider2D;

		private bool hasRb2d;

		private static ContactPoint2D[] contacts = new ContactPoint2D[10];

		public override void Reset()
		{
			gameObject = null;
			useRB2D = null;
			tagFilter = "";
			layerFilter = null;
			includeTriggers = true;
			contactEvent = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				Finish();
				return;
			}
			hasRb2d = false;
			if (useRB2D.Value)
			{
				rigidbody2D = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				hasRb2d = rigidbody2D != null;
				_ = hasRb2d;
			}
			if (!hasRb2d)
			{
				collider2D = ownerDefaultTarget.GetComponent<Collider2D>();
				if (collider2D == null)
				{
					Finish();
					return;
				}
			}
			if (CheckContacts())
			{
				Finish();
			}
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			CheckContacts();
			Finish();
		}

		private bool CheckContacts()
		{
			ContactFilter2D contactFilter = default(ContactFilter2D);
			if (!layerFilter.IsNone)
			{
				int num = 1 << layerFilter.Value;
				contactFilter.SetLayerMask(num);
			}
			contactFilter.useTriggers = includeTriggers.Value;
			int num2 = (hasRb2d ? rigidbody2D.GetContacts(contactFilter, contacts) : collider2D.GetContacts(contactFilter, contacts));
			if (num2 == 0)
			{
				contactFilter = default(ContactFilter2D);
				num2 = (hasRb2d ? rigidbody2D.GetContacts(contactFilter, contacts) : collider2D.GetContacts(contactFilter, contacts));
				if (num2 == 0)
				{
					contacts.EmptyArray();
					return false;
				}
			}
			bool flag = !string.IsNullOrEmpty(tagFilter.Value);
			for (int i = 0; i < num2; i++)
			{
				ContactPoint2D contactPoint2D = contacts[i];
				Collider2D collider = contactPoint2D.collider;
				if (!flag || collider.CompareTag(tagFilter.Value))
				{
					contacts.EmptyArray();
					base.Fsm.Event(contactEvent);
					return true;
				}
			}
			contacts.EmptyArray();
			return false;
		}
	}
}
