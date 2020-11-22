using System;
using Gtk;
using Mono.Unix;
using Pinta.Core;

namespace Pinta
{
	public class ResizeImageDialog : Dialog
	{
		private RadioButton percentageRadio;
		private RadioButton absoluteRadio;
		private SpinButton percentageSpinner;
		private SpinButton widthSpinner;
		private SpinButton heightSpinner;
		private CheckButton aspectCheckbox;

		private bool value_changing;
		
		public ResizeImageDialog () : base (Catalog.GetString ("Resize Image"), PintaCore.Chrome.MainWindow,
		                                    DialogFlags.Modal,
											Gtk.Stock.Cancel, Gtk.ResponseType.Cancel,
											Gtk.Stock.Ok, Gtk.ResponseType.Ok)
		{
			Build ();

			aspectCheckbox.Active = true;

			widthSpinner.Value = PintaCore.Workspace.ImageSize.Width;
			heightSpinner.Value = PintaCore.Workspace.ImageSize.Height;

			percentageRadio.Toggled += new EventHandler (percentageRadio_Toggled);
			absoluteRadio.Toggled += new EventHandler (absoluteRadio_Toggled);
			percentageRadio.Toggle ();

			percentageSpinner.Value = 100;
			percentageSpinner.ValueChanged += new EventHandler (percentageSpinner_ValueChanged);

			widthSpinner.ValueChanged += new EventHandler (widthSpinner_ValueChanged);
			heightSpinner.ValueChanged += new EventHandler (heightSpinner_ValueChanged);
			
			AlternativeButtonOrder = new int[] { (int) Gtk.ResponseType.Ok, (int) Gtk.ResponseType.Cancel };
			DefaultResponse = Gtk.ResponseType.Ok;

			widthSpinner.ActivatesDefault = true;
			heightSpinner.ActivatesDefault = true;
			percentageSpinner.ActivatesDefault = true;
			percentageSpinner.GrabFocus();
		}

		#region Public Methods
		public void SaveChanges ()
		{
			PintaCore.Workspace.ResizeImage (widthSpinner.ValueAsInt, heightSpinner.ValueAsInt);
		}
		#endregion
		
		#region Private Methods
		private void heightSpinner_ValueChanged (object sender, EventArgs e)
		{
			if (value_changing)
				return;
				
			if (aspectCheckbox.Active) {
				value_changing = true;
				widthSpinner.Value = (int)((heightSpinner.Value * PintaCore.Workspace.ImageSize.Width) / PintaCore.Workspace.ImageSize.Height);
				value_changing = false;
			}
		}

		private void widthSpinner_ValueChanged (object sender, EventArgs e)
		{
			if (value_changing)
				return;

			if (aspectCheckbox.Active) {
				value_changing = true;
				heightSpinner.Value = (int)((widthSpinner.Value * PintaCore.Workspace.ImageSize.Height) / PintaCore.Workspace.ImageSize.Width);
				value_changing = false;
			}
		}

		private void percentageSpinner_ValueChanged (object sender, EventArgs e)
		{
			widthSpinner.Value = (int)(PintaCore.Workspace.ImageSize.Width * (percentageSpinner.ValueAsInt / 100f));
			heightSpinner.Value = (int)(PintaCore.Workspace.ImageSize.Height * (percentageSpinner.ValueAsInt / 100f));
		}

		private void absoluteRadio_Toggled (object sender, EventArgs e)
		{
			RadioToggle ();
		}

		private void percentageRadio_Toggled (object sender, EventArgs e)
		{
			RadioToggle ();
		}

		private void RadioToggle ()
		{
			if (percentageRadio.Active) {
				percentageSpinner.Sensitive = true;

				widthSpinner.Sensitive = false;
				heightSpinner.Sensitive = false;
				aspectCheckbox.Sensitive = false;
			} else {
				percentageSpinner.Sensitive = false;

				widthSpinner.Sensitive = true;
				heightSpinner.Sensitive = true;
				aspectCheckbox.Sensitive = true;
			}
		}

		private void Build()
		{
			Icon = PintaCore.Resources.GetIcon ("Menu.Image.Resize.png");
			WindowPosition = WindowPosition.CenterOnParent;

			DefaultWidth = 300;
			DefaultHeight = 200;

			percentageRadio = new RadioButton (Catalog.GetString ("By percentage:"));
			absoluteRadio = new RadioButton (percentageRadio, Catalog.GetString ("By absolute size:"));

			percentageSpinner = new SpinButton (1, 1000, 1);
			widthSpinner = new SpinButton (1, 10000, 1);
			heightSpinner = new SpinButton (1, 10000, 1);

			aspectCheckbox = new CheckButton (Catalog.GetString ("Maintain aspect ratio"));

			const int spacing = 6;
			var main_vbox = new VBox () { Spacing = spacing, BorderWidth = 12 };

			var hbox_percent = new HBox () { Spacing = spacing };
			hbox_percent.PackStart (percentageRadio, true, true, 0);
			hbox_percent.PackStart (percentageSpinner, false, false, 0);
			hbox_percent.PackEnd (new Label ("%"), false, false, 0);
			main_vbox.PackStart (hbox_percent, false, false, 0);

			main_vbox.PackStart (absoluteRadio, false, false, 0);

			var hbox_width = new HBox () { Spacing = spacing };
			hbox_width.PackStart (new Label (Catalog.GetString ("Width:")), false, false, 0);
			hbox_width.PackStart (widthSpinner, false, false, 0);
			hbox_width.PackStart (new Label (Catalog.GetString ("pixels")), false, false, 0);
			main_vbox.PackStart (hbox_width, false, false, 0);

			var hbox_height = new HBox () { Spacing = spacing };
			hbox_height.PackStart (new Label (Catalog.GetString ("Height:")), false, false, 0);
			hbox_height.PackStart (heightSpinner, false, false, 0);
			hbox_height.PackStart (new Label (Catalog.GetString ("pixels")), false, false, 0);
			main_vbox.PackStart (hbox_height, false, false, 0);

			main_vbox.PackStart (aspectCheckbox, false, false, 0);

			VBox.BorderWidth = 2;
			VBox.Add (main_vbox);

			ShowAll ();
		}
		#endregion
	}
}

