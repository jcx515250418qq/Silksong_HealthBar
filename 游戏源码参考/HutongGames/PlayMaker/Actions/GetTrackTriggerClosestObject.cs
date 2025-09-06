using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetTrackTriggerClosestObject : FsmStateAction
	{
		public FsmOwnerDefault ClosestTo;

		[CheckForComponent(typeof(TrackTriggerObjects))]
		public FsmGameObject TrackTrigger;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject ExcludedObject;

		private List<GameObject> excludeWrapper;

		public override void Reset()
		{
			ClosestTo = null;
			TrackTrigger = null;
			StoreObject = null;
			ExcludedObject = null;
		}

		public override void OnEnter()
		{
			TrackTriggerObjects component = TrackTrigger.Value.GetComponent<TrackTriggerObjects>();
			if (component.IsInside)
			{
				if (excludeWrapper == null)
				{
					excludeWrapper = new List<GameObject>(1);
				}
				else
				{
					excludeWrapper.Clear();
				}
				if ((bool)ExcludedObject.Value)
				{
					excludeWrapper.Add(ExcludedObject.Value);
				}
				Vector2 toPos = ClosestTo.GetSafe(this).transform.position;
				GameObject closestInside = component.GetClosestInside(toPos, excludeWrapper);
				StoreObject.Value = closestInside;
			}
			else
			{
				StoreObject.Value = null;
			}
			Finish();
		}
	}
}
