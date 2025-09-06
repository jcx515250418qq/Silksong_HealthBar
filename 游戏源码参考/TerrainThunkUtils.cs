using GlobalSettings;
using UnityEngine;

public static class TerrainThunkUtils
{
	public enum SlashDirection
	{
		None = 0,
		Left = 1,
		Right = 2,
		Up = 3,
		Down = 4
	}

	public struct TerrainThunkConditionArgs
	{
		public int RecoilDirection;

		public Vector2 ThunkPos;
	}

	public delegate bool TerrainThunkConditionDelegate(TerrainThunkConditionArgs args);

	public static Vector2 GenerateTerrainThunk(Collision2D collision, ContactPoint2D[] storeContacts, SlashDirection slashDirection, Vector2 recoilOrigin, out int recoilDir, out int surfaceDir, TerrainThunkConditionDelegate conditionDelegate = null)
	{
		return GenerateTerrainThunk(collision.GetContacts(storeContacts), storeContacts, slashDirection, recoilOrigin, out recoilDir, out surfaceDir, conditionDelegate);
	}

	public static Vector2 GenerateTerrainThunk(int contactCount, ContactPoint2D[] storeContacts, SlashDirection slashDirection, Vector2 recoilOrigin, out int recoilDir, out int surfaceDir, TerrainThunkConditionDelegate conditionDelegate = null)
	{
		recoilDir = -1;
		surfaceDir = -1;
		GameObject enemyNailTerrainThunk = Effects.EnemyNailTerrainThunk;
		if (!enemyNailTerrainThunk)
		{
			Debug.LogError("No terrain thunk prefab to spawn!");
			return Vector2.zero;
		}
		bool flag = false;
		float num = float.MaxValue;
		Vector2 vector = Vector2.zero;
		bool flag2 = false;
		for (int i = 0; i < contactCount; i++)
		{
			ContactPoint2D contactPoint2D = storeContacts[i];
			Collider2D collider = contactPoint2D.collider;
			if (collider.isTrigger)
			{
				continue;
			}
			Vector2 point = contactPoint2D.point;
			Vector2 vector2 = point - recoilOrigin;
			switch (slashDirection)
			{
			case SlashDirection.Left:
				if (vector2.x > 0f)
				{
					continue;
				}
				break;
			case SlashDirection.Right:
				if (vector2.x < 0f)
				{
					continue;
				}
				break;
			case SlashDirection.Up:
				if (vector2.y < 0f)
				{
					continue;
				}
				break;
			case SlashDirection.Down:
				if (vector2.y > 0f)
				{
					continue;
				}
				break;
			}
			bool doRecoil = true;
			GetThunkProperties(collider.gameObject, out var shouldThunk, ref doRecoil);
			Vector2 vector3 = recoilOrigin;
			switch (slashDirection)
			{
			case SlashDirection.Left:
			case SlashDirection.Right:
				vector3.y = point.y;
				break;
			case SlashDirection.Up:
			case SlashDirection.Down:
				vector3.x = point.x;
				break;
			}
			Vector2 vector4 = point - vector3;
			Vector2 normalized = vector4.normalized;
			float distance = vector4.magnitude + 1f;
			RaycastHit2D raycastHit2D = Helper.Raycast2D(vector3, normalized, distance, 1 << collider.gameObject.layer);
			if (raycastHit2D.collider == null)
			{
				continue;
			}
			float num2 = Vector2.Distance(raycastHit2D.point, recoilOrigin);
			if (num2 > num)
			{
				continue;
			}
			num = num2;
			flag = true;
			vector = raycastHit2D.point;
			if (shouldThunk)
			{
				flag2 = true;
			}
			Vector2 normal = raycastHit2D.normal;
			float degrees = 57.29578f * Mathf.Atan2(normal.y, normal.x);
			surfaceDir = DirectionUtils.GetCardinalDirection(degrees);
			if (doRecoil)
			{
				switch (slashDirection)
				{
				case SlashDirection.None:
				{
					Vector2 vector5 = -vector2.normalized;
					float degrees2 = 57.29578f * Mathf.Atan2(vector5.y, vector5.x);
					recoilDir = DirectionUtils.GetCardinalDirection(degrees2);
					break;
				}
				case SlashDirection.Left:
					recoilDir = 0;
					break;
				case SlashDirection.Right:
					recoilDir = 2;
					break;
				case SlashDirection.Up:
					recoilDir = 3;
					break;
				case SlashDirection.Down:
					recoilDir = 1;
					break;
				}
			}
		}
		if (!flag)
		{
			return Vector2.zero;
		}
		if (conditionDelegate != null)
		{
			TerrainThunkConditionArgs terrainThunkConditionArgs = default(TerrainThunkConditionArgs);
			terrainThunkConditionArgs.RecoilDirection = recoilDir;
			terrainThunkConditionArgs.ThunkPos = vector;
			TerrainThunkConditionArgs args = terrainThunkConditionArgs;
			if (!conditionDelegate(args))
			{
				return Vector2.zero;
			}
		}
		if (flag2)
		{
			enemyNailTerrainThunk.Spawn(vector, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
		}
		return vector;
	}

	public static void GetThunkProperties(GameObject target, out bool shouldThunk, ref bool doRecoil)
	{
		if ((bool)target.GetComponent<TinkEffect>())
		{
			shouldThunk = false;
			doRecoil = false;
			return;
		}
		NonThunker component = target.GetComponent<NonThunker>();
		if (component != null)
		{
			if (component.active)
			{
				shouldThunk = false;
				doRecoil = component.doRecoil;
			}
			else
			{
				shouldThunk = true;
			}
		}
		else
		{
			shouldThunk = true;
		}
	}
}
