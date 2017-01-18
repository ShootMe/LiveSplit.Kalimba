#if LiveSplit
using LiveSplit.Kalimba;
using LiveSplit.UI.Components;
#endif
using System.Reflection;
using System.Runtime.InteropServices;
[assembly: AssemblyTitle("LiveSplit.Kalimba")]
[assembly: AssemblyDescription("Autosplitter for Kalimba")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("LiveSplit.Kalimba")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("b3294e28-2bd4-4e39-92fa-e04a620c7e7f")]
[assembly: AssemblyVersion("1.9.7.0")]
[assembly: AssemblyFileVersion("1.9.7.0")]
#if LiveSplit
[assembly: ComponentFactory(typeof(KalimbaFactory))]
#endif