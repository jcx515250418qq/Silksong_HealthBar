using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Translates a Game Object per FixedUpdate, and also raycasts to detect if terrain is passed through. Move on EITHER x or y only! Ie cardinal directions ")]
	public class TranslateContinuous : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Translation along x axis.")]
		public FsmFloat x;

		[Tooltip("Translation along y axis.")]
		public FsmFloat y;

		public FsmInt[] layerMask;

		private GameObject go;

		private int moveDirection;

		private BoxCollider2D collider;

		private Vector2 point1Offset;

		private Vector2 point2Offset;

		private Vector2 point3Offset;

		private Vector2 rayOrigin1;

		private Vector2 rayOrigin2;

		private Vector2 rayOrigin3;

		private Vector2 rayCastDirection;

		private Vector2 debugDirection;

		private float moveDistance;

		private Vector2 translate;

		private bool hitWall;

		public override void Reset()
		{
			gameObject = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			go = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				Finish();
			}
			collider = go.GetComponent<BoxCollider2D>();
			if (x.Value < 0f)
			{
				moveDirection = 2;
				rayCastDirection = new Vector2(1f, 0f);
				moveDistance = x.Value - collider.bounds.size.x;
				debugDirection = new Vector2(x.Value, 0f);
			}
			else if (x.Value > 0f)
			{
				moveDirection = 0;
				rayCastDirection = new Vector2(1f, 0f);
				moveDistance = x.Value + collider.bounds.size.x;
				debugDirection = new Vector2(x.Value, 0f);
			}
			else if (y.Value < 0f)
			{
				moveDirection = 3;
				rayCastDirection = new Vector2(0f, 1f);
				moveDistance = y.Value - collider.bounds.size.y;
				debugDirection = new Vector2(0f, y.Value);
			}
			else if (y.Value > 0f)
			{
				moveDirection = 1;
				rayCastDirection = new Vector2(0f, 1f);
				moveDistance = y.Value + collider.bounds.size.y;
				debugDirection = new Vector2(0f, y.Value);
			}
			if (moveDirection == 0 || moveDirection == 3)
			{
				point1Offset = new Vector2(collider.offset.x - collider.bounds.size.x / 2f, collider.offset.y + collider.bounds.size.y / 2f);
			}
			if (moveDirection == 1 || moveDirection == 2)
			{
				point1Offset = new Vector2(collider.offset.x + collider.bounds.size.x / 2f, collider.offset.y - collider.bounds.size.y / 2f);
			}
			if (moveDirection == 2 || moveDirection == 3)
			{
				point2Offset = new Vector2(collider.offset.x + collider.bounds.size.x / 2f, collider.offset.y + collider.bounds.size.y / 2f);
			}
			if (moveDirection == 0 || moveDirection == 1)
			{
				point2Offset = new Vector2(collider.offset.x - collider.bounds.size.x / 2f, collider.offset.y - collider.bounds.size.y / 2f);
			}
			if (moveDirection == 0)
			{
				point3Offset = new Vector2(collider.offset.x - collider.bounds.size.x / 2f, collider.offset.y);
			}
			else if (moveDirection == 1)
			{
				point3Offset = new Vector2(collider.offset.x, collider.offset.y - collider.bounds.size.y / 2f);
			}
			else if (moveDirection == 2)
			{
				point3Offset = new Vector2(collider.offset.x + collider.bounds.size.x / 2f, collider.offset.y);
			}
			else if (moveDirection == 3)
			{
				point3Offset = new Vector2(collider.offset.x, collider.offset.y + collider.bounds.size.y / 2f);
			}
			DoTranslate();
		}

		public override void OnFixedUpdate()
		{
			DoTranslate();
		}

		private void DoTranslate()
		{
			hitWall = false;
			translate = new Vector2(x.Value, y.Value);
			rayOrigin1 = new Vector2(go.transform.position.x + point1Offset.x, go.transform.position.y + point1Offset.y);
			rayOrigin2 = new Vector2(go.transform.position.x + point2Offset.x, go.transform.position.y + point2Offset.y);
			rayOrigin3 = new Vector2(go.transform.position.x + point3Offset.x, go.transform.position.y + point3Offset.y);
			Debug.DrawLine(rayOrigin2, new Vector2(rayOrigin2.x + moveDistance, rayOrigin2.y), Color.yellow);
			RaycastHit2D raycastHit2D = Helper.Raycast2D(rayOrigin1, rayCastDirection, moveDistance, 256);
			RaycastHit2D raycastHit2D2 = Helper.Raycast2D(rayOrigin2, rayCastDirection, moveDistance, 256);
			RaycastHit2D raycastHit2D3 = Helper.Raycast2D(rayOrigin3, rayCastDirection, moveDistance, 256);
			bool flag = raycastHit2D.collider != null;
			bool flag2 = raycastHit2D2.collider != null;
			bool flag3 = raycastHit2D3.collider != null;
			if (flag || flag2 || flag3)
			{
				float num = 0f;
				if (moveDirection == 2)
				{
					if (flag)
					{
						num = raycastHit2D.point.x;
						if (flag2)
						{
							num = Mathf.Max(num, raycastHit2D2.point.x);
						}
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.x);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.x;
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.x);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.x;
					}
					translate.x += num - (rayOrigin1.x + rayCastDirection.x * moveDistance);
					hitWall = true;
				}
				if (moveDirection == 0)
				{
					if (flag)
					{
						num = raycastHit2D.point.x;
						if (flag2)
						{
							num = Mathf.Min(num, raycastHit2D2.point.x);
						}
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.x);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.x;
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.x);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.x;
					}
					translate.x += num - (rayOrigin1.x + rayCastDirection.x * moveDistance);
					hitWall = true;
				}
				if (moveDirection == 1)
				{
					if (flag)
					{
						num = raycastHit2D.point.y;
						if (flag2)
						{
							num = Mathf.Min(num, raycastHit2D2.point.y);
						}
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.y);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.y;
						if (flag3)
						{
							num = Mathf.Min(num, raycastHit2D3.point.y);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.y;
					}
					translate.y += num - (rayOrigin1.y + rayCastDirection.y * moveDistance);
					hitWall = true;
				}
				if (moveDirection == 3)
				{
					if (flag)
					{
						num = raycastHit2D.point.y;
						if (flag2)
						{
							num = Mathf.Max(num, raycastHit2D2.point.y);
						}
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.y);
						}
					}
					else if (flag2)
					{
						num = raycastHit2D2.point.y;
						if (flag3)
						{
							num = Mathf.Max(num, raycastHit2D3.point.y);
						}
					}
					else if (flag3)
					{
						num = raycastHit2D3.point.y;
					}
					translate.y += num - (rayOrigin1.y + rayCastDirection.y * moveDistance);
					hitWall = true;
				}
			}
			else
			{
				hitWall = false;
			}
			if (hitWall)
			{
				Finish();
			}
			else
			{
				go.transform.Translate(translate, Space.World);
			}
		}
	}
}
