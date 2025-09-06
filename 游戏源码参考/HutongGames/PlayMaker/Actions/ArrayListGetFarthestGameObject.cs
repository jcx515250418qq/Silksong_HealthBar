using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the farthest GameObject within an arrayList from a transform or position.")]
	public class ArrayListGetFarthestGameObject : ArrayListActions
	{
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[Tooltip("Compare the distance of the items in the list to the position of this gameObject")]
		public FsmGameObject distanceFrom;

		[Tooltip("If DistanceFrom declared, use OrDistanceFromVector3 as an offset")]
		public FsmVector3 orDistanceFromVector3;

		public bool everyframe;

		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject farthestGameObject;

		[UIHint(UIHint.Variable)]
		public FsmInt farthestIndex;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			distanceFrom = null;
			orDistanceFromVector3 = null;
			farthestGameObject = null;
			farthestIndex = null;
			everyframe = true;
		}

		public override void OnEnter()
		{
			if (!SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(gameObject), reference.Value))
			{
				Finish();
			}
			DoFindFarthestGo();
			if (!everyframe)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFindFarthestGo();
		}

		private void DoFindFarthestGo()
		{
			if (!isProxyValid())
			{
				return;
			}
			Vector3 value = orDistanceFromVector3.Value;
			GameObject value2 = distanceFrom.Value;
			if (value2 != null)
			{
				value += value2.transform.position;
			}
			float num = float.PositiveInfinity;
			int num2 = 0;
			foreach (GameObject array in proxy.arrayList)
			{
				if (array != null)
				{
					float sqrMagnitude = (array.transform.position - value).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						num = sqrMagnitude;
						farthestGameObject.Value = array;
						farthestIndex.Value = num2;
					}
				}
				num2++;
			}
		}
	}
}
