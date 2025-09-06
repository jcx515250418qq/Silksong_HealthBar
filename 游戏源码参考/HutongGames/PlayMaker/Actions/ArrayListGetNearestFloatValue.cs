using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the average value within an arrayList.")]
	public class ArrayListGetNearestFloatValue : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("The target Value")]
		public FsmFloat floatValue;

		[Tooltip("Performs every frame. WARNING, it could be affecting performances.")]
		public bool everyframe;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The index of the nearest Value")]
		public FsmInt nearestIndex;

		[UIHint(UIHint.Variable)]
		[Tooltip("The nearest Value")]
		public FsmFloat nearestValue;

		private List<float> _floats;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			floatValue = null;
			nearestIndex = null;
			nearestValue = null;
			everyframe = true;
		}

		public override void OnEnter()
		{
			if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				Finish();
			}
			DoGetNearestValue();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetNearestValue();
		}

		private void DoGetNearestValue()
		{
			if (!isProxyValid())
			{
				return;
			}
			_floats = new List<float>();
			foreach (object array in proxy.arrayList)
			{
				_floats.Add(Convert.ToSingle(array));
			}
			float value = floatValue.Value;
			if (_floats.Count <= 0)
			{
				return;
			}
			float value2 = float.MaxValue;
			float num = float.MaxValue;
			int value3 = 0;
			int num2 = 0;
			foreach (float @float in _floats)
			{
				float num3 = Mathf.Abs(@float - value);
				if (num > num3)
				{
					num = num3;
					value2 = @float;
					value3 = num2;
				}
				num2++;
			}
			nearestIndex.Value = value3;
			nearestValue.Value = value2;
		}
	}
}
