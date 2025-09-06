using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates a GameObject and de-activates other GameObjects at the same level of the hierarchy. E.g, a single UI Screen, a single accessory etc. This action is very helpful if you often organize GameObject hierarchies in this way. \nNOTE: The targeted GameObject should have a parent. This action will not work on GameObjects at the scene root.")]
	public class ActivateSolo : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to activate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Re-activate if already active. This means deactivating the target GameObject then activating it again. This will reset FSMs on that GameObject.")]
		public FsmBool allowReactivate;

		private int activatedFrame = -1;

		public override void Reset()
		{
			gameObject = null;
			allowReactivate = new FsmBool
			{
				Value = true
			};
		}

		public override void OnEnter()
		{
			DoActivateSolo();
			Finish();
		}

		private void DoActivateSolo()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null || ownerDefaultTarget.transform.parent == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform;
			foreach (Transform item in ownerDefaultTarget.transform.parent.transform)
			{
				if (item != transform)
				{
					item.gameObject.SetActive(value: false);
				}
			}
			if (allowReactivate.Value && Time.frameCount != activatedFrame)
			{
				transform.gameObject.SetActive(value: false);
				activatedFrame = Time.frameCount;
			}
			ownerDefaultTarget.SetActive(value: true);
		}
	}
}
