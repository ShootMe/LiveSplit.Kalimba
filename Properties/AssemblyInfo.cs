﻿using LiveSplit.Kalimba;
using LiveSplit.UI.Components;
using System.Reflection;
using System.Runtime.InteropServices;
[assembly: AssemblyTitle("LiveSplit.Kalimba")]
[assembly: AssemblyDescription("Autosplitter for Kalimba")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("LiveSplit.Kalimba")]
[assembly: AssemblyCopyright("Copyright © 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("b3294e28-2bd4-4e39-92fa-e04a620c7e7f")]
[assembly: AssemblyVersion("2.6.2.0")]
[assembly: AssemblyFileVersion("2.6.2.0")]
#if LiveSplit
[assembly: ComponentFactory(typeof(KalimbaFactory))]
#endif