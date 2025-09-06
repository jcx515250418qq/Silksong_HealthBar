using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GamepadVibrationMixer : VibrationMixer
{
	public sealed class GamepadVibrationEmission : VibrationEmission
	{
		public struct Values
		{
			public float Small;

			public float Large;

			public bool IsNearlyZero
			{
				get
				{
					if (Small < Mathf.Epsilon)
					{
						return Large < Mathf.Epsilon;
					}
					return false;
				}
			}
		}

		private GamepadVibrationMixer mixer;

		private GamepadVibration gamepadVibration;

		private float duration;

		private bool isLooping;

		private bool isPlaying;

		private bool isRealtime;

		private string tag;

		private VibrationTarget target;

		private float timer;

		public override bool IsLooping
		{
			get
			{
				return isLooping;
			}
			set
			{
				isLooping = value;
			}
		}

		public override bool IsPlaying => isPlaying;

		public override bool IsRealtime => isRealtime;

		public override string Tag
		{
			get
			{
				return tag;
			}
			set
			{
				tag = value;
			}
		}

		public override VibrationTarget Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public override float Time
		{
			get
			{
				return timer;
			}
			set
			{
				timer = value;
			}
		}

		public GamepadVibrationEmission(GamepadVibrationMixer mixer, GamepadVibration gamepadVibration, bool isLooping, string tag, VibrationTarget target, bool isRealtime, float strength)
		{
			this.mixer = mixer;
			this.gamepadVibration = gamepadVibration;
			duration = gamepadVibration.GetDuration();
			this.isLooping = isLooping;
			isPlaying = true;
			this.isRealtime = isRealtime;
			this.tag = tag;
			this.target = target;
			BaseStrength = strength;
		}

		public override void Play()
		{
			isPlaying = true;
		}

		public override void Stop()
		{
			isPlaying = false;
		}

		public Values GetCurrentValues()
		{
			Values result = default(Values);
			result.Small = ((target.Motors != 0) ? (gamepadVibration.SmallMotor.Evaluate(timer) * Strength) : 0f);
			result.Large = ((target.Motors != 0) ? (gamepadVibration.LargeMotor.Evaluate(timer) * Strength) : 0f);
			return result;
		}

		public void Advance(float deltaTime)
		{
			timer += deltaTime * gamepadVibration.PlaybackRate * Speed;
			if (timer >= duration)
			{
				if (isLooping)
				{
					timer = Mathf.Repeat(timer, duration);
				}
				else
				{
					isPlaying = false;
				}
			}
		}

		public override string ToString()
		{
			if (!(gamepadVibration != null))
			{
				return "null";
			}
			return gamepadVibration.name;
		}
	}

	public enum PlatformAdjustments
	{
		None = 0,
		DualShock = 1
	}

	private bool isPaused;

	private List<GamepadVibrationEmission> playingEmissions;

	private PlatformAdjustments platformAdjustment;

	private GamepadVibrationEmission.Values currentValues;

	public static bool AllowPlatformAdjustment { get; set; } = true;

	public static float AdjustmentFactor { get; set; } = 0.5f;

	public override bool IsPaused
	{
		get
		{
			return isPaused;
		}
		set
		{
			isPaused = value;
		}
	}

	public override int PlayingEmissionCount => playingEmissions.Count;

	public GamepadVibrationEmission.Values CurrentValues => currentValues;

	public override VibrationEmission GetPlayingEmission(int playingEmissionIndex)
	{
		return playingEmissions[playingEmissionIndex];
	}

	public GamepadVibrationMixer(PlatformAdjustments platformAdjustment = PlatformAdjustments.None)
	{
		this.platformAdjustment = platformAdjustment;
		playingEmissions = new List<GamepadVibrationEmission>(25);
	}

	public override VibrationEmission PlayEmission(VibrationData vibrationData, VibrationTarget vibrationTarget, bool isLooping, string tag, bool isRealtime)
	{
		if (vibrationData.GamepadVibration == null)
		{
			return null;
		}
		GamepadVibrationEmission gamepadVibrationEmission = new GamepadVibrationEmission(this, vibrationData.GamepadVibration, isLooping, tag, vibrationTarget, isRealtime, vibrationData.Strength);
		playingEmissions.Add(gamepadVibrationEmission);
		return gamepadVibrationEmission;
	}

	public override VibrationEmission PlayEmission(VibrationEmission emission)
	{
		if (emission == null)
		{
			return null;
		}
		if (emission is GamepadVibrationEmission item)
		{
			emission.Play();
			playingEmissions.Add(item);
		}
		return emission;
	}

	public override void StopAllEmissions()
	{
		for (int i = 0; i < playingEmissions.Count; i++)
		{
			playingEmissions[i].Stop();
		}
	}

	public override void StopAllEmissionsWithTag(string tag)
	{
		for (int i = 0; i < playingEmissions.Count; i++)
		{
			GamepadVibrationEmission gamepadVibrationEmission = playingEmissions[i];
			if (gamepadVibrationEmission.Tag == tag)
			{
				gamepadVibrationEmission.Stop();
			}
		}
	}

	public void Update(float deltaTime)
	{
		GamepadVibrationEmission.Values values = default(GamepadVibrationEmission.Values);
		values.Small = 0f;
		values.Large = 0f;
		GamepadVibrationEmission.Values values2 = values;
		if (!isPaused)
		{
			playingEmissions.RemoveAll((GamepadVibrationEmission emission) => !emission.IsPlaying);
			foreach (GamepadVibrationEmission playingEmission in playingEmissions)
			{
				if (playingEmission.IsRealtime || !(Time.timeScale <= Mathf.Epsilon))
				{
					GamepadVibrationEmission.Values values3 = playingEmission.GetCurrentValues();
					values2.Small = AdjustForPlatform(Mathf.Max(values2.Small, values3.Small));
					values2.Large = AdjustForPlatform(Mathf.Max(values2.Large, values3.Large));
					playingEmission.Advance(deltaTime);
				}
			}
		}
		currentValues = values2;
	}

	private float AdjustForPlatform(float val)
	{
		if (platformAdjustment == PlatformAdjustments.DualShock && AllowPlatformAdjustment)
		{
			float b = Mathf.Clamp01(Mathf.Sin(val * MathF.PI * 0.5f));
			val = Mathf.Lerp(val, b, AdjustmentFactor);
		}
		return val * VibrationManager.StrengthMultiplier;
	}
}
