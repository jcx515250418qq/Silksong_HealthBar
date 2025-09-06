using System;

namespace HutongGames.PlayMaker.Actions
{
	public sealed class CheckHeroNailImbuement : FsmStateAction
	{
		[Serializable]
		private enum CheckType
		{
			Any = 0,
			Fire = 1,
			Poison = 2
		}

		[ObjectType(typeof(CheckType))]
		public FsmEnum checkType;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		[ObjectType(typeof(NailElements))]
		[UIHint(UIHint.Variable)]
		public FsmEnum currentElementResult;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public FsmBool everyFrame;

		public override void Reset()
		{
			checkType = null;
			trueEvent = null;
			falseEvent = null;
			currentElementResult = null;
			storeResult = null;
			everyFrame = null;
		}

		public override void OnEnter()
		{
			if (DoCheck() || !everyFrame.Value)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (DoCheck())
			{
				Finish();
			}
		}

		private bool DoCheck()
		{
			HeroController instance = HeroController.instance;
			NailElements nailElements = NailElements.None;
			bool flag = false;
			bool result = false;
			if (instance != null)
			{
				HeroNailImbuement nailImbuement = instance.NailImbuement;
				if ((bool)nailImbuement)
				{
					nailElements = nailImbuement.CurrentElement;
					switch ((CheckType)(object)checkType.Value)
					{
					case CheckType.Any:
						flag = nailElements != NailElements.None;
						break;
					case CheckType.Fire:
						flag = nailElements == NailElements.Fire;
						break;
					case CheckType.Poison:
						flag = nailElements == NailElements.Poison;
						break;
					default:
						storeResult.Value = false;
						result = true;
						break;
					}
				}
				else
				{
					result = true;
				}
			}
			else
			{
				result = true;
			}
			currentElementResult.Value = nailElements;
			storeResult.Value = flag;
			if (flag)
			{
				base.Fsm.Event(trueEvent);
			}
			else
			{
				base.Fsm.Event(falseEvent);
			}
			return result;
		}
	}
}
