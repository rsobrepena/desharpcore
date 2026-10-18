using System;

namespace DesharpCore {
	internal struct ExceptionToRender {
		internal Exception Exception;
		internal Exception CausedBy;
		internal bool Catched;
	}
}
