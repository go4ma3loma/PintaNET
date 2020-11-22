﻿using System;
using Pinta.Core;
using Mono.Unix;
using Gdk;

namespace Pinta.Actions
{
	class NewScreenshotAction : IActionHandler
	{
		#region IActionHandler Members
		public void Initialize ()
		{
			PintaCore.Actions.File.NewScreenshot.Activated += Activated;
		}

		public void Uninitialize ()
		{
			PintaCore.Actions.File.NewScreenshot.Activated -= Activated;
		}
		#endregion

		private void Activated (object sender, EventArgs e)
		{
			int delay = PintaCore.Settings.GetSetting<int> ("screenshot-delay", 0);

			SpinButtonEntryDialog dialog = new SpinButtonEntryDialog (Catalog.GetString ("Take Screenshot"),
					PintaCore.Chrome.MainWindow, Catalog.GetString ("Delay before taking a screenshot (seconds):"), 0, 300, delay);

			if (dialog.Run () == (int)Gtk.ResponseType.Ok) {
				delay = dialog.GetValue ();

				PintaCore.Settings.PutSetting ("screenshot-delay", delay);
				PintaCore.Settings.SaveSettings ();

				GLib.Timeout.Add ((uint)delay * 1000, () => {
					Screen screen = Screen.Default;
					Document doc = PintaCore.Workspace.NewDocument (new Size (screen.Width, screen.Height), new Cairo.Color (1, 1, 1));

					using (Pixbuf pb = Pixbuf.FromDrawable (screen.RootWindow, screen.RootWindow.Colormap, 0, 0, 0, 0, screen.Width, screen.Height)) {
						using (Cairo.Context g = new Cairo.Context (doc.UserLayers[0].Surface)) {
							CairoHelper.SetSourcePixbuf (g, pb, 0, 0);
							g.Paint ();
						}
					}

					doc.IsDirty = true;

					if (!PintaCore.Chrome.MainWindow.IsActive) {
						PintaCore.Chrome.MainWindow.UrgencyHint = true;

						// Don't flash forever
						GLib.Timeout.Add (3 * 1000, () => PintaCore.Chrome.MainWindow.UrgencyHint = false);
					}

					return false;
				});
			}

			dialog.Destroy ();
		}
	}
}
