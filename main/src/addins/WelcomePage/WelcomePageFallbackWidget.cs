// 
// WelcomePageFallbackWidget.cs
// 
// Author:
//   Scott Ellington
//   Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
// Copyright (c) 2005 Scott Ellington
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Xml;
using System.Reflection;

using Gtk;
using Gdk;
using Freedesktop.RecentFiles;

namespace MonoDevelop.WelcomePage
{
	public class WelcomePageFallbackView : WelcomePageView
	{
		WelcomePageFallbackWidget widget;
		ScrolledWindow scroller;
		
		public WelcomePageFallbackView () : base ()
		{
			scroller = new ScrolledWindow ();
			widget = new WelcomePageFallbackWidget (this);
			scroller.AddWithViewport (widget);
			scroller.Show ();
		}
		
		public override Widget Control {
			get { return scroller;  }
		}
		
		protected override void RecentChangesHandler (object sender, EventArgs e)
		{
			widget.LoadRecent ();
		}
	}
	
	public partial class WelcomePageFallbackWidget : Gtk.EventBox
	{
		Gdk.Pixbuf bgPixbuf;
		Gdk.Pixbuf logoPixbuf;
		Gdk.Pixbuf decorationPixbuf;
		
		WelcomePageView parentView;
		
		const string headerSize = "medium";
		const string textSize = "small";
		static readonly string headerFormat = "<span size=\"" + headerSize + "\"  weight=\"bold\" foreground=\"#2525a6\">{0}</span>";
		static readonly string tableHeaderFormat = "<span size=\"" + textSize + "\" weight=\"bold\">{0}</span>";
		static readonly string textFormat = "<span size=\"" + textSize + "\">{0}</span>";
		
		readonly int logoOffset = 20;
		
		public WelcomePageFallbackWidget (WelcomePageView parentView) : base ()
		{
			this.Build ();
			this.parentView = parentView;
			
			string logoPath = System.IO.Path.Combine (parentView.DataDirectory, "mono-logo.png");
			logoPixbuf = new Gdk.Pixbuf (logoPath);
			
			string bgPath = System.IO.Path.Combine (parentView.DataDirectory, "mono-bg.png");
			bgPixbuf = new Gdk.Pixbuf (bgPath);
			
			string decorationPath = System.IO.Path.Combine (parentView.DataDirectory, "mono-decoration.png");
			decorationPixbuf = new Gdk.Pixbuf (decorationPath);
			
			ModifyBg (StateType.Normal, Style.White);
			
			alignment1.SetPadding ((uint) (logoOffset + logoPixbuf.Height + logoOffset), 0, (uint) logoOffset, 0);
			BuildFromXml ();
			LoadRecent ();
		}
		
		void BuildFromXml ()
		{
			XmlDocument xml = new XmlDocument();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream ("WelcomePageContent.xml")) {
				xml.Load (new XmlTextReader (stream));
			}
			
			//Actions
			XmlNode actions = xml.SelectSingleNode ("/WelcomePage/Actions");
			headerActions.Markup = string.Format (headerFormat, actions.Attributes ["_title"].Value);
			foreach (XmlNode link in actions.ChildNodes) {
				XmlAttribute a; 
				LinkButton button = new LinkButton ();
				button.Clicked += HandleLink;
				a = link.Attributes ["_title"];
				if (a != null) button.Label = string.Format (textFormat, a.Value);
				a = link.Attributes ["href"];
				if (a != null) button.LinkUrl = a.Value;
				a = link.Attributes ["_desc"];
				if (a != null) button.HoverMessage = a.Value;
				actionBox.PackEnd (button, true, false, 0);
			}
			actionBox.ShowAll ();
			
			//Support Links
			XmlNode supportLinks = xml.SelectSingleNode ("/WelcomePage/Links[@_title=\"Support Links\"]");
			headerSupportLinks.Markup = string.Format (headerFormat, supportLinks.Attributes ["_title"].Value);
			foreach (XmlNode link in supportLinks.ChildNodes) {
				XmlAttribute a; 
				LinkButton button = new LinkButton ();
				button.Clicked += HandleLink;
				a = link.Attributes ["_title"];
				if (a != null) button.Label = string.Format (textFormat, a.Value);
				a = link.Attributes ["href"];
				if (a != null) button.LinkUrl = a.Value;
				a = link.Attributes ["_desc"];
				if (a != null) button.HoverMessage = a.Value;
				supportLinkBox.PackEnd (button, true, false, 0);
			}
			supportLinkBox.ShowAll ();
			
			//Development Links
			XmlNode devLinks = xml.SelectSingleNode ("/WelcomePage/Links[@_title=\"Development Links\"]");
			headerDevLinks.Markup = string.Format (headerFormat, devLinks.Attributes ["_title"].Value);
			foreach (XmlNode link in devLinks.ChildNodes) {
				XmlAttribute a; 
				LinkButton button = new LinkButton ();
				button.Clicked += HandleLink;
				a = link.Attributes ["_title"];
				if (a != null) button.Label = string.Format (textFormat, a.Value);
				a = link.Attributes ["href"];
				if (a != null) button.LinkUrl = a.Value;
				a = link.Attributes ["_desc"];
				if (a != null) button.HoverMessage = a.Value;
				devLinkBox.PackEnd (button, true, false, 0);
			}
			devLinkBox.ShowAll ();
			
			//Recently Changed
			XmlNode recChanged = xml.SelectSingleNode ("/WelcomePage/Projects");
			headerRecentProj.Markup = string.Format (headerFormat, recChanged.Attributes ["_title"].Value);
			this.projNameLabel.Markup = string.Format (tableHeaderFormat, recChanged.Attributes ["_col1"].Value);
			projTimeLabel.Markup = string.Format (tableHeaderFormat, recChanged.Attributes ["_col2"].Value);
			//_linkTitle="Open Project"
		}
		
		void HandleLink (object sender, EventArgs e)
		{
			LinkButton button = (LinkButton) sender;
			if (parentView != null)
				parentView.HandleLinkAction (button.LinkUrl); 
		}
		
		//draw the background
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			Pixbuf stretched = bgPixbuf.ScaleSimple (Allocation.Width,
			                                  bgPixbuf.Height,
			                                  InterpType.Nearest);
			
			GdkWindow.DrawPixbuf (Style.BackgroundGC (StateType.Normal), 
			                      stretched, 0, 0, 0, 0, 
			                      stretched.Width, stretched.Height, 
			                      RgbDither.Normal, 0, 0);
			
			GdkWindow.DrawPixbuf (Style.BackgroundGC (StateType.Normal), 
			                      decorationPixbuf, 0, 0, 0, 0, 
			                      decorationPixbuf.Width, decorationPixbuf.Height, 
			                      RgbDither.Normal, 0, 0);
			
			GdkWindow.DrawPixbuf (Style.BackgroundGC (StateType.Normal), 
			                      logoPixbuf, 0, 0, logoOffset, logoOffset, 
			                      logoPixbuf.Width, logoPixbuf.Height, 
			                      RgbDither.Normal, 0, 0);
			
			foreach (Widget widget in Children)
				PropagateExpose (widget, evnt);
			
			return true;
		}
		
		protected override void OnDestroyed ()
		{
			base.OnDestroyed ();
			parentView = null;
		}
		
		public void LoadRecent ()
		{
			Widget[] oldChildren = (Widget[]) recentFilesTable.Children.Clone ();
			foreach (Widget w in oldChildren)
				recentFilesTable.Remove (w);
			
			RecentItem[] items = parentView.RecentProjects;
			if (items == null || items.Length < 1)
				return;
			
			uint i = 0;
			foreach (RecentItem ri in items) {
				LinkButton button = new LinkButton ();
				Label label = new Label ();
				recentFilesTable.Attach (button, 0, 1, i, i+1);
				recentFilesTable.Attach (label, 1, 2, i, i+1);
				button.Clicked += HandleLink;
				label.Justify = Justification.Right;
				label.Xalign = 1;
				button.Xalign = 0;
				
				string name = (ri.Private != null && ri.Private.Length > 0) ?
					ri.Private :
					System.IO.Path.GetFileNameWithoutExtension (ri.LocalPath);
				button.Label = string.Format (textFormat, name);
				button.HoverMessage = ri.LocalPath;
				button.LinkUrl = "project://" + ri.LocalPath;
				label.Markup = string.Format (textFormat, WelcomePageView.TimeSinceEdited (ri.Timestamp));
				i++;
				
				button.InnerLabel.MaxWidthChars = 22;
				button.InnerLabel.Ellipsize = Pango.EllipsizeMode.End;
			}
			recentFilesTable.RowSpacing = 0;
			recentFilesTable.ShowAll ();
		}
	}
	
	
	
	public class LinkButton : Gtk.Button
	{
		string hoverMessage = null;
		Label label;
		static Tooltips tooltips;
		static int tipcount;
		
		static Tooltips Tooltips {
			get {
				if (tooltips == null)
					tooltips = new Tooltips ();
				return tooltips;
			}
		}
		
		public LinkButton () : base ()
		{
			label = new Label ();
			label.Xalign = 0;
			label.Xpad = 0;
			label.Ypad = 0;
			Add (label);
			Relief = ReliefStyle.None;
			tipcount ++;
		}
			
		public string HoverMessage {
			get { return hoverMessage; }
			set {
				hoverMessage = value;
				Tooltips.SetTip (this, hoverMessage, string.Empty);
			}
		}
		
		public new string Label {
			get { return label.Text; }
			set {
				label.Markup = string.Format ("<span underline=\"single\" foreground=\"#5a7ac7\">{0}</span>", value);
			}
		}
		
		protected override void OnClicked ()
		{
			base.OnClicked ();
			//FIXME
			//Gnome.Url.Show (linkUrl);
		}
		
		protected override bool OnDestroyEvent (Event evnt)
		{
			bool retval = base.OnDestroyEvent (evnt);
			tipcount--;
			if (tipcount < 1) {
				tooltips.Destroy ();
				tooltips = null;
			}
			return retval;
		}
		
		string linkUrl;
		
		public string LinkUrl {
			get { return linkUrl; }
			set { linkUrl = value; }
		}
		
		public Label InnerLabel {
			get { return label; }
		}
	}
}
