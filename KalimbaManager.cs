using LiveSplit.Kalimba.Memory;
using System;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.Kalimba {
	public partial class KalimbaManager : Form {
		public KalimbaMemory Memory { get; set; }
		public KalimbaManager() {
			InitializeComponent();
			Visible = false;
			Thread t = new Thread(UpdateLoop);
			t.IsBackground = true;
			t.Start();
		}

		private void KalimbaManager_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = true;
		}

		private void btnNewGame_Click(object sender, System.EventArgs e) {
			try {
				Memory.EraseData();
			} catch { }
		}
		private void btnAllTotems_Click(object sender, System.EventArgs e) {
			try {
				Memory.SetScore(PlatformLevelId.None, 40);
			} catch { }
		}
		private void btnPreviousCheckpoint_Click(object sender, System.EventArgs e) {
			try {
				int cp = Memory.GetCurrentCheckpoint();
				Memory.SetCheckpoint(cp - 1);
			} catch { }
		}
		private void btnNextCheckpoint_Click(object sender, System.EventArgs e) {
			try {
				int cp = Memory.GetCurrentCheckpoint();
				Memory.SetCheckpoint(cp + 1);
			} catch { }
		}

		private void UpdateLoop() {
			try {
				while (true) {
					UpdateValues();
					Thread.Sleep(33);
				}
			} catch { }
		}
		public void UpdateValues() {
			if (this.InvokeRequired) {
				this.Invoke((Action)UpdateValues);
			} else if (this.Visible && Memory.IsHooked) {
				lblLevel.Text = "Level: " + Memory.GetLevelName();
				lblCurrentCheckpoint.Text = "Checkpoint: " + (Memory.GetCurrentCheckpoint() + 1) + " / " + Memory.GetCheckpointCount();
				lblP1Pos.Text = "P1: (" + Memory.GetLastXP1().ToString("0.00") + ", " + Memory.GetLastYP1().ToString("0.00") + ")";
				lblP2Pos.Text = "P2: (" + Memory.GetLastXP2().ToString("0.00") + ", " + Memory.GetLastYP2().ToString("0.00") + ")";
			}
		}
	}
}