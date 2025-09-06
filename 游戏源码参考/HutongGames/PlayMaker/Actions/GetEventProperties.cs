using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets properties on the last event that caused a state change. Use Set Event Properties to define these values when sending events")]
	public class GetEventProperties : FsmStateAction
	{
		[CompoundArray("Event Properties", "Key", "Data")]
		public FsmString[] keys;

		[UIHint(UIHint.Variable)]
		public FsmVar[] datas;

		public override void Reset()
		{
			keys = new FsmString[1];
			datas = new FsmVar[1];
		}

		public override void OnEnter()
		{
			try
			{
				if (SetEventProperties.properties == null)
				{
					throw new ArgumentException("no properties");
				}
				for (int i = 0; i < keys.Length; i++)
				{
					if (SetEventProperties.properties.ContainsKey(keys[i].Value))
					{
						PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, datas[i], SetEventProperties.properties[keys[i].Value]);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log("no properties found " + ex);
			}
			Finish();
		}
	}
}
