using DesharpCore.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace DesharpCore.Panels {
	[ComVisible(true)]
	public class Session: IPanel, ISessionPanel {
		public static string PanelName = "session";
		public static int DumpDepth = 0;
		public int[] DefaultWindowSizes => new int[] { 600, 600 };
		public bool AddIfEmpty => true;
		public string IconValue => Session.PanelName;
		public string Name => Session.PanelName;
		public PanelType PanelType => PanelType.BarBtnAndWindow;
		public PanelIconType PanelIconType => PanelIconType.Class;

		protected int count = 0;
		protected StringBuilder content = new StringBuilder();

		public void SessionBegin() { }
		public void SessionEnd () {
			ISession session = WebContext.Session;
			if (session == null) return;
			List<string> sessionKeys = new List<string>();
			try {
				foreach (string key in session.Keys) {
					if (key != Debug.SESSION_STORAGE_KEY) sessionKeys.Add(key);
				}
			} catch { }
			this.content
				.Append(@"<div class=""content"">")
				.Append(@"<b class=""heading"">Configuration:</b>")
				.Append(@"<table class=""session-configuration""><tbody>")
				.Append("<tr><th>Id</th><td>" + HttpUtility.HtmlEncode(session.Id) + "</td></tr>")
				.Append("<tr><th>Available</th><td>" + session.IsAvailable + "</td></tr>")
				.Append("</tbody></table>");
			if (sessionKeys.Count == 0)
				this.content.Append(@"<b class=""heading"">No items</b>");
			this.content.Append("</p>");
			this.count = sessionKeys.Count;
			if (sessionKeys.Count > 0) {
				this.content.Append(@"<b class=""heading"">Items:</b>");
				this.content.Append(@"<div class=""inset"">");
				int depth = Session.DumpDepth > 0 ? Session.DumpDepth : Dispatcher.DumpDepth;
				string dumpBeginCode = @"<div class=""desharp-dump"">";
				int beginCodePos = 0;
				string dumpCode;
				foreach (string sessionKey in sessionKeys) {
					session.TryGetValue(sessionKey, out byte[] rawValue);
					string value = rawValue == null ? null : Encoding.UTF8.GetString(rawValue);
					dumpCode = Debug.Dump(value, new DumpOptions {
						Depth = depth,
						Return = true
					});
					beginCodePos = dumpCode.IndexOf(dumpBeginCode);
					if (beginCodePos == -1) beginCodePos = 0;
					dumpCode = dumpCode.Substring(0, beginCodePos) + 
						dumpCode.Substring(beginCodePos, dumpBeginCode.Length) +
						@"<span class=""string"">""" + HttpUtility.HtmlEncode(sessionKey) + @"""</span><s>:&nbsp;</s>" +
						dumpCode.Substring(beginCodePos + dumpBeginCode.Length);
					this.content.Append(dumpCode);
				}
				this.content.Append("</div>");
			}
			this.content.Append("</div>");
		}
		public string[] RenderBarTitle () {
			return new string[] { "Session (" + this.count + ")" };
		}
		public string RenderWindowContent () {
			return this.content.ToString();
		}
	}
}
