// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using Gtk;
using Gecko;

using MonoDevelop.Ide.Gui.Undo;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Core.Properties;
using MonoDevelop.Core;
using MonoDevelop.Core.Gui;
using MonoDevelop.Core.Gui.Components;
using MonoDevelop.Components;
using MonoDevelop.Components.HtmlControl;

namespace MonoDevelop.Ide.Gui.BrowserDisplayBinding
{
	public class BrowserPane : AbstractViewContent, ISecondaryViewContent
	{	
		protected HtmlViewPane htmlViewPane;
		protected IViewContent parent;

		public void Selected ()
		{	
		}

		public void Deselected ()
		{
		}

		public void NotifyBeforeSave ()
		{
		}

		public override string TabPageLabel
		{
			get {
				return GettextCatalog.GetString ("Web Browser");
			}
		}
		
		public void BaseContentChanged ()
		{
			ITextBuffer buffer = (ITextBuffer) parent.GetContent (typeof(ITextBuffer));
			
			try {
				htmlViewPane.MozillaControl.OpenStream ("file://", "text/html");
				htmlViewPane.MozillaControl.AppendData (buffer.Text);
				htmlViewPane.MozillaControl.CloseStream ();
				GLib.Timeout.Add (50, new GLib.TimeoutHandler (checkFocus));
			} catch {
				Runtime.LoggingService.Error ("Gecko# tossed an exception");
			}
		}

		public bool checkFocus ()
		{			
			//not sure why this was here, may be to fix a GTK+ 2.4 bug
			Gtk.ScrolledWindow sw = (ScrolledWindow) ((VBox) parent.Control).Children [1];
			if (sw.Children[0].HasFocus == false) {
				sw.Children[0].GrabFocus ();
				return false;
			}
			return true;
		}

		public override Widget Control {
			get {
				return htmlViewPane;
			}
		}
		
		public override bool IsDirty {
			get {
				return false;
			}
			set {
			}
		}
		
		public override bool IsViewOnly {
			get {
				return true;
			}
		}
		
		public BrowserPane (bool showNavigation, IViewContent parent) : this (showNavigation)
		{
			this.parent = parent;
			
			//suppress in-window hyperlinking, but only when this used as is an 
			//ISecondaryViewContent, i.e, it has a parent
			htmlViewPane.MozillaControl.OpenUri += CatchUri;
		}

		public BrowserPane (bool showNavigation)
		{
			htmlViewPane = new HtmlViewPane(showNavigation);
			htmlViewPane.MozillaControl.TitleChange += new EventHandler (OnTitleChanged);
		}
		
		void CatchUri (object sender, OpenUriArgs e)
		{
			e.RetVal = true;
			Gnome.Url.Show (e.AURI);
		}
		
		public BrowserPane () : this (true)
		{
		}
		
		public override void Dispose()
		{
			htmlViewPane.Dispose();
		}
		
		public override void Load(string url)
		{
			htmlViewPane.Navigate(url);
		}
		
		public override void Save(string url)
		{
			Load(url);
		}
		
		private void OnTitleChanged (object o, EventArgs args)
		{
			ContentName = htmlViewPane.MozillaControl.Title; 
		}
	}
	
	public class HtmlViewPane : Gtk.Frame
	{
		MozillaControl htmlControl = null;
		SdStatusBar status;
		
		VBox topPanel = new VBox (false, 2);
		Navbar nav = new Navbar ();
		
		bool loading = false;
		
		public MozillaControl MozillaControl {
			get {
				return htmlControl;
			}
		}
		
		public HtmlViewPane (bool showNavigation)
		{
			Shadow = Gtk.ShadowType.In;
			VBox mainbox = new VBox (false, 2);
			status = (SdStatusBar) Services.StatusBar.Control;
			
			if (showNavigation) {
				
				nav.Back += new EventHandler (OnBackClicked);
				nav.Forward += new EventHandler (OnForwardClicked);
				nav.Stop += new EventHandler (OnStopClicked);
				nav.Reload += new EventHandler (OnRefreshClicked);
				nav.Go += new EventHandler (OnEntryActivated);
				
				topPanel.PackStart (nav);
				mainbox.PackStart (topPanel, false, false, 2);
			} 
			
			htmlControl = new MozillaControl ();
			htmlControl.NetStart += new EventHandler (OnNetStart);
			htmlControl.NetStop += new EventHandler (OnNetStop);
			htmlControl.LocChange += new EventHandler (OnLocationChanged);
			htmlControl.ShowAll ();
			
			mainbox.PackStart (htmlControl);
			this.Add (mainbox);
			this.ShowAll ();
		}
		
		void OnEntryActivated (object o, EventArgs args)
		{
			htmlControl.LoadUrl (nav.Url);
		}
		
		public void CreatedWebBrowserHandle(object sender, EventArgs evArgs) 
		{
		}
		
		public void Navigate(string name)
		{
			nav.Url = name;
			htmlControl.LoadUrl (name);
		}

		private void OnNetStart (object o, EventArgs args)
		{
			Services.StatusBar.SetMessage (GettextCatalog.GetString ("Loading..."));
			loading = true;
			GLib.Idle.Add (new GLib.IdleHandler (Pulse));
		}

		bool Pulse ()
		{
			if (loading) {
				status.Pulse ();
				System.Threading.Thread.Sleep (100);
				return true;
			}
			status.EndProgress ();
			Services.StatusBar.SetMessage (GettextCatalog.GetString ("Done."));
			return false;
		}

		private void OnNetStop (object o, EventArgs args)
		{
			loading = false;
		}

		void OnLocationChanged (object o, EventArgs args)
		{
			nav.Url = htmlControl.Location;
		}

		private void OnBackClicked (object o, EventArgs args)
		{
			htmlControl.GoBack ();
		}
		
		private void OnForwardClicked (object o, EventArgs args)
		{
			htmlControl.GoForward ();
		}
		
		private void OnStopClicked (object o, EventArgs args)
		{
			htmlControl.Stop ();
		}
		
		private void OnRefreshClicked (object o, EventArgs args)
		{
			htmlControl.Refresh ();
		}
	}
}
