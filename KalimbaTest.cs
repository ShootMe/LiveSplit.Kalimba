namespace LiveSplit.Kalimba {
	public class KalimbaTest {
		private static KalimbaComponent comp = null;
		public static void Main(string[] args) {
			try {
#if LiveSplit
				comp = new KalimbaComponent(null, true);
#else
				comp = new KalimbaComponent(true);
#endif
				System.Windows.Forms.Application.Run(comp.Manager);
			} catch { }
		}
	}
}