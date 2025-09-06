using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SpawnProjectile : FsmStateAction
	{
		public FsmOwnerDefault SpawnPoint;

		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmGameObject Prefab;

		public FsmVector3 Position;

		public FsmQuaternion Rotation;

		public FsmVector2 ImpulseForce;

		[HideIf("IsNoSpawnPointSpecified")]
		public Space Space;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreSpawned;

		public bool IsNoSpawnPointSpecified()
		{
			if (SpawnPoint.OwnerOption == OwnerDefaultOption.SpecifyGameObject)
			{
				return SpawnPoint.GameObject.IsNone;
			}
			return false;
		}

		public override void Reset()
		{
			SpawnPoint = new FsmOwnerDefault();
			Prefab = new FsmGameObject();
			Position = new FsmVector3
			{
				UseVariable = true
			};
			Rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			ImpulseForce = new FsmVector2();
			Space = Space.Self;
			StoreSpawned = new FsmGameObject();
		}

		public override void OnEnter()
		{
			Vector3 value = Position.Value;
			Quaternion rotation = Rotation.Value;
			GameObject safe = SpawnPoint.GetSafe(this);
			if ((bool)safe)
			{
				value += safe.transform.position;
				rotation = safe.transform.rotation;
			}
			if ((bool)Prefab.Value)
			{
				GameObject gameObject = Prefab.Value.Spawn(value, rotation);
				StoreSpawned.Value = gameObject;
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				if ((bool)component)
				{
					Vector2 force = ((Space == Space.World || safe == null) ? ImpulseForce.Value : ((Vector2)safe.transform.TransformVector(ImpulseForce.Value)));
					component.AddForce(force, ForceMode2D.Impulse);
				}
			}
			Finish();
		}
	}
}
