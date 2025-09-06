using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Rigid bodies start sleeping when they come to rest. This action wakes up all rigid bodies in the scene. E.g., if you Set Gravity and want objects at rest to respond.See Unity Docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>.")]
	[SeeAlso("<a href =\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>")]
	public class WakeAllRigidBodies : FsmStateAction
	{
		[Tooltip("Do it every frame - use with caution! Sleeping is an important physics optimization!")]
		public bool everyFrame;

		private Rigidbody[] bodies;

		public override void Reset()
		{
			everyFrame = false;
		}

		public override void OnEnter()
		{
			bodies = Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
			DoWakeAll();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoWakeAll();
		}

		private void DoWakeAll()
		{
			bodies = Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[];
			if (bodies != null)
			{
				Rigidbody[] array = bodies;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].WakeUp();
				}
			}
		}
	}
}
