using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SpawnProjectileV2 : FsmStateAction
	{
		public FsmOwnerDefault SpawnPoint;

		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmGameObject Prefab;

		public FsmVector3 Position;

		public FsmQuaternion Rotation;

		public FsmBool FlipScaleFireLeft;

		public FsmFloat Speed;

		public FsmFloat MinAngle;

		public FsmFloat MaxAngle;

		[HideIf("IsNoSpawnPointSpecified")]
		public Space Space;

		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreSpawned;

		[UsedImplicitly]
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
			SpawnPoint = null;
			Prefab = null;
			Position = new FsmVector3
			{
				UseVariable = true
			};
			Rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			FlipScaleFireLeft = null;
			Speed = null;
			MinAngle = null;
			MaxAngle = null;
			Space = Space.Self;
			StoreSpawned = null;
		}

		public override void OnEnter()
		{
			Vector3 position = Position.Value;
			Quaternion value = Rotation.Value;
			GameObject safe = SpawnPoint.GetSafe(this);
			if ((bool)safe)
			{
				position = safe.transform.TransformPoint(position);
			}
			if ((bool)Prefab.Value)
			{
				GameObject gameObject = Prefab.Value.Spawn(position, value);
				StoreSpawned.Value = gameObject;
				gameObject.transform.localScale = Prefab.Value.transform.localScale;
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				if ((bool)component)
				{
					float value2 = Speed.Value;
					float num = UnityEngine.Random.Range(MinAngle.Value, MaxAngle.Value);
					float x = value2 * Mathf.Cos(num * (MathF.PI / 180f));
					float y = value2 * Mathf.Sin(num * (MathF.PI / 180f));
					Vector2 vector = new Vector2(x, y);
					if (Space == Space.Self && safe != null)
					{
						vector = safe.transform.TransformVector(vector);
					}
					HeroController componentInParent = safe.GetComponentInParent<HeroController>();
					if ((bool)componentInParent && componentInParent.cState.wallSliding)
					{
						vector.x *= -1f;
					}
					if (vector.x < 0f && FlipScaleFireLeft.Value)
					{
						gameObject.transform.FlipLocalScale(x: true);
					}
					component.linearVelocity = vector;
					gameObject.GetComponent<IProjectile>()?.VelocityWasSet();
				}
			}
			Finish();
		}
	}
}
