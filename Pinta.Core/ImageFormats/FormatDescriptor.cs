using System;
using System.Text;

using Gtk;
using Mono.Unix;

namespace Pinta.Core
{
	/// <summary>
	/// Describes information about a file format, such as the
	/// supported file extensions.
	/// </summary>
	public sealed class FormatDescriptor
	{
		/// <summary>
		/// A list of the supported extensions (for example, "jpeg" and "JPEG").
		/// </summary>
		public string[] Extensions { get; private set; }

		/// <summary>
		/// The importer for this file format. This may be null if only exporting
		/// is supported for this format.
		/// </summary>
		public IImageImporter Importer { get; private set; }

		/// <summary>
		/// The exporter for this file format. This may be null if only importing
		/// is supported for this format.
		/// </summary>
		public IImageExporter Exporter { get; private set; }

		/// <summary>
		/// A file filter for use in the file dialog.
		/// </summary>
		public FileFilter Filter { get; private set; }

		/// <param name="displayPrefix">
		/// A descriptive name for the format, such as "OpenRaster". This will be displayed
		/// in the file dialog's filter.
		/// </param>
		/// <param name="extensions">A list of supported file extensions (for example, "jpeg" and "JPEG").</param>
		/// <param name="importer">The importer for this file format, or null if importing is not supported.</param>
		/// <param name="exporter">The exporter for this file format, or null if exporting is not supported.</param>
		public FormatDescriptor (string displayPrefix, string[] extensions,
		                         IImageImporter importer, IImageExporter exporter)
		{
			if (extensions == null || (importer == null && exporter == null)) {
				throw new ArgumentNullException ("Format descriptor is initialized incorrectly");
			}
		
			this.Extensions = extensions;
			this.Importer = importer;
			this.Exporter = exporter;
			
			FileFilter ff = new FileFilter ();
			StringBuilder formatNames = new StringBuilder ();
			
			foreach (string ext in extensions) {
				if (formatNames.Length > 0)
					formatNames.Append (", ");
				
				string wildcard = string.Format ("*.{0}", ext);
				ff.AddPattern (wildcard);
				formatNames.Append (wildcard);
			}

			ff.Name = string.Format (Catalog.GetString ("{0} image ({1})"), displayPrefix, formatNames);
			this.Filter = ff;
		}
		
		public bool IsReadOnly ()
		{
			return Exporter == null;
		}

		public bool IsWriteOnly ()
		{
			return Importer == null;
		}
	}
}
