using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class GetClosestGameObject : FsmStateAction
	{
		private struct Match
		{
			public FsmGameObject GameObject;

			public FsmEvent Event;
		}

		public FsmOwnerDefault Target;

		public FsmGameObject[] GameObjects;

		public FsmEvent[] MatchEvents;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreGameObject;

		public bool EveryFrame;

		private Vector2 targetPosition;

		private readonly List<Match> matches = new List<Match>();

		public override void Reset()
		{
			Target = null;
			GameObjects = new FsmGameObject[0];
			MatchEvents = new FsmEvent[0];
			StoreGameObject = null;
			EveryFrame = false;
		}

		public override string ErrorCheck()
		{
			if (GameObjects.Length == 0 && MatchEvents.Length == 0)
			{
				return string.Empty;
			}
			if (GameObjects.Length == MatchEvents.Length)
			{
				return string.Empty;
			}
			return "GameObjects and MatchEvents must have the same amount of elements!";
		}

		public override void OnEnter()
		{
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
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			targetPosition = safe.transform.position;
			matches.Clear();
			int num = Mathf.Min(GameObjects.Length, MatchEvents.Length);
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					matches.Add(new Match
					{
						GameObject = GameObjects[i],
						Event = MatchEvents[i]
					});
				}
				matches.Sort(delegate(Match match1, Match match2)
				{
					float num2 = (match1.GameObject.Value ? Vector2.Distance(targetPosition, match1.GameObject.Value.transform.position) : float.MaxValue);
					float value = (match2.GameObject.Value ? Vector2.Distance(targetPosition, match2.GameObject.Value.transform.position) : float.MaxValue);
					return num2.CompareTo(value);
				});
				Match match3 = matches[0];
				StoreGameObject.Value = match3.GameObject.Value;
				base.Fsm.Event(match3.Event);
			}
		}
	}
}
