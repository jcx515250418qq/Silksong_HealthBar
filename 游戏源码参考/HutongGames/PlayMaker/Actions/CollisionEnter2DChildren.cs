using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CollisionEnter2DChildren : FsmStateAction
	{
		public enum CollisionTypes
		{
			Enter = 0,
			Exit = 1
		}

		public FsmOwnerDefault Target;

		public CollisionTypes CollisionType;

		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString CollideTag;

		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt CollideLayer;

		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreNormal;

		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent SendEvent;

		private readonly HashSet<CollisionEnterEvent> collisionTrackers = new HashSet<CollisionEnterEvent>();

		public override void Reset()
		{
			Target = null;
			CollisionType = CollisionTypes.Enter;
			CollideTag = new FsmString
			{
				UseVariable = true
			};
			CollideLayer = new FsmInt
			{
				UseVariable = true
			};
			StoreNormal = null;
			SendEvent = null;
		}

		public override void OnEnter()
		{
			Collider2D[] componentsInChildren = Target.GetSafe(this).GetComponentsInChildren<Collider2D>();
			foreach (Collider2D collider2D in componentsInChildren)
			{
				CollisionEnterEvent collisionEnterEvent = collider2D.GetComponent<CollisionEnterEvent>();
				if (!collisionEnterEvent)
				{
					collisionEnterEvent = collider2D.gameObject.AddComponent<CollisionEnterEvent>();
				}
				collisionTrackers.Add(collisionEnterEvent);
			}
			switch (CollisionType)
			{
			case CollisionTypes.Enter:
			{
				foreach (CollisionEnterEvent collisionTracker in collisionTrackers)
				{
					collisionTracker.CollisionEntered += TrackerOnCollision;
				}
				break;
			}
			case CollisionTypes.Exit:
			{
				foreach (CollisionEnterEvent collisionTracker2 in collisionTrackers)
				{
					collisionTracker2.CollisionExited += TrackerOnCollision;
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public override void OnExit()
		{
			switch (CollisionType)
			{
			case CollisionTypes.Enter:
				foreach (CollisionEnterEvent collisionTracker in collisionTrackers)
				{
					collisionTracker.CollisionEntered -= TrackerOnCollision;
				}
				break;
			case CollisionTypes.Exit:
				foreach (CollisionEnterEvent collisionTracker2 in collisionTrackers)
				{
					collisionTracker2.CollisionExited -= TrackerOnCollision;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			collisionTrackers.Clear();
		}

		private void TrackerOnCollision(Collision2D collision)
		{
			if ((CollideLayer.IsNone || collision.gameObject.layer == CollideLayer.Value) && (CollideTag.IsNone || collision.gameObject.CompareTag(CollideTag.Value)))
			{
				StoreNormal.Value = collision.GetSafeContact().Normal;
				base.Fsm.Event(SendEvent);
			}
		}
	}
}
