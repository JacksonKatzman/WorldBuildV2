using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

public abstract class OutputLogger
{
	private static bool ALLOW_ALL_LOGS = false;

	private static readonly Dictionary<LogSource, bool> AllowedLogs
	= new Dictionary<LogSource, bool>
{
	{ LogSource.IMPORTANT, true },
	{ LogSource.CITY, false },
	{ LogSource.NAMEGEN, true },
	{ LogSource.WORLDGEN, true },
	{ LogSource.FACTION, true },
	{ LogSource.FACTIONACTION, true },
	{ LogSource.PEOPLE, false },
	{ LogSource.MAIN, true }
};

	public static void LogFormat(string format, LogSource source, params object[] args)
	{
		if(AllowedLogs[source] || ALLOW_ALL_LOGS)
		{
			Debug.LogFormat(format, args);
		}
	}

	public static void LogFormatAndPause(string format, LogSource source, params object[] args)
	{
		Debug.LogFormat(format, args);
		WorldHandler.Instance.DebugPause = true;
	}
}
