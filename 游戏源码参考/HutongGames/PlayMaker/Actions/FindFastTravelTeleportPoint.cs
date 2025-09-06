using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class FindFastTravelTeleportPoint : FsmStateAction
	{
		private const int LAYER_MASK = 256;

		private const float GROUND_PADDING = 0.3f;

		private const float GROUND_RAY_SPACING = 0.5f;

		private const float GROUND_WIDTH = 5.5f;

		private const float ROOF_HEIGHT = 4f;

		private const float GROUND_DISTANCE = 0.5f;

		private const float GROUND_THICKNESS = 1.5f;

		private const float GROUND_THICKNESS_PADDING = 0.1f;

		public FsmOwnerDefault RelativeTo;

		public FsmVector2 Position;

		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreGroundPos;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreRoofHeight;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreRightClear;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreLeftClear;

		public FsmEvent IsClearEvent;

		public FsmEvent NotClearEvent;

		private static Material _debugLineMaterial;

		private static RaycastHit2D[] _rayHitStoreA = new RaycastHit2D[10];

		private static RaycastHit2D[] _rayHitStoreB = new RaycastHit2D[10];

		public override void Reset()
		{
			RelativeTo = null;
			Position = null;
			StoreGroundPos = null;
			StoreRoofHeight = null;
			StoreRightClear = null;
			StoreLeftClear = null;
			IsClearEvent = null;
			NotClearEvent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = RelativeTo.GetSafe(this);
			Vector2 groundPos;
			float roofHeight;
			bool isRightClear;
			bool isLeftClear;
			bool flag = IsClear(safe ? ((Vector2)safe.transform.TransformPoint(Position.Value)) : Position.Value, out groundPos, out roofHeight, out isRightClear, out isLeftClear, drawDebugLines: false);
			StoreRoofHeight.Value = roofHeight;
			StoreRightClear.Value = isRightClear;
			StoreLeftClear.Value = isLeftClear;
			StoreGroundPos.Value = groundPos;
			base.Fsm.Event(flag ? IsClearEvent : NotClearEvent);
			Finish();
		}

		public static bool IsClear(Vector2 pos, out Vector2 groundPos, out float roofHeight, out bool isRightClear, out bool isLeftClear, bool drawDebugLines)
		{
			isRightClear = false;
			isLeftClear = false;
			roofHeight = 0f;
			if (!TryGetGroundPos(pos, out groundPos, drawDebugLines))
			{
				return false;
			}
			Vector2 origin = groundPos;
			origin.y += 0.3f;
			if (!CheckRoofHeight(groundPos + new Vector2(-0.3f, 0f), out roofHeight, drawDebugLines))
			{
				return false;
			}
			if (!CheckRoofHeight(groundPos + new Vector2(0.3f, 0f), out roofHeight, drawDebugLines))
			{
				return false;
			}
			isLeftClear = IsClearDirectional(origin, -1f, 2.75f, 256, drawDebugLines);
			isRightClear = IsClearDirectional(origin, 1f, 2.75f, 256, drawDebugLines);
			return isLeftClear | isRightClear;
		}

		private static bool TryGetGroundPos(Vector2 pos, out Vector2 groundPos, bool drawDebugLines)
		{
			RaycastHit2D closestHit;
			bool flag = Helper.IsRayHittingNoTriggers(pos, Vector2.down, 5f, 256, out closestHit);
			if (NoTeleportRegion.GetTeleportBlockedState(pos) == NoTeleportRegion.TeleportAllowState.Blocked)
			{
				flag = false;
			}
			if (!flag)
			{
				if (drawDebugLines)
				{
					DrawDebugLine(pos, pos + Vector2.down * 5f, Color.red);
				}
				groundPos = Vector2.zero;
				return false;
			}
			groundPos = closestHit.point;
			if (drawDebugLines)
			{
				DrawDebugLine(pos, groundPos, Color.yellow);
			}
			return true;
		}

		private static bool CheckRoofHeight(Vector2 groundPos, out float roofHeight, bool drawDebugLines)
		{
			groundPos.y += Physics2D.defaultContactOffset * 2f;
			bool flag = Helper.IsRayHittingNoTriggers(groundPos, Vector2.up, 4f, 256, out var closestHit);
			if (drawDebugLines)
			{
				DrawDebugLine(groundPos, flag ? closestHit.point : (groundPos + Vector2.up * 4f), flag ? Color.red : Color.blue);
			}
			flag = NoTeleportRegion.GetTeleportBlockedState(groundPos) switch
			{
				NoTeleportRegion.TeleportAllowState.Blocked => true, 
				NoTeleportRegion.TeleportAllowState.Allowed => false, 
				_ => flag, 
			};
			roofHeight = (flag ? closestHit.distance : 10f);
			return !flag;
		}

		private static bool IsClearDirectional(Vector2 origin, float direction, float length, int layerMask, bool drawDebugLines)
		{
			Vector2 vector = new Vector2(direction, 0f);
			bool num = Helper.IsRayHittingNoTriggers(origin, vector, length, layerMask);
			if (drawDebugLines)
			{
				DrawDebugLine(origin, origin + vector * length, Color.yellow);
			}
			if (num)
			{
				return false;
			}
			try
			{
				while (length > 0f)
				{
					Vector2 vector2 = origin + vector * length;
					int num2 = Physics2D.RaycastNonAlloc(vector2, Vector2.down, _rayHitStoreA, 1.8f, 256);
					bool flag = false;
					RaycastHit2D raycastHit2D = default(RaycastHit2D);
					for (int i = 0; i < Mathf.Min(num2, _rayHitStoreA.Length); i++)
					{
						RaycastHit2D raycastHit2D2 = _rayHitStoreA[i];
						if (!raycastHit2D2.collider.isTrigger && !(raycastHit2D2.distance > 0.5f))
						{
							flag = true;
							raycastHit2D = raycastHit2D2;
							break;
						}
					}
					length -= 0.5f;
					if (drawDebugLines)
					{
						DrawDebugLine(vector2, vector2 + Vector2.down * 1.8f, Color.yellow);
					}
					if (!flag)
					{
						return false;
					}
					switch (NoTeleportRegion.GetTeleportBlockedState(vector2))
					{
					case NoTeleportRegion.TeleportAllowState.Blocked:
						return false;
					case NoTeleportRegion.TeleportAllowState.Allowed:
						continue;
					}
					Vector2 vector3 = raycastHit2D.point - new Vector2(0f, 1.5f);
					int num3 = num2;
					int a = Physics2D.RaycastNonAlloc(vector3, Vector2.up, _rayHitStoreB, 1.4f, 256);
					bool flag2 = false;
					RaycastHit2D raycastHit2D3 = default(RaycastHit2D);
					for (int j = 0; j < Mathf.Min(a, _rayHitStoreB.Length); j++)
					{
						RaycastHit2D raycastHit2D4 = _rayHitStoreB[j];
						if (raycastHit2D4.collider.isTrigger)
						{
							continue;
						}
						flag2 = true;
						raycastHit2D3 = raycastHit2D4;
						bool flag3 = false;
						for (int k = 0; k < Mathf.Min(num2, _rayHitStoreA.Length); k++)
						{
							RaycastHit2D raycastHit2D5 = _rayHitStoreA[k];
							if (!(raycastHit2D5.collider != raycastHit2D4.collider) && !(Math.Abs(raycastHit2D5.point.y - raycastHit2D4.point.y) < 0.01f))
							{
								flag3 = true;
								break;
							}
						}
						if (flag3)
						{
							num3--;
						}
					}
					if (num3 > 0)
					{
						flag2 = false;
					}
					if (drawDebugLines)
					{
						DrawDebugLine(vector3, vector3 + Vector2.up * 1.4f, new Color(1f, 1f, 0f, 0.5f));
					}
					if (flag2)
					{
						if (drawDebugLines)
						{
							DrawDebugLine(vector3, raycastHit2D3.point, Color.red);
						}
						return false;
					}
				}
			}
			finally
			{
				for (int l = 0; l < _rayHitStoreA.Length; l++)
				{
					_rayHitStoreA[l] = default(RaycastHit2D);
				}
				for (int m = 0; m < _rayHitStoreB.Length; m++)
				{
					_rayHitStoreB[m] = default(RaycastHit2D);
				}
			}
			return true;
		}

		private static void DrawDebugLine(Vector2 startPoint, Vector2 endPoint, Color color)
		{
			if ((object)_debugLineMaterial == null)
			{
				_debugLineMaterial = new Material(Shader.Find("Sprites/Default"));
			}
			GL.PushMatrix();
			_debugLineMaterial.SetPass(0);
			GL.Begin(1);
			GL.Color(color);
			GL.Vertex(startPoint);
			GL.Vertex(endPoint);
			GL.End();
			GL.PopMatrix();
		}
	}
}
