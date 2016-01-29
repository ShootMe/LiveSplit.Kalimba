namespace LiveSplit.Kalimba {
	public class KalimbaTest {
		
		public static void Main(string[] args) {
			KalimbaComponent comp = new KalimbaComponent();

			while (true) {
				comp.GetValues();

				System.Threading.Thread.Sleep(5);
			}
		}
	}
}