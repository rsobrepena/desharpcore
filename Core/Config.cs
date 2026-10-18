using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace DesharpCore.Core {
	internal class Config {
		internal const string APP_SETTINGS_ENABLED = "DesharpCore:Enabled"; // true | false | 1 | 0
		internal const string APP_SETTINGS_EDITOR = "DesharpCore:Editor"; // MSVS2005 | MSVS2008 | MSVS2010 | MSVS2012 | MSVS2013 | MSVS2015 | MSVS2017
		internal const string APP_SETTINGS_OUTPUT = "DesharpCore:Output"; // text | html
		internal const string APP_SETTINGS_DEBUG_IPS = "DesharpCore:DebugIps"; // 127.0.0.1,88.31.45.67,...
		internal const string APP_SETTINGS_LEVELS = "DesharpCore:Levels"; // exception,-debug,info,notice,warning,error,critical,alert,emergency,-javascript
		internal const string APP_SETTINGS_PANELS = "DesharpCore:Panels"; // DesharpCore.Panels.Session,DesharpCore.Panels.Routing
		internal const string APP_SETTINGS_DIRECTORY = "DesharpCore:Directory"; // ~/logs
		internal const string APP_SETTINGS_WRITE_MILISECONDS = "DesharpCore:WriteMiliseconds"; // 0
		internal const string APP_SETTINGS_ERROR_PAGE = "DesharpCore:ErrorPage"; // ~/path/to/custom/error-page-500.html
		internal const string APP_SETTINGS_DEPTH = "DesharpCore:Depth"; // 3
		internal const string APP_SETTINGS_MAX_LENGTH = "DesharpCore:MaxLength"; // 1024
		internal const string APP_SETTINGS_SOURCE_LOC = "DesharpCore:SourceLocation"; // true | false | 1 | 0
		internal const string APP_SETTINGS_NOTIFY_SETTINGS = "DesharpCore:NotifySettings"; // { host: 'smtp.mailbox.com', port: 587, ssl: true, user: 'username', password: '1234', from: 'desharp@app.com', to: 'username@mailbox.com', priority: 'high', timeout: 30000 }
        internal const string APP_SETTINGS_DUMP_COMPILLER_GENERATED = "DesharpCore:DumpCompillerGenerated"; // false
        private static Dictionary<string, string> _appSettings = new Dictionary<string, string>();
		static Config () {
			try {
				string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
					?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
				IConfigurationBuilder builder = new ConfigurationBuilder()
					.SetBasePath(AppContext.BaseDirectory)
					.AddJsonFile("appsettings.json", true, false);
				if (!String.IsNullOrEmpty(environmentName))
					builder.AddJsonFile($"appsettings.{environmentName}.json", true, false);
				IConfiguration configuration = builder.AddEnvironmentVariables().Build();
				foreach (KeyValuePair<string, string> item in configuration.AsEnumerable()) {
					if (item.Value == null) continue;
					string itemKey = item.Key.Trim();
					if (itemKey.ToLower().IndexOf("desharpcore:") == 0) {
						Config._appSettings["DesharpCore:" + itemKey.Substring("desharpcore:".Length)] = item.Value.Trim();
					}
				}
			} catch { }
		}
		internal static bool? GetEnabled () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_ENABLED)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_ENABLED].Trim().ToLower();
				return (rawValue == "false" || rawValue == "0" || rawValue == "") ? false : true;
			} else {
				return null;
			}
		}
		internal static Type[] GetDebugPanels () {
			List<Type> result = new List<Type>();
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_PANELS)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_PANELS].Trim();
				Regex r = new Regex(@"[^a-zA-Z0-9_\,\.]");
				rawValue = r.Replace(rawValue, "");
				List<string> rawItems = rawValue.Split(',').ToList<string>();
				Type value;
				foreach (string rawItem in rawItems) {
					try {
						value = Type.GetType(rawItem, true);
						if (value != null && !result.Contains(value)) {
							result.Add(value);
						}
					} catch { }
				}
			}
			return result.ToArray();
		}
		internal static string GetEditor () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_EDITOR)) {
				return Config._appSettings[Config.APP_SETTINGS_EDITOR].Trim();
			} else {
				return null;
			}
		}
		internal static string GetErrorPage () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_ERROR_PAGE)) {
				return Config._appSettings[Config.APP_SETTINGS_ERROR_PAGE].Trim();
			}
			return "";
		}
		internal static LogFormat? GetLogFormat () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_OUTPUT)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_OUTPUT].Trim().ToLower();
				if (rawValue == "html") return LogFormat.Html;
				if (rawValue == "text") return LogFormat.Text;
			}
			return null;
		}
		internal static List<string> GetDebugIps () {
			List<string> result = new List<string>();
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_DEBUG_IPS)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_DEBUG_IPS].Trim().ToLower();
				Regex r = new Regex(@"[^a-f0-9\.\,:]");
				rawValue = r.Replace(rawValue, "");
				result = rawValue.Split(',').ToList<string>();
			}
			return result;
		}
		internal static Dictionary<string, int> GetLevels () {
			Dictionary<string, int> result = new Dictionary<string, int>();
			List<string> allPossiblelevels = LevelValues.Values.Values.ToList<string>();
			allPossiblelevels.Add("exception");
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_LEVELS)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_LEVELS].Trim().ToLower();
				Regex r = new Regex(@"[^a-z\,\-\+]");
				rawValue = r.Replace(rawValue, "");
				List<string> rawItems = rawValue.Split(',').ToList<string>();
				string key;
				int value;
				foreach (string rawItem in rawItems) {
					if (rawItem.Substring(0, 1) == "-") {
						key = rawItem.Substring(1);
						value = 0;
					} else if (rawItem.Substring(0, 1) == "+") {
						key = rawItem.Substring(1);
						value = 2;
					} else {
						key = rawItem;
						value = 1;
					}
					if (!result.ContainsKey(key)) 
						result[key] = value;
				}
			}
			int allLevelsDefaultValue = result.Count > 0 ? 0 : 1;
			foreach (string levelKey in allPossiblelevels) 
				if (!result.ContainsKey(levelKey))
					result.Add(levelKey, allLevelsDefaultValue);
			return result;
		}
		internal static int GetLogWriteMilisecond () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_WRITE_MILISECONDS)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_WRITE_MILISECONDS].Trim();
				rawValue = new Regex("[^0-9]").Replace(rawValue, "");
				if (rawValue.Length > 0) {
					return Int32.Parse(rawValue);
				}
			}
			return 0;
		}
		internal static string GetDirectory () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_DIRECTORY)) {
				return Config._appSettings[Config.APP_SETTINGS_DIRECTORY].Trim();
			}
			return "";
        }
        internal static int GetDepth () {
            if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_DEPTH)) {
                string rawValue = Config._appSettings[Config.APP_SETTINGS_DEPTH].Trim();
                rawValue = new Regex("[^0-9]").Replace(rawValue, "");
                if (rawValue.Length > 0) { 
                    return Int32.Parse(rawValue);
                }
            }
            return 0;
		}
		internal static int GetMaxLength () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_MAX_LENGTH)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_MAX_LENGTH].Trim();
				rawValue = new Regex("[^0-9]").Replace(rawValue, "");
				if (rawValue.Length > 0) {
					return Int32.Parse(rawValue);
				}
			}
			return 0;
        }
		internal static bool? GetSourceLocation () {
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_SOURCE_LOC)) {
				string rawValue = Config._appSettings[Config.APP_SETTINGS_SOURCE_LOC].Trim().ToLower();
				return (rawValue == "false" || rawValue == "0" || rawValue == "") ? false : true;
			} else {
				return null;
			}
		}
        internal static bool? GetDumpCompillerGenerated () {
            if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_DUMP_COMPILLER_GENERATED)) {
                string rawValue = Config._appSettings[Config.APP_SETTINGS_DUMP_COMPILLER_GENERATED].Trim().ToLower();
                return (rawValue == "false" || rawValue == "0" || rawValue == "") ? false : true;
            } else {
                return null;
            }
        }
        internal static Dictionary<string, object> GetNotifySettings () {
			Dictionary<string, object> result = new Dictionary<string, object>();
			if (Config._appSettings.ContainsKey(Config.APP_SETTINGS_NOTIFY_SETTINGS)) {
				string rawJson = Config._appSettings[Config.APP_SETTINGS_NOTIFY_SETTINGS].Trim();
				try {
					// remove all new lines and tabs
					Regex r1 = new Regex("[\r\n\t]");
					rawJson = r1.Replace(rawJson, "");
					// fix all keys with missing begin and end double quotes
					Regex r2 = new Regex(@"([^""])([a-zA-Z]+):");
					rawJson = r2.Replace(rawJson, @"$1""$2"":");
					// change all values with single quots to double quots
					Regex r3 = new Regex(@"'([^']*)'");
					rawJson = r3.Replace(rawJson, @"""$1""");
					// remove all double dots with leading space to double dots only
					Regex r4 = new Regex(@""":\s");
					rawJson = r4.Replace(rawJson, @""":");
					// remove all spaces between value and another key
					Regex r5 = new Regex(@""",\s+""");
					rawJson = r5.Replace(rawJson, @""",""");
					Regex r6 = new Regex(@"""\s+\}");
					rawJson = r6.Replace(rawJson, @"""}");
					// so let's deserialize string data
					JsonSerializer jsonSerializer = new JsonSerializer();
					result = jsonSerializer.Deserialize<Dictionary<string, object>>(rawJson);
				} catch (Exception ex) {
					result.Add("error", ex.Message);
				}
			}
			if (result.ContainsKey("port")) {
				string portStr = result["port"].ToString();
				int port = 25;
				Int32.TryParse(portStr, out port);
				result["port"] = port;
			}
			if (result.ContainsKey("timeout")) {
				string timeoutStr = result["timeout"].ToString();
				int timeout = 10000;
				Int32.TryParse(timeoutStr, out timeout);
				result["timeout"] = timeout;
			}
			if (result.ContainsKey("ssl")) {
				result["ssl"] = Boolean.Parse(result["ssl"].ToString().ToLower());
			} else {
				result["ssl"] = false;
			}
			if (result.ContainsKey("background")) {
				result["background"] = Boolean.Parse(result["background"].ToString().ToLower());
			} else {
				result["background"] = false;
			}
			return result;
		}
	}
}
