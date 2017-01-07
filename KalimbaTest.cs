namespace LiveSplit.Kalimba {
	public class KalimbaTest {
		private static KalimbaComponent comp = null;
		public static void Main(string[] args) {
			comp = new KalimbaComponent(true);
			System.Windows.Forms.Application.Run(comp.Manager);
		}
	}
}