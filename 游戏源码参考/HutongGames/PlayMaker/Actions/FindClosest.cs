using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds the closest object to the specified Game Object.\nOptionally filter by Tag and Visibility.")]
	public class FindClosest : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to measure from.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Tag)]
		[Tooltip("Only consider objects with this Tag. NOTE: It's generally a lot quicker to find objects with a Tag!")]
		public FsmString withTag;

		[Tooltip("If checked, ignores the object that owns this FSM.")]
		public FsmBool ignoreOwner;

		[Tooltip("Only consider objects visible to the camera.")]
		public FsmBool mustBeVisible;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the closest object.")]
		public FsmGameObject storeObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance to the closest object.")]
		public FsmFloat storeDistance;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			withTag = "Untagged";
			ignoreOwner = true;
			mustBeVisible = false;
			storeObject = null;
			storeDistance = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoFindClosest();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoFindClosest();
		}

		private void DoFindClosest()
		{
			GameObject gameObject = ((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			GameObject[] array = ((!string.IsNullOrEmpty(withTag.Value) && !(withTag.Value == "Untagged")) ? GameObject.FindGameObjectsWithTag(withTag.Value) : ((GameObject[])Object.FindObjectsOfType(typeof(GameObject))));
			GameObject value = null;
			float num = float.PositiveInfinity;
			GameObject[] array2 = array;
			foreach (GameObject gameObject2 in array2)
			{
				if ((!ignoreOwner.Value || !(gameObject2 == base.Owner)) && (!mustBeVisible.Value || ActionHelpers.IsVisible(gameObject2)))
				{
					float sqrMagnitude = (gameObject.transform.position - gameObject2.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						value = gameObject2;
					}
				}
			}
			storeObject.Value = value;
			if (!storeDistance.IsNone)
			{
				storeDistance.Value = Mathf.Sqrt(num);
			}
		}
	}
}
