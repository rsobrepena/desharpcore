using Newtonsoft.Json;

namespace DesharpCore.Core {
	/// <summary>
	/// Thin internal wrapper around Newtonsoft.Json used for all JSON serialization.
	/// </summary>
	internal class JsonSerializer {
		public string Serialize(object obj) {
			return JsonConvert.SerializeObject(obj);
		}
		public T Deserialize<T>(string input) {
			return JsonConvert.DeserializeObject<T>(input);
		}
	}
}
