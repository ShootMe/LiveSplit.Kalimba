using LiveSplit.Kalimba.Memory;
using System;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.Kalimba {
	public partial class KalimbaManager : Form {
		public KalimbaMemory Memory { get; set; }
		public KalimbaComponent Component { get; set; }
		public KalimbaManager() {
			InitializeComponent();
			Visible = false;
			Thread t = new Thread(UpdateLoop);
			t.IsBackground = true;
			t.Start();
		}

		private void KalimbaManager_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = Memory != null;
		}

		private void btnNewGame_Click(object sender, System.EventArgs e) {
			try {
				if (Memory.GetCurrentMenu() != MenuScreen.MainMenu) {
					MessageBox.Show("Please go to the Main Menu before setting Kalimba back to a New Game.", "Kalimba");
				} else {
					Memory.EraseData();
				}
			} catch { }
		}
		private void btnAllTotems_Click(object sender, System.EventArgs e) {
			try {
				if (Memory.GetCurrentMenu() != MenuScreen.MainMenu) {
					MessageBox.Show("Please go to the Main Menu before resetting all Totems.", "Kalimba");
				} else {
					Memory.SetLevelScore(PlatformLevelId.None, 40);
				}
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
			} else if (this.Visible && Memory != null && Memory.IsHooked) {
				MenuScreen menu = Memory.GetCurrentMenu();
				btnNextCheckpoint.Enabled = menu == MenuScreen.InGame && (Component == null || Component.Model == null || Component.Model.CurrentState.CurrentPhase != Model.TimerPhase.Running);
				btnPreviousCheckpoint.Enabled = menu == MenuScreen.InGame && (Component == null || Component.Model == null || Component.Model.CurrentState.CurrentPhase != Model.TimerPhase.Running);
				lblLevel.Text = "Level: " + Memory.GetLevelName();
				lblCurrentCheckpoint.Text = "Checkpoint: " + (Memory.GetCurrentCheckpoint() + 1) + " / " + Memory.GetCheckpointCount();
				lblP1Pos.Text = "T1: (" + Memory.GetLastXP1().ToString("0.00") + ", " + Memory.GetLastYP1().ToString("0.00") + ")";
				lblP2Pos.Text = "T2: (" + Memory.GetLastXP2().ToString("0.00") + ", " + Memory.GetLastYP2().ToString("0.00") + ")";
			} else if (Memory == null && this.Visible) {
				this.Hide();
			}
		}
	}
}