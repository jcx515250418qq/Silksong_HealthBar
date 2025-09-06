using GlobalSettings;
using UnityEngine;

public class GrassBehaviour : MonoBehaviour, IUpdateBatchableFixedUpdate, IUpdateBatchableLateUpdate
{
	[Header("Animation")]
	public float walkReactAmount = 1f;

	public AnimationCurve walkReactCurve;

	public float walkReactLength;

	[Space]
	public float attackReactAmount = 2f;

	public AnimationCurve attackReactCurve;

	public float attackReactLength;

	[Space]
	public bool disableSizeMultiplier;

	[Space]
	public string pushAnim = "Push";

	private Animator animator;

	[Header("Sound")]
	public AudioClip[] pushSounds;

	public AudioClip[] cutSounds;

	private AudioSource source;

	private AnimationCurve curve;

	private float animLength = 2f;

	private float animElapsed;

	private float pushAmount = 1f;

	private float pushDirection;

	private bool returned = true;

	private bool isCut;

	private float pushAmountError;

	private Rigidbody2D player;

	private Collider2D col;

	private SpriteRenderer[] renderers;

	private float camShakePushMultiplier;

	private MaterialPropertyBlock propertyBlock;

	private static double _audioPlayCooldown;

	private static readonly int _pushAmountShaderProp = Shader.PropertyToID("_PushAmount");

	private float previousPushAmount = -1f;

	private bool hasStarted;

	private UpdateBatcher updateBatcher;

	public bool ShouldUpdate => hasStarted;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
		animator = GetComponentInChildren<Animator>();
		col = GetComponent<Collider2D>();
		propertyBlock = new MaterialPropertyBlock();
		updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
		updateBatcher.Add(this);
	}

	private void OnDestroy()
	{
		if ((bool)updateBatcher)
		{
			updateBatcher.Remove(this);
			updateBatcher = null;
		}
	}

	private void Start()
	{
		if (!base.transform.IsOnHeroPlane())
		{
			GrassCut component = GetComponent<GrassCut>();
			if ((bool)component)
			{
				Object.Destroy(component);
			}
			Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i]);
			}
		}
		renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		pushAmountError = Random.Range(-0.01f, 0.01f);
		if (disableSizeMultiplier)
		{
			camShakePushMultiplier = 1f;
		}
		else
		{
			float num = 0f;
			SpriteRenderer[] array = renderers;
			foreach (SpriteRenderer spriteRenderer in array)
			{
				SetPushAmount(spriteRenderer, pushAmountError);
				float y = spriteRenderer.bounds.size.y;
				if (y > num)
				{
					num = y;
				}
			}
			camShakePushMultiplier = 0.5f * (3f / num);
		}
		base.transform.SetPositionZ(base.transform.position.z + Random.Range(-0.0001f, 0.0001f));
		hasStarted = true;
	}

	private void OnEnable()
	{
		CameraManagerReference mainCameraShakeManager = GlobalSettings.Camera.MainCameraShakeManager;
		if ((bool)mainCameraShakeManager)
		{
			mainCameraShakeManager.CameraShakedWorldForce += OnCameraShaked;
		}
	}

	private void OnDisable()
	{
		CameraManagerReference mainCameraShakeManager = GlobalSettings.Camera.MainCameraShakeManager;
		if ((bool)mainCameraShakeManager)
		{
			mainCameraShakeManager.CameraShakedWorldForce -= OnCameraShaked;
		}
	}

	public void BatchedLateUpdate()
	{
		if (returned)
		{
			return;
		}
		float num = ((animLength > 0f) ? (curve.Evaluate(animElapsed / animLength) * pushAmount * pushDirection * Mathf.Sign(base.transform.lossyScale.x) + pushAmountError) : 0f);
		if (!Mathf.Approximately(previousPushAmount, num))
		{
			SpriteRenderer[] array = renderers;
			foreach (SpriteRenderer rend in array)
			{
				SetPushAmount(rend, num);
			}
			previousPushAmount = num;
		}
		if (animElapsed >= animLength)
		{
			returned = true;
			if ((bool)animator && animator.HasParameter(pushAnim, AnimatorControllerParameterType.Bool))
			{
				animator.SetBool(pushAnim, value: false);
			}
		}
		animElapsed += Time.deltaTime;
	}

	public void BatchedFixedUpdate()
	{
		if (!player || !returned || !(Mathf.Abs(player.linearVelocity.x) >= 0.1f))
		{
			return;
		}
		pushDirection = Mathf.Sign(player.linearVelocity.x);
		returned = false;
		animElapsed = 0f;
		pushAmount = walkReactAmount;
		curve = walkReactCurve;
		animLength = walkReactLength;
		if (!isCut)
		{
			PlayRandomSound(pushSounds, limitFrequency: false);
		}
		if ((bool)animator)
		{
			if (animator.HasParameter(pushAnim, AnimatorControllerParameterType.Bool))
			{
				animator.SetBool(pushAnim, value: true);
			}
			else if (animator.HasParameter(pushAnim, AnimatorControllerParameterType.Trigger))
			{
				animator.SetTrigger(pushAnim);
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!returned || GameManager.instance.IsInSceneTransition)
		{
			return;
		}
		Transform transform = base.transform;
		Vector2 vector = (col ? col.bounds.center : transform.position);
		float num;
		for (num = transform.eulerAngles.z; num < 0f; num += 360f)
		{
		}
		while (num > 360f)
		{
			num -= 360f;
		}
		Vector2 rhs = new Vector2(Mathf.Cos(num), Mathf.Sin(num));
		Rigidbody2D component = collision.GetComponent<Rigidbody2D>();
		Vector2 lhs = ((!(component != null)) ? (vector - (Vector2)collision.transform.position) : component.linearVelocity);
		if (lhs.magnitude > 0f)
		{
			lhs.Normalize();
		}
		pushDirection = Mathf.Sign(Vector2.Dot(lhs, rhs));
		returned = false;
		animElapsed = 0f;
		if (GrassCut.ShouldCut(collision))
		{
			pushAmount = attackReactAmount;
			curve = attackReactCurve;
			animLength = attackReactLength;
			if (!isCut)
			{
				PlayRandomSound(pushSounds, limitFrequency: false);
			}
		}
		else
		{
			pushAmount = walkReactAmount;
			curve = walkReactCurve;
			animLength = walkReactLength;
			if (collision.CompareTag("Player"))
			{
				player = collision.GetComponent<Rigidbody2D>();
			}
			if (!isCut)
			{
				PlayRandomSound(pushSounds, limitFrequency: false);
			}
		}
		if ((bool)animator && animator.HasParameter(pushAnim, null))
		{
			animator.SetBool(pushAnim, value: true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			player = null;
		}
	}

	public void CutReact(Collider2D collision)
	{
		if (!isCut)
		{
			if ((bool)collision)
			{
				pushDirection = Mathf.Sign(base.transform.position.x - collision.transform.position.x);
			}
			else
			{
				pushDirection = ((Random.Range(0, 2) <= 0) ? 1 : (-1));
			}
			returned = false;
			animElapsed = 0f;
			pushAmount = attackReactAmount;
			curve = attackReactCurve;
			animLength = attackReactLength;
		}
		PlayRandomSound(cutSounds, limitFrequency: true);
		isCut = true;
	}

	public void WindReact(Collider2D collision)
	{
		if (returned)
		{
			pushDirection = Mathf.Sign(base.transform.position.x - collision.transform.position.x);
			returned = false;
			animElapsed = 0f;
			pushAmount = walkReactAmount;
			curve = walkReactCurve;
			animLength = walkReactLength;
			if (!isCut)
			{
				PlayRandomSound(pushSounds, limitFrequency: false);
			}
		}
	}

	private void PlayRandomSound(AudioClip[] clips, bool limitFrequency)
	{
		if (!source || clips.Length == 0)
		{
			return;
		}
		if (limitFrequency)
		{
			if (Time.unscaledTimeAsDouble < _audioPlayCooldown)
			{
				return;
			}
			_audioPlayCooldown = Time.unscaledTimeAsDouble + (double)Audio.AudioEventFrequencyLimit;
		}
		AudioClip clip = clips[Random.Range(0, clips.Length)];
		source.PlayOneShot(clip);
	}

	private void SetPushAmount(Renderer rend, float value)
	{
		rend.GetPropertyBlock(propertyBlock);
		propertyBlock.SetFloat(_pushAmountShaderProp, value);
		rend.SetPropertyBlock(propertyBlock);
	}

	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (intensity >= CameraShakeWorldForceIntensities.Medium)
		{
			pushDirection = ((Random.Range(0, 2) <= 0) ? 1 : (-1));
			returned = false;
			animElapsed = 0f;
			if (intensity <= CameraShakeWorldForceIntensities.Medium)
			{
				pushAmount = walkReactAmount;
				curve = walkReactCurve;
				animLength = walkReactLength;
			}
			else
			{
				pushAmount = attackReactAmount;
				curve = attackReactCurve;
				animLength = attackReactLength;
			}
			pushAmount *= camShakePushMultiplier;
			animLength += Random.Range(-0.3f, 0.3f);
			if ((bool)animator && animator.HasParameter(pushAnim, null))
			{
				animator.SetBool(pushAnim, value: true);
			}
		}
	}
}
