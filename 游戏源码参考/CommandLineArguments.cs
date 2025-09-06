using System;
using UnityEngine;

public static class CommandLineArguments
{
	private const StringComparison ArgumentComparison = StringComparison.OrdinalIgnoreCase;

	private const string ShowPerformanceHUDFlag = "--show-performance-hud";

	private const string RemoteSaveDirectoryPrefix = "--remote-save-directory=";

	private const string EnableDeveloperCheatsFlag = "--enable-developer-cheats";

	public static bool ShowPerformanceHUD { get; private set; }

	public static string RemoteSaveDirectory { get; private set; }

	public static bool EnableDeveloperCheats { get; private set; }

	static CommandLineArguments()
	{
		if (Application.isEditor)
		{
			return;
		}
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		if (commandLineArgs == null)
		{
			return;
		}
		foreach (string text in commandLineArgs)
		{
			if (text.Equals("--show-performance-hud", StringComparison.OrdinalIgnoreCase))
			{
				ShowPerformanceHUD = true;
			}
			else if (text.Equals("--enable-developer-cheats", StringComparison.OrdinalIgnoreCase))
			{
				EnableDeveloperCheats = true;
			}
			else if (text.StartsWith("--remote-save-directory=", StringComparison.OrdinalIgnoreCase))
			{
				RemoteSaveDirectory = text.Substring("--remote-save-directory=".Length);
			}
		}
	}
}
