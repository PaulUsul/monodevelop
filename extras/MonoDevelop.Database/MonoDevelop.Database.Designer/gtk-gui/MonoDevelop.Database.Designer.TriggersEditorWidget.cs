
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Database.Designer
{
	public partial class TriggersEditorWidget
	{
		private global::Gtk.VPaned vpaned;

		private global::Gtk.HBox hbox;

		private global::Gtk.ScrolledWindow windowTriggers;

		private global::Gtk.TreeView listTriggers;

		private global::Gtk.VButtonBox vbuttonbox;

		private global::Gtk.Button buttonAdd;

		private global::Gtk.Button buttonRemove;

		private global::Gtk.Frame frame;

		private global::Gtk.Alignment alignment;

		private global::MonoDevelop.Database.Components.SqlEditorWidget sqlEditor;

		private global::Gtk.Label GtkLabel2;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Database.Designer.TriggersEditorWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.Database.Designer.TriggersEditorWidget";
			// Container child MonoDevelop.Database.Designer.TriggersEditorWidget.Gtk.Container+ContainerChild
			this.vpaned = new global::Gtk.VPaned ();
			this.vpaned.CanFocus = true;
			this.vpaned.Name = "vpaned";
			this.vpaned.Position = 205;
			// Container child vpaned.Gtk.Paned+PanedChild
			this.hbox = new global::Gtk.HBox ();
			this.hbox.Name = "hbox";
			this.hbox.Spacing = 6;
			// Container child hbox.Gtk.Box+BoxChild
			this.windowTriggers = new global::Gtk.ScrolledWindow ();
			this.windowTriggers.CanFocus = true;
			this.windowTriggers.Name = "windowTriggers";
			this.windowTriggers.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child windowTriggers.Gtk.Container+ContainerChild
			this.listTriggers = new global::Gtk.TreeView ();
			this.listTriggers.CanFocus = true;
			this.listTriggers.Name = "listTriggers";
			this.windowTriggers.Add (this.listTriggers);
			this.hbox.Add (this.windowTriggers);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox[this.windowTriggers]));
			w2.Position = 0;
			// Container child hbox.Gtk.Box+BoxChild
			this.vbuttonbox = new global::Gtk.VButtonBox ();
			this.vbuttonbox.Name = "vbuttonbox";
			this.vbuttonbox.Spacing = 6;
			this.vbuttonbox.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(3));
			// Container child vbuttonbox.Gtk.ButtonBox+ButtonBoxChild
			this.buttonAdd = new global::Gtk.Button ();
			this.buttonAdd.CanFocus = true;
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.UseStock = true;
			this.buttonAdd.UseUnderline = true;
			this.buttonAdd.Label = "gtk-add";
			this.vbuttonbox.Add (this.buttonAdd);
			global::Gtk.ButtonBox.ButtonBoxChild w3 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox[this.buttonAdd]));
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbuttonbox.Gtk.ButtonBox+ButtonBoxChild
			this.buttonRemove = new global::Gtk.Button ();
			this.buttonRemove.Sensitive = false;
			this.buttonRemove.CanFocus = true;
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.UseStock = true;
			this.buttonRemove.UseUnderline = true;
			this.buttonRemove.Label = "gtk-remove";
			this.vbuttonbox.Add (this.buttonRemove);
			global::Gtk.ButtonBox.ButtonBoxChild w4 = ((global::Gtk.ButtonBox.ButtonBoxChild)(this.vbuttonbox[this.buttonRemove]));
			w4.Position = 1;
			w4.Expand = false;
			w4.Fill = false;
			this.hbox.Add (this.vbuttonbox);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox[this.vbuttonbox]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.vpaned.Add (this.hbox);
			global::Gtk.Paned.PanedChild w6 = ((global::Gtk.Paned.PanedChild)(this.vpaned[this.hbox]));
			w6.Resize = false;
			// Container child vpaned.Gtk.Paned+PanedChild
			this.frame = new global::Gtk.Frame ();
			this.frame.Name = "frame";
			this.frame.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame.Gtk.Container+ContainerChild
			this.alignment = new global::Gtk.Alignment (0f, 0f, 1f, 1f);
			this.alignment.Name = "alignment";
			this.alignment.LeftPadding = ((uint)(12));
			// Container child alignment.Gtk.Container+ContainerChild
			this.sqlEditor = null;
			this.alignment.Add (this.sqlEditor);
			this.frame.Add (this.alignment);
			this.GtkLabel2 = new global::Gtk.Label ();
			this.GtkLabel2.Name = "GtkLabel2";
			this.GtkLabel2.LabelProp = global::MonoDevelop.Database.AddinCatalog.GetString ("Statement");
			this.GtkLabel2.UseMarkup = true;
			this.frame.LabelWidget = this.GtkLabel2;
			this.vpaned.Add (this.frame);
			this.Add (this.vpaned);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.Show ();
			this.buttonAdd.Clicked += new global::System.EventHandler (this.AddClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.RemoveClicked);
		}
	}
}
