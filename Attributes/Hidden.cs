using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DesharpCore {
    /// <summary>
    /// Add hidden attribute to hide any property or field in your class 
    /// to print it by DesharpCore.Debug.Dump() or DesharpCore.Debug.Log() methods.
    /// To print always everything, add to your (App|Web).config: 
    /// &#60;add key="Desharp:DumpCompillerGenerated" value="true" /&#62;
    /// </summary>
	[ComVisible(true)]
    public class HiddenAttribute: Attribute {
    }
}
