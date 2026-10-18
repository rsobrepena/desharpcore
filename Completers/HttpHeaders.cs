using DesharpCore.Core;
using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace DesharpCore.Completers {
	internal class HttpHeaders {
		internal static List<string[]> CompletePossibleHttpHeaders () {
			List<string[]> headers = new List<string[]>();
			try {
				HttpRequest request = WebContext.Current.Request;
				string fullUrl = HttpUtility.HtmlEncode(request.GetDisplayUrl());
				headers.Add(new string[] { "URL", fullUrl });
				headers.Add(new string[] { "IP", Tools.GetClientIpAddress() });
				foreach (var header in request.Headers) {
					string headerName = header.Key;
					string headerValue = HttpUtility.HtmlEncode(header.Value.ToString());
					if (headerName.Length > Dispatcher.DumpMaxLength)
						headerName = headerName.Substring(0, Dispatcher.DumpMaxLength);
					if (headerValue.Length > Dispatcher.DumpMaxLength)
						headerValue = headerValue.Substring(0, Dispatcher.DumpMaxLength);
					headers.Add(new string[] { headerName, headerValue });
				}
			} catch { }
			return headers;
		}
	}
}
