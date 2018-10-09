namespace LiveSplit.Kalimba {
	public class KalimbaTest {
		private static KalimbaComponent comp = null;
		public static void Main(string[] args) {
			try {
				comp = new KalimbaComponent(null, true);
				System.Windows.Forms.Application.Run(comp.Manager);
			} catch { }
		}
	}
}