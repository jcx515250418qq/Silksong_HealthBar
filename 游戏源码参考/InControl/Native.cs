using System;
using System.Runtime.InteropServices;

namespace InControl
{
	internal static class Native
	{
		private const string libraryName = "InControlNative";

		private const CallingConvention callingConvention = CallingConvention.Cdecl;

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_Init")]
		public static extern void Init(NativeInputOptions options);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_Stop")]
		public static extern void Stop();

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetVersionInfo")]
		public static extern void GetVersionInfo(out NativeVersionInfo versionInfo);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetDeviceInfo")]
		public static extern bool GetDeviceInfo(uint handle, out InputDeviceInfo deviceInfo);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetDeviceState")]
		public static extern bool GetDeviceState(uint handle, out IntPtr deviceState);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetDeviceEvents")]
		public static extern int GetDeviceEvents(out IntPtr deviceEvents);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetHapticState")]
		public static extern void SetHapticState(uint handle, byte lowFrequency, byte highFrequency);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetTriggersHapticState")]
		public static extern void SetTriggersHapticState(uint handle, byte leftTrigger, byte rightTrigger);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetLightColor")]
		public static extern void SetLightColor(uint handle, byte red, byte green, byte blue);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetLightFlash")]
		public static extern void SetLightFlash(uint handle, byte flashOnDuration, byte flashOffDuration);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetAnalogGlyphName")]
		public static extern uint GetAnalogGlyphName(uint handle, uint index, out IntPtr glyphName);

		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetButtonGlyphName")]
		public static extern uint GetButtonGlyphName(uint handle, uint index, out IntPtr glyphName);
	}
}
