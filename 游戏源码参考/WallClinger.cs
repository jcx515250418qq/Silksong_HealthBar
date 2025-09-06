using UnityEngine;

[RequireComponent(typeof(tk2dSpriteAnimator), typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class WallClinger : MonoBehaviour
{
	private const float SKIN_WIDTH = 0.5f;

	public bool IsSpriteFacingRight;

	public bool StartInactive;

	[Space]
	public float MoveSpeed;

	[Space]
	public float EdgePaddingTop;

	public float EdgePaddingBottom;

	public float PaddingTop;

	public float PaddingBottom;

	[Space]
	public string ClimbUpAnim;

	public string ClimbDownAnim;

	private tk2dSpriteAnimator animator;

	private BoxCollider2D collider;

	private Rigidbody2D body;

	private bool _isActive;

	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			if (_isActive && !value)
			{
				Rigidbody2D rigidbody2D = body;
				float? y = 0f;
				rigidbody2D.SetVelocity(null, y);
			}
			else if (!_isActive && value)
			{
				StartMovingDirection();
			}
			_isActive = value;
		}
	}

	private void OnDrawGizmosSelected()
	{
		collider = GetComponent<BoxCollider2D>();
		if ((bool)collider)
		{
			DoRayCasts(drawingGizmos: true, gizmoDirection: true);
			DoRayCasts(drawingGizmos: true, gizmoDirection: false);
		}
	}

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		collider = GetComponent<BoxCollider2D>();
		body = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		IsActive = !StartInactive;
	}

	private void FixedUpdate()
	{
		if (IsActive)
		{
			DoRayCasts(drawingGizmos: false, gizmoDirection: false);
		}
	}

	private void DoRayCasts(bool drawingGizmos, bool gizmoDirection)
	{
		Vector2 vector = collider.size / 2f;
		Vector2 vector2 = collider.offset - vector;
		Vector2 vector3 = collider.offset + vector;
		if (!IsSpriteFacingRight)
		{
			float x = vector2.x;
			vector2.x = vector3.x;
			vector3.x = x;
		}
		bool flag = ((!drawingGizmos) ? (body.linearVelocity.y > 0f) : gizmoDirection);
		Vector2 vector4 = (flag ? Vector2.up : Vector2.down);
		Vector2 vector5 = (IsSpriteFacingRight ? Vector2.right : Vector2.left);
		Vector2 vector6 = new Vector2(vector3.x, flag ? vector3.y : vector2.y);
		float x2 = (IsSpriteFacingRight ? (-0.5f) : 0.5f);
		Vector2 vector7 = vector6 + new Vector2(x2, flag ? EdgePaddingTop : (0f - EdgePaddingBottom));
		Vector2 vector8 = vector6 + new Vector2(x2, flag ? (-0.5f) : 0.5f);
		float num = 1f;
		float num2 = (flag ? PaddingTop : PaddingBottom) + 0.5f;
		if (!drawingGizmos)
		{
			num2 = Mathf.Max(num2, Mathf.Abs(body.linearVelocity.y * Time.deltaTime));
			bool num3 = IsRayHittingLocal(vector8, vector4, num2);
			bool flag2 = IsRayHittingLocal(vector7, vector5, num);
			if (num3 || !flag2)
			{
				StartMovingDirection((int)Mathf.Sign(0f - body.linearVelocity.y));
			}
		}
		else
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(vector7, vector7 + vector5 * num);
			Gizmos.DrawLine(vector8, vector8 + vector4 * num2);
		}
	}

	private bool IsRayHittingLocal(Vector2 originLocal, Vector2 directionLocal, float length)
	{
		return base.transform.IsRayHittingLocal(originLocal, directionLocal, length, 256);
	}

	public void StartMovingDirection(int direction = 0)
	{
		_isActive = true;
		if (direction == 0)
		{
			direction = ((Random.Range(0, 2) > 0) ? 1 : (-1));
		}
		Rigidbody2D rigidbody2D = body;
		float? y = (float)direction * MoveSpeed;
		rigidbody2D.SetVelocity(null, y);
		string value = ((direction > 0) ? ClimbUpAnim : ClimbDownAnim);
		if (!string.IsNullOrEmpty(value))
		{
			animator.Play(value);
		}
	}
}
