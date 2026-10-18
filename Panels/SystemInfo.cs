using DesharpCore.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace DesharpCore.Panels {
	[ComVisible(true)]
	public class SystemInfo: IPanel {
		public static string PanelName = "systeminfo";
		public int[] DefaultWindowSizes => new int[] { 600, 600 };
		public bool AddIfEmpty => true;
		public string IconValue => SystemInfo.PanelName;
		public string Name => SystemInfo.PanelName;
		public PanelIconType PanelIconType => PanelIconType.Class;
		public PanelType PanelType => PanelType.BarBtnAndWindow;

		protected string title = "";
		protected StringBuilder content = new StringBuilder();

		public string[] RenderBarTitle () {
			this.completeTitleAndContent();
			return new string[] { this.title, "System Info" };
		}
		public string RenderWindowContent () {
			return this.content.ToString();
		}
		protected virtual void completeTitleAndContent () {
			string msFormat = "0.###";
			string mbFormat = "0.00";
			string bFormat = "0,000.###";
			CultureInfo formatInfo = new CultureInfo("en-US");
			Process currentProcess = Process.GetCurrentProcess();
			HttpContext context = WebContext.Current;
			string ramPeakWorking = (currentProcess.PeakWorkingSet64 / 1048576.0)
				.ToString(mbFormat, formatInfo) + " MB (" + currentProcess.PeakWorkingSet64.ToString(bFormat, formatInfo) + " bytes)";
			string ramPeakPaged = (currentProcess.PeakPagedMemorySize64 / 1048576.0)
				.ToString(mbFormat, formatInfo) + " MB (" + currentProcess.PeakPagedMemorySize64.ToString(bFormat, formatInfo) + " bytes)";
			string requestTime = (DateTime.Now - WebContext.RequestStarted).TotalMilliseconds
				.ToString(msFormat, formatInfo) + " ms";
			long gcTotalMemoryLong = GC.GetTotalMemory(true);
			string gcTotalMemory = (gcTotalMemoryLong / 1048576.0)
				.ToString(mbFormat, formatInfo) + " MB (" + gcTotalMemoryLong.ToString(bFormat, formatInfo) + " bytes)";
			this.title = requestTime;
			this.content.Append(@"<table class=""system-info""><tbody>");
			string fullUrl = HttpUtility.HtmlEncode(context.Request.GetDisplayUrl());
			fullUrl = fullUrl.Replace("?", "\n\t?").Replace("&amp;", "\n\t&amp;");
			this
				.addContentTableRow("URL", fullUrl, "url")
				.addContentTableRow("Execution time", requestTime)
				.addContentTableRow("GC RAM", gcTotalMemory)
				.addContentTableRow("Server working RAM peak", ramPeakWorking)
				.addContentTableRow("Server pager RAM peak", ramPeakPaged)
				.addContentTableRow("Your IP", context.Connection.RemoteIpAddress?.ToString() ?? "")
				.addContentTableRow("HTTP method / response code", context.Request.Method + " / " + context.Response.StatusCode.ToString())
				.addContentTableRow("Runtime Version", Environment.Version.ToString())
				.addContentTableRow("ASP.NET Core Version", typeof(HttpContext).Assembly.GetName().Version.ToString())
				.addContentTableRow("DesharpCore", Assembly.GetExecutingAssembly().GetName().Version.ToString())
				.addContentTableRow("Server", context.Features.Get<Microsoft.AspNetCore.Http.Features.IServerVariablesFeature>()?["SERVER_SOFTWARE"] ?? "");
			this.content.Append("</tbody></table>");
		}
		protected virtual SystemInfo addContentTableRow (string labelText, string contenText, string cssClass = null) {
			this.content.Append(
                "<tr" + (
                    String.IsNullOrEmpty(cssClass) ? "" : @" class=""" + cssClass + @""""
                ) + "><th>" 
                    + labelText 
                + "</th><td>" 
                    + contenText.ToString() 
                + "</td></tr>"
            );
			return this;
		}
	}
}
