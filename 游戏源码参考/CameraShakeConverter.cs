public static class CameraShakeConverter
{
	public static CameraShakeWorldForceFlag ToFlag(this CameraShakeWorldForceIntensities intensity)
	{
		return intensity switch
		{
			CameraShakeWorldForceIntensities.None => CameraShakeWorldForceFlag.None, 
			CameraShakeWorldForceIntensities.Small => CameraShakeWorldForceFlag.Small, 
			CameraShakeWorldForceIntensities.Medium => CameraShakeWorldForceFlag.Medium, 
			CameraShakeWorldForceIntensities.Intense => CameraShakeWorldForceFlag.Intense, 
			_ => CameraShakeWorldForceFlag.None, 
		};
	}

	public static CameraShakeWorldForceFlag ToFlagMax(this CameraShakeWorldForceIntensities intensity)
	{
		return intensity switch
		{
			CameraShakeWorldForceIntensities.None => CameraShakeWorldForceFlag.None, 
			CameraShakeWorldForceIntensities.Small => CameraShakeWorldForceFlag.Small, 
			CameraShakeWorldForceIntensities.Medium => CameraShakeWorldForceFlag.Medium, 
			CameraShakeWorldForceIntensities.Intense => CameraShakeWorldForceFlag.Intense, 
			_ => CameraShakeWorldForceFlag.None, 
		};
	}

	public static CameraShakeWorldForceIntensities ToIntensity(this CameraShakeWorldForceFlag flag)
	{
		return flag switch
		{
			CameraShakeWorldForceFlag.None => CameraShakeWorldForceIntensities.None, 
			CameraShakeWorldForceFlag.Small => CameraShakeWorldForceIntensities.Small, 
			CameraShakeWorldForceFlag.Medium => CameraShakeWorldForceIntensities.Medium, 
			CameraShakeWorldForceFlag.Intense => CameraShakeWorldForceIntensities.Intense, 
			_ => CameraShakeWorldForceIntensities.None, 
		};
	}
}
