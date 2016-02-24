using System.Threading;
namespace LiveSplit.Kalimba {
	public class KalimbaTest {
		private static KalimbaComponent comp = new KalimbaComponent();
		public static void Main(string[] args) {
			Thread t = new Thread(GetVals);
			t.IsBackground = true;
			t.Start();
			System.Windows.Forms.Application.Run();
		}
		private static void GetVals() {
			while (true) {
				comp.GetValues();

				Thread.Sleep(5);
			}
		}
	}
}