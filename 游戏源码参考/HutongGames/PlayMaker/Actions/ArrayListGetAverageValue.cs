using System;
using System.Collections.Generic;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the average value within an arrayList. It can use float, int, vector2 and vector3 ( uses magnitude), rect ( uses surface), gameobject ( using bounding box volume), and string ( use lenght)")]
	public class ArrayListGetAverageValue : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("Performs every frame. WARNING, it could be affecting performances.")]
		public bool everyframe;

		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The average Value")]
		public FsmFloat averageValue;

		private List<float> _floats;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			averageValue = null;
			everyframe = true;
		}

		public override void OnEnter()
		{
			if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				Finish();
			}
			DoGetAverageValue();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetAverageValue();
		}

		private void DoGetAverageValue()
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
			if (_floats.Count > 0)
			{
				averageValue.Value = _floats.Aggregate((float acc, float cur) => acc + cur) / (float)_floats.Count;
			}
			else
			{
				averageValue.Value = 0f;
			}
		}
	}
}
