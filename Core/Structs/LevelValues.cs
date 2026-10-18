using System.Collections.Generic;

namespace DesharpCore {
	internal struct LevelValues {
		internal static Dictionary<Level, string> Values = new Dictionary<Level, string>() {
			{ Level.DEBUG		, "debug" },
			{ Level.INFO		, "info" },
			{ Level.NOTICE		, "notice" },
			{ Level.WARNING		, "warning" },
			{ Level.ERROR		, "error" },
			{ Level.CRITICAL	, "critical" },
			{ Level.ALERT		, "alert" },
			{ Level.EMERGENCY	, "emergency" },
			{ Level.JAVASCRIPT	, "javascript" },
		};
	}
}
