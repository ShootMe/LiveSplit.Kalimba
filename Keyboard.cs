using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace LiveSplit.Kalimba {
	public class KeyboardHook {
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern short GetAsyncKeyState(Keys vkey);
		private Dictionary<Keys, bool> RegisteredKeys { get; set; }
		public event KeyEventHandler KeyPressed;

		public KeyboardHook() {
			RegisteredKeys = new Dictionary<Keys, bool>();
		}
		public void RegisterHotKey(Keys key) {
			if (!RegisteredKeys.ContainsKey(key)) {
				RegisteredKeys.Add(key, false);
			}
		}
		public void UnregisterAllHotkeys() {
			RegisteredKeys.Clear();
		}
		public void Poll() {
			List<Keys> changed = new List<Keys>();
			foreach (KeyValuePair<Keys, bool> current in RegisteredKeys) {
				Keys key = current.Key;
				bool modifiers = true;
				Keys keys = Keys.None;
				if (IsKeyDown(Keys.ShiftKey)) {
					modifiers &= key.HasFlag(Keys.Shift);
					keys |= Keys.Shift;
				}
				if (IsKeyDown(Keys.ControlKey)) {
					modifiers &= key.HasFlag(Keys.Control);
					keys |= Keys.Control;
				}
				if (IsKeyDown(Keys.Menu)) {
					modifiers &= key.HasFlag(Keys.Alt);
					keys |= Keys.Alt;
				}
				bool hasKeyDown = IsKeyDown(key & ~keys);
				if (hasKeyDown != current.Value) {
					changed.Add(key);
				}
				if ((modifiers & hasKeyDown) && !current.Value) {
					KeyPressed?.Invoke(this, new KeyEventArgs(key));
				}
			}

			for (int i = 0; i < changed.Count; i++) {
				Keys key = changed[i];
				bool current = RegisteredKeys[key];
				RegisteredKeys[key] = !current;
			}
		}
		protected bool IsKeyDown(Keys key) {
			return (KeyboardHook.GetAsyncKeyState(key) >> 15 & 1) == 1;
		}
	}
}