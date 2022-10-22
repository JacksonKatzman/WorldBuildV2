using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

public abstract class OutputLogger
{
	private static LogAllowance allowance = LogAllowance.SOME;
	private static bool ALLOW_PAUSES = false;

	private static readonly Dictionary<LogSource, bool> AllowedLogs
	= new Dictionary<LogSource, bool>
{
	{ LogSource.IMPORTANT, true },
	{ LogSource.CITY, false },
	{ LogSource.NAMEGEN, false },
	{ LogSource.WORLDGEN, false },
	{ LogSource.FACTION, false },
	{ LogSource.FACTIONACTION, false },
	{ LogSource.PEOPLE, false },
	{ LogSource.EVENT, true },
	{ LogSource.PROFILE, true },
	{ LogSource.MAIN, false }
};

	public static void LogFormat(string format, LogSource source, params object[] args)
	{
		if((allowance == LogAllowance.ALL) || (allowance == LogAllowance.SOME && AllowedLogs[source]))
		{
			Debug.LogFormat(format, args);
		}
	}

	public static void Log(string log)
	{
		Debug.Log(log);
	}

	public static void LogWarning(string log)
	{
		Debug.LogWarning(log);
	}

	public static void LogError(string log)
	{
		Debug.LogError(log);
	}
}
