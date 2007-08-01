// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Ide.Gui.Dialogs {
    
    
    internal partial class AddinLoadErrorDialog {
        
        private Gtk.HBox hbox1;
        
        private Gtk.Image image1;
        
        private Gtk.VBox vbox4;
        
        private Gtk.Label label4;
        
        private Gtk.ScrolledWindow scrolledwindow1;
        
        private Gtk.TreeView errorTree;
        
        private Gtk.Label labelContinue;
        
        private Gtk.Label labelFatal;
        
        private Gtk.Label labelWarning;
        
        private Gtk.Button noButton;
        
        private Gtk.Button yesButton;
        
        private Gtk.Button closeButton;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget MonoDevelop.Ide.Gui.Dialogs.AddinLoadErrorDialog
            this.Name = "MonoDevelop.Ide.Gui.Dialogs.AddinLoadErrorDialog";
            this.Title = "MonoDevelop";
            this.TypeHint = ((Gdk.WindowTypeHint)(1));
            this.BorderWidth = ((uint)(6));
            this.DefaultHeight = 350;
            // Internal child MonoDevelop.Ide.Gui.Dialogs.AddinLoadErrorDialog.VBox
            Gtk.VBox w1 = this.VBox;
            w1.Name = "dialog-vbox1";
            w1.Spacing = 6;
            w1.BorderWidth = ((uint)(2));
            // Container child dialog-vbox1.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 12;
            this.hbox1.BorderWidth = ((uint)(6));
            // Container child hbox1.Gtk.Box+BoxChild
            this.image1 = new Gtk.Image();
            this.image1.Name = "image1";
            this.image1.Xalign = 0F;
            this.image1.Yalign = 0F;
            this.image1.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-dialog-error", Gtk.IconSize.Dialog, 48);
            this.hbox1.Add(this.image1);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox1[this.image1]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.vbox4 = new Gtk.VBox();
            this.vbox4.Name = "vbox4";
            this.vbox4.Spacing = 6;
            // Container child vbox4.Gtk.Box+BoxChild
            this.label4 = new Gtk.Label();
            this.label4.Name = "label4";
            this.label4.Xalign = 0F;
            this.label4.Yalign = 0F;
            this.label4.LabelProp = Mono.Unix.Catalog.GetString("The following add-ins could not be started:");
            this.vbox4.Add(this.label4);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.vbox4[this.label4]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.scrolledwindow1 = new Gtk.ScrolledWindow();
            this.scrolledwindow1.Name = "scrolledwindow1";
            this.scrolledwindow1.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow1.Gtk.Container+ContainerChild
            this.errorTree = new Gtk.TreeView();
            this.errorTree.Name = "errorTree";
            this.errorTree.HeadersVisible = false;
            this.errorTree.HeadersClickable = true;
            this.scrolledwindow1.Add(this.errorTree);
            this.vbox4.Add(this.scrolledwindow1);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox4[this.scrolledwindow1]));
            w5.Position = 1;
            // Container child vbox4.Gtk.Box+BoxChild
            this.labelContinue = new Gtk.Label();
            this.labelContinue.WidthRequest = 479;
            this.labelContinue.Name = "labelContinue";
            this.labelContinue.Xalign = 0F;
            this.labelContinue.Yalign = 0F;
            this.labelContinue.LabelProp = Mono.Unix.Catalog.GetString("You can start MonoDevelop without these add-ins, but the functionality they provide will be missing. Do you wish to continue?");
            this.labelContinue.Wrap = true;
            this.vbox4.Add(this.labelContinue);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.vbox4[this.labelContinue]));
            w6.Position = 2;
            w6.Expand = false;
            w6.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.labelFatal = new Gtk.Label();
            this.labelFatal.Name = "labelFatal";
            this.labelFatal.Xalign = 0F;
            this.labelFatal.Yalign = 0F;
            this.labelFatal.LabelProp = Mono.Unix.Catalog.GetString("MonoDevelop cannot start because a fatal error has been detected.");
            this.vbox4.Add(this.labelFatal);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.vbox4[this.labelFatal]));
            w7.Position = 3;
            w7.Expand = false;
            w7.Fill = false;
            // Container child vbox4.Gtk.Box+BoxChild
            this.labelWarning = new Gtk.Label();
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Xalign = 0F;
            this.labelWarning.Yalign = 0F;
            this.labelWarning.LabelProp = Mono.Unix.Catalog.GetString("MonoDevelop can run without these add-ins, but the functionality they provide will be missing.");
            this.labelWarning.Wrap = true;
            this.vbox4.Add(this.labelWarning);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.vbox4[this.labelWarning]));
            w8.Position = 4;
            w8.Expand = false;
            w8.Fill = false;
            this.hbox1.Add(this.vbox4);
            Gtk.Box.BoxChild w9 = ((Gtk.Box.BoxChild)(this.hbox1[this.vbox4]));
            w9.Position = 1;
            w1.Add(this.hbox1);
            Gtk.Box.BoxChild w10 = ((Gtk.Box.BoxChild)(w1[this.hbox1]));
            w10.Position = 0;
            // Internal child MonoDevelop.Ide.Gui.Dialogs.AddinLoadErrorDialog.ActionArea
            Gtk.HButtonBox w11 = this.ActionArea;
            w11.Name = "GtkDialog_ActionArea";
            w11.Spacing = 10;
            w11.BorderWidth = ((uint)(5));
            w11.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child GtkDialog_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.noButton = new Gtk.Button();
            this.noButton.CanFocus = true;
            this.noButton.Name = "noButton";
            this.noButton.UseStock = true;
            this.noButton.UseUnderline = true;
            this.noButton.Label = "gtk-no";
            this.AddActionWidget(this.noButton, -9);
            Gtk.ButtonBox.ButtonBoxChild w12 = ((Gtk.ButtonBox.ButtonBoxChild)(w11[this.noButton]));
            w12.Expand = false;
            w12.Fill = false;
            // Container child GtkDialog_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.yesButton = new Gtk.Button();
            this.yesButton.CanFocus = true;
            this.yesButton.Name = "yesButton";
            this.yesButton.UseStock = true;
            this.yesButton.UseUnderline = true;
            this.yesButton.Label = "gtk-yes";
            this.AddActionWidget(this.yesButton, -8);
            Gtk.ButtonBox.ButtonBoxChild w13 = ((Gtk.ButtonBox.ButtonBoxChild)(w11[this.yesButton]));
            w13.Position = 1;
            w13.Expand = false;
            w13.Fill = false;
            // Container child GtkDialog_ActionArea.Gtk.ButtonBox+ButtonBoxChild
            this.closeButton = new Gtk.Button();
            this.closeButton.CanFocus = true;
            this.closeButton.Name = "closeButton";
            this.closeButton.UseStock = true;
            this.closeButton.UseUnderline = true;
            this.closeButton.Label = "gtk-close";
            this.AddActionWidget(this.closeButton, -7);
            Gtk.ButtonBox.ButtonBoxChild w14 = ((Gtk.ButtonBox.ButtonBoxChild)(w11[this.closeButton]));
            w14.Position = 2;
            w14.Expand = false;
            w14.Fill = false;
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 573;
            this.labelFatal.Hide();
            this.labelWarning.Hide();
            this.closeButton.Hide();
            this.Show();
        }
    }
}
