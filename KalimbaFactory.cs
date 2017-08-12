#if LiveSplit
using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Reflection;
namespace LiveSplit.Kalimba {
    public class KalimbaFactory : IComponentFactory {
        public string ComponentName { get { return "Kalimba Autosplitter v" + this.Version.ToString(); } }
        public string Description { get { return "Autosplitter for Kalimba"; } }
        public ComponentCategory Category { get { return ComponentCategory.Control; } }
        public IComponent Create(LiveSplitState state) { return new KalimbaComponent(state); }
        public string UpdateName { get { return this.ComponentName; } }
		public string UpdateURL { get { return "https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/"; } }
		public string XMLURL { get { return this.UpdateURL + "Components/LiveSplit.Kalimba.Updates.xml"; } }
		public Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
    }
}
#endif