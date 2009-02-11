//  Project.cs
//
//  This file was derived from a file from #Develop. 
//
//  Copyright (C) 2001-2007 Mike Krüger <mkrueger@novell.com>
// 
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml;
using System.CodeDom.Compiler;

using MonoDevelop.Core;
using MonoDevelop.Core.Execution;
using Mono.Addins;
using MonoDevelop.Projects.Dom.Output;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects.Extensions;

namespace MonoDevelop.Projects
{
	public enum NewFileSearch {
		None,
		OnLoad,
		OnLoadAutoInsert
	}
	
	/// <summary>
	/// External language bindings must extend this class
	/// </summary>
	[DataInclude (typeof(ProjectFile))]
	[DataItem (FallbackType=typeof(UnknownProject))]
	public abstract class Project : SolutionEntityItem
	{
		[ItemProperty ("Description", DefaultValue="")]
		protected string description     = "";

		[ItemProperty ("newfilesearch", DefaultValue = NewFileSearch.None)]
		protected NewFileSearch newFileSearch  = NewFileSearch.None;
		
		[ItemProperty ("AppDesignerFolder")]
		string designerFolder;

		string [] buildActions = null;
		ProjectFileCollection projectFiles;
		
		bool isDirty = false;
		
		public Project ()
		{
			FileService.FileChanged += OnFileChanged;
			projectFiles = new ProjectFileCollection ();
			Items.Bind (projectFiles);
			DependencyResolutionEnabled = true;
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				NotifyModified ("Description");
			}
		}
		
		public ProjectFileCollection Files {
			get {
				return projectFiles;
			}
		}
		
		public NewFileSearch NewFileSearch {
			get {
				return newFileSearch;
			}

			set {
				newFileSearch = value;
				NotifyModified ("NewFileSearch");
			}
		}
		
		public abstract string ProjectType {
			get;
		}
		
		public virtual Ambience Ambience {
			get { return new NetAmbience (); }
		}
		
		[Browsable(false)]
		public virtual string [] SupportedLanguages {
			get {
				return new String [] { "" };
			}
		}

		public string DesignerFolder {
			get {
				return designerFolder;
			}
			set {
				designerFolder = value;
			}
		}
		
		//NOTE: groups the common actions at the top, separated by a "--" entry *IF* there are 
		// more "uncommon" actions than "common" actions
		public string[] GetBuildActions ()
		{
			if (buildActions != null)
				return buildActions;
			
			// find all the actions in use and add them to the list of standard actions
			Hashtable actions = new Hashtable ();
			object marker = new object (); //avoid using bools as they need to be boxed. re-use single object instead
			
			//ad the standard actions
			foreach (string action in GetStandardBuildActions ())
				actions [action] = marker;
			
			//add any more actions that are in the project file
			foreach (ProjectFile pf in projectFiles)
				if (!actions.ContainsKey (pf.BuildAction))
					actions [pf.BuildAction] = marker;
			
			//remove the "common" actions, since they're handled separately
			IList<string> commonActions = GetCommonBuildActions ();
			foreach (string action in commonActions)
				if (actions.Contains (action))
					actions.Remove (action);
			
			//calculate dimensions for our new array and create it
			int dashPos = commonActions.Count;
			bool hasDash = commonActions.Count > 0 && actions.Count > 0;
			int arrayLen = commonActions.Count + actions.Count;
			int uncommonStart = hasDash? dashPos + 1 : dashPos;
			if (hasDash)
				arrayLen++;
			buildActions = new string [arrayLen];
			
			//populate it
			if (commonActions.Count > 0)
				commonActions.CopyTo (buildActions, 0);
			if (hasDash)
				buildActions [dashPos] = "--";
			if (actions.Count > 0)
				actions.Keys.CopyTo (buildActions, uncommonStart);
			
			//sort the actions
			if (hasDash) {
				//it may be better to leave common actions in the order that the project specified
				//Array.Sort (buildActions, 0, commonActions.Count, StringComparer.Ordinal);
				Array.Sort (buildActions, uncommonStart, arrayLen - uncommonStart, StringComparer.Ordinal);
			} else {
				Array.Sort (buildActions, StringComparer.Ordinal);
			}
			
			return buildActions;
		}
		
		protected virtual IEnumerable<string> GetStandardBuildActions ()
		{
			return BuildAction.StandardActions;
		}
		
		protected virtual IList<string> GetCommonBuildActions ()
		{
			return BuildAction.StandardActions;
		}

		public bool IsFileInProject(string filename)
		{
			return GetProjectFile (filename) != null;
		}
		
		public ProjectFile GetProjectFile (string fileName)
		{
			return Files.GetFile (fileName);
		}
		
		public virtual bool IsCompileable (string fileName)
		{
			return false;
		}
		
		public virtual string GetDefaultBuildAction (string fileName)
		{
			if (IsCompileable (fileName))
				return BuildAction.Compile;
			else
				return BuildAction.None;
		}
				
		public static Project LoadProject (string filename, IProgressMonitor monitor)
		{
			Project prj = Services.ProjectService.ReadSolutionItem (monitor, filename) as Project;
			if (prj == null)
				throw new InvalidOperationException ("Invalid project file: " + filename);
			
			return prj;
		}

		
		public override void Dispose()
		{
			FileService.FileChanged -= OnFileChanged;
			foreach (ProjectFile file in Files) {
				file.Dispose ();
			}
			base.Dispose ();
		}
		
		public ProjectFile AddFile (string filename)
		{
			return AddFile (filename, null);
		}
		
		public ProjectFile AddFile (string filename, string buildAction)
		{
			foreach (ProjectFile fInfo in Files) {
				if (fInfo.Name == filename) {
					return fInfo;
				}
			}
			
			if (String.IsNullOrEmpty (buildAction)) {
				buildAction = GetDefaultBuildAction (filename);
			}
			
			ProjectFile newFileInformation = new ProjectFile (filename, buildAction);
			Files.Add (newFileInformation);
			return newFileInformation;
		}
		
		public void AddFile (ProjectFile projectFile) {
			Files.Add (projectFile);
		}
		
		public ProjectFile AddDirectory (string relativePath)
		{
			string newPath = Path.Combine (BaseDirectory, relativePath);
			
			foreach (ProjectFile fInfo in Files)
				if (fInfo.Name == newPath && fInfo.Subtype == Subtype.Directory)
					return fInfo;
			
			if (!Directory.Exists (newPath)) {
				if (File.Exists (newPath)) {
					string message = GettextCatalog.GetString ("Cannot create directory {0}, as a file with that name exists.", newPath);
					throw new InvalidOperationException (message);
				}
				FileService.CreateDirectory (newPath);
			}
			
			ProjectFile newDir = new ProjectFile (newPath);
			newDir.Subtype = Subtype.Directory;
			AddFile (newDir);
			return newDir;
		}
		
		protected internal override BuildResult OnBuild (IProgressMonitor monitor, string solutionConfiguration)
		{
			// create output directory, if not exists
			ProjectConfiguration conf = GetActiveConfiguration (solutionConfiguration) as ProjectConfiguration;
			if (conf == null) {
				BuildResult cres = new BuildResult ();
				cres.AddError (GettextCatalog.GetString ("Configuration '{0}' not found in project '{1}'", solutionConfiguration, Name));
				return cres;
			}
			string outputDir = conf.OutputDirectory;
			try {
				DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}
			} catch (Exception e) {
				throw new ApplicationException("Can't create project output directory " + outputDir + " original exception:\n" + e.ToString());
			}
			
			//copy references and files marked to "CopyToOutputDirectory"
			CopySupportFiles (monitor, solutionConfiguration);
		
			StringParserService.Properties["Project"] = Name;
			
			monitor.Log.WriteLine (GettextCatalog.GetString ("Performing main compilation..."));
			BuildResult res = DoBuild (monitor, conf.Id);
			
			isDirty = false;
			
			if (res != null) {
				string errorString = GettextCatalog.GetPluralString("{0} error", "{0} errors", res.ErrorCount, res.ErrorCount);
				string warningString = GettextCatalog.GetPluralString("{0} warning", "{0} warnings", res.WarningCount, res.WarningCount);
			
				monitor.Log.WriteLine(GettextCatalog.GetString("Build complete -- ") + errorString + ", " + warningString);
			}
			
			return res;
		}
		
		public void CopySupportFiles (IProgressMonitor monitor, string solutionConfiguration)
		{
			ProjectConfiguration config = (ProjectConfiguration) GetActiveConfiguration (solutionConfiguration);
			
			foreach (FileCopySet.Item item in GetSupportFileList (solutionConfiguration)) {
				string dest = Path.GetFullPath (Path.Combine (config.OutputDirectory, item.Target));
				string src = Path.GetFullPath (item.Src);
				
				try {
					if (dest == src)
						continue;
					
					if (item.CopyOnlyIfNewer && File.Exists (dest) &&
					    (File.GetLastWriteTimeUtc (dest) >=  File.GetLastWriteTimeUtc (src)))
						continue;
					
					if (!Directory.Exists (Path.GetDirectoryName (dest)))
						FileService.CreateDirectory (Path.GetDirectoryName (dest));
					
					if (File.Exists (src))
						FileService.CopyFile (src, dest);
					else
						monitor.ReportError (
							GettextCatalog.GetString ("Could not find support file '{0}'.", src), null);
					
				} catch (IOException ex) {
					monitor.ReportError (
						GettextCatalog.GetString ("Error copying support file '{0}'.", dest), ex);
				}
			}
		}
		
		public void DeleteSupportFiles (IProgressMonitor monitor, string solutionConfiguration)
		{
			ProjectConfiguration config = (ProjectConfiguration) GetActiveConfiguration (solutionConfiguration);
			
			foreach (FileCopySet.Item item in GetSupportFileList (solutionConfiguration)) {
				string dest = Path.Combine (config.OutputDirectory, item.Target);
				
				try {
					if (File.Exists (dest)) {
						FileService.DeleteFile (dest);
					}
				} catch (IOException ex) {
					monitor.ReportError (
						GettextCatalog.GetString ("Error deleting support file '{0}'.", dest), ex);
				}
			}
		}
		
		public FileCopySet GetSupportFileList (string solutionConfiguration)
		{
			FileCopySet list = new FileCopySet ();
			PopulateSupportFileList (list, solutionConfiguration);
			return list;
		}
		
		protected virtual void PopulateSupportFileList (FileCopySet list, string solutionConfiguration)
		{
			foreach (ProjectFile pf in Files) {
				if (pf.CopyToOutputDirectory == FileCopyMode.None)
					continue;
				string outpath = pf.IsExternalToProject? Path.GetFileName (pf.Name) : pf.RelativePath;
				list.Add (pf.FilePath, pf.CopyToOutputDirectory == FileCopyMode.PreserveNewest, outpath);
			}
		}
		
		protected virtual BuildResult DoBuild (IProgressMonitor monitor, string itemConfiguration)
		{
			BuildResult res = ItemHandler.RunTarget (monitor, "Build", itemConfiguration);
			return res ?? new BuildResult ();
		}
		
		protected internal override void OnClean (IProgressMonitor monitor, string solutionConfiguration)
		{
			SetDirty ();
			ProjectConfiguration config = GetActiveConfiguration (solutionConfiguration) as ProjectConfiguration;
			if (config == null) {
				monitor.ReportError (GettextCatalog.GetString ("Configuration '{0}' not found in project '{1}'", solutionConfiguration, Name), null);
				return;
			}
			
			// Delete the generated assembly
			string file = GetOutputFileName (solutionConfiguration);
			if (file != null) {
				if (File.Exists (file))
					FileService.DeleteFile (file);
			}
			
			DeleteSupportFiles (monitor, solutionConfiguration);

			DoClean (monitor, config.Id);
		}
		
		protected virtual void DoClean (IProgressMonitor monitor, string itemConfiguration)
		{
			ItemHandler.RunTarget (monitor, "Clean", itemConfiguration);
		}
		
		void GetBuildableReferencedItems (List<SolutionItem> referenced, SolutionItem item, string configuration)
		{
			if (referenced.Contains (item)) return;
			
			if (item.NeedsBuilding (configuration))
				referenced.Add (item);

			foreach (SolutionItem ritem in item.GetReferencedItems (configuration))
				GetBuildableReferencedItems (referenced, ritem, configuration);
		}
		
		protected internal override void OnExecute (IProgressMonitor monitor, ExecutionContext context, string solutionConfiguration)
		{
			ProjectConfiguration config = GetActiveConfiguration (solutionConfiguration) as ProjectConfiguration;
			if (config == null) {
				monitor.ReportError (GettextCatalog.GetString ("Configuration '{0}' not found in project '{1}'", solutionConfiguration, Name), null);
				return;
			}
			DoExecute (monitor, context, config.Id);
		}
		
		protected virtual void DoExecute (IProgressMonitor monitor, ExecutionContext context, string itemConfiguration)
		{
		}
		
		public string GetOutputFileName (string solutionConfiguration)
		{
			return OnGetOutputFileName (GetActiveConfigurationId (solutionConfiguration));
		}
		
		protected virtual string OnGetOutputFileName (string itemConfiguration)
		{
			return null;
		}
		
		protected internal override bool OnGetNeedsBuilding (string solutionConfiguration)
		{
			if (!isDirty) {
				if (CheckNeedsBuild (solutionConfiguration))
					SetDirty ();
			}
			return isDirty;
		}
		
		protected internal override void OnSetNeedsBuilding (bool value, string solutionConfiguration)
		{
			isDirty = value;
		}
		
		void SetDirty ()
		{
			if (!Loading)
				isDirty = true;
		}
		
		protected virtual bool CheckNeedsBuild (string solutionConfiguration)
		{
			DateTime tim = GetLastBuildTime (solutionConfiguration);
			if (tim == DateTime.MinValue)
				return true;
			
			foreach (ProjectFile file in Files) {
				if (file.BuildAction == BuildAction.Content || file.BuildAction == BuildAction.None)
					continue;
				try {
					if (File.GetLastWriteTime (file.FilePath) > tim)
						return true;
				} catch (IOException) {
					// Ignore.
				}
			}
			
			foreach (SolutionItem pref in GetReferencedItems (solutionConfiguration)) {
				if (pref.GetLastBuildTime (solutionConfiguration) > tim || pref.NeedsBuilding (solutionConfiguration))
					return true;
			}

			return false;
		}
		
		internal protected override DateTime OnGetLastBuildTime (string solutionConfiguration)
		{
			string conf = GetActiveConfigurationId (solutionConfiguration);
			string file = OnGetOutputFileName (conf);
			if (file == null)
				return DateTime.MinValue;

			FileInfo finfo = new FileInfo (file);
			if (!finfo.Exists) return DateTime.MinValue;
			else return finfo.LastWriteTime;
		}

		void OnFileChanged (object source, FileEventArgs e)
		{
			ProjectFile file = GetProjectFile (e.FileName);
			if (file != null) {
				SetDirty ();
				try {
					NotifyFileChangedInProject (file);
				} catch {
					// Workaround Mono bug. The watcher seems to
					// stop watching if an exception is thrown in
					// the event handler
				}
			}

		}
		
		internal protected override List<string> OnGetItemFiles (bool includeReferencedFiles)
		{
			List<string> col = base.OnGetItemFiles (includeReferencedFiles);
			if (includeReferencedFiles) {
				foreach (ProjectFile pf in Files) {
					if (pf.Subtype != Subtype.Directory)
						col.Add (pf.FilePath);
				}
			}
			return col;
		}
		
		protected internal override void OnItemAdded (object obj)
		{
			base.OnItemAdded (obj);
			if (obj is ProjectFile)
				NotifyFileAddedToProject ((ProjectFile) obj);
		}
		
		protected internal override void OnItemRemoved (object obj)
		{
			base.OnItemRemoved (obj);
			if (obj is ProjectFile)
				NotifyFileRemovedFromProject ((ProjectFile) obj);
		}
		
 		internal void NotifyFileChangedInProject (ProjectFile file)
		{
			OnFileChangedInProject (new ProjectFileEventArgs (this, file));
		}
		
 		internal void NotifyFilePropertyChangedInProject (ProjectFile file)
		{
			NotifyModified ("Files");
			OnFilePropertyChangedInProject (new ProjectFileEventArgs (this, file));
		}
		
		List<ProjectFile> unresolvedDeps;
		
		void NotifyFileRemovedFromProject (ProjectFile file)
		{
			file.SetProject (null);
			
			if (DependencyResolutionEnabled) {
				if (unresolvedDeps.Contains (file))
					unresolvedDeps.Remove (file);
				foreach (ProjectFile f in file.DependentChildren) {
					f.DependsOnFile = null;
					if (!string.IsNullOrEmpty (f.DependsOn))
						unresolvedDeps.Add (f);
				}
				file.DependsOnFile = null;
			}
			
			SetDirty ();
			NotifyModified ("Files");
			OnFileRemovedFromProject (new ProjectFileEventArgs (this, file));
		}
		
		void NotifyFileAddedToProject (ProjectFile file)
		{
			if (file.Project != null)
				throw new InvalidOperationException ("ProjectFile already belongs to a project");
			file.SetProject (this);
			
			ResolveDependencies (file);
			
			SetDirty ();
			NotifyModified ("Files");
			OnFileAddedToProject (new ProjectFileEventArgs (this, file));
		}
		
		internal void ResolveDependencies (ProjectFile file)
		{
			if (!DependencyResolutionEnabled)
				return;
			
			if (!file.ResolveParent ())
				unresolvedDeps.Add (file);
			
			List<ProjectFile> resolved = null;
			foreach (ProjectFile unres in unresolvedDeps) {
				if (string.IsNullOrEmpty (unres.DependsOn )) {
					resolved.Add (unres);
				}
				if (unres.ResolveParent ()) {
					if (resolved == null)
						resolved = new List<ProjectFile> ();
						resolved.Add (unres);
				}
			}
			if (resolved != null)
				foreach (ProjectFile pf in resolved)
					unresolvedDeps.Remove (pf);
		}
		
		bool DependencyResolutionEnabled {
			set {
				if (value) {
					if (unresolvedDeps != null)
						return;
					
					unresolvedDeps = new List<ProjectFile> ();
					foreach (ProjectFile file in projectFiles)
						ResolveDependencies (file);
				} else {
					unresolvedDeps = null;
				}
			}
			get { return unresolvedDeps != null; }
		}
		
		internal void NotifyFileRenamedInProject (ProjectFileRenamedEventArgs args)
		{
			SetDirty ();
			NotifyModified ("Files");
			OnFileRenamedInProject (args);
		}
		
		protected virtual void OnFileRemovedFromProject (ProjectFileEventArgs e)
		{
			buildActions = null;
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject (this, e);
			}
		}
		
		protected virtual void OnFileAddedToProject (ProjectFileEventArgs e)
		{
			buildActions = null;
			if (FileAddedToProject != null) {
				FileAddedToProject (this, e);
			}
		}

 		protected virtual void OnFileChangedInProject (ProjectFileEventArgs e)
		{
			if (FileChangedInProject != null) {
				FileChangedInProject (this, e);
			}
		}
		
 		protected virtual void OnFilePropertyChangedInProject (ProjectFileEventArgs e)
		{
			buildActions = null;
			if (FilePropertyChangedInProject != null) {
				FilePropertyChangedInProject (this, e);
			}
		}
		
 		protected virtual void OnFileRenamedInProject (ProjectFileRenamedEventArgs e)
		{
			if (FileRenamedInProject != null) {
				FileRenamedInProject (this, e);
			}
		}
				
		public event ProjectFileEventHandler FileRemovedFromProject;
		public event ProjectFileEventHandler FileAddedToProject;
		public event ProjectFileEventHandler FileChangedInProject;
		public event ProjectFileEventHandler FilePropertyChangedInProject;
		public event ProjectFileRenamedEventHandler FileRenamedInProject;
	}
	
	public class UnknownProject: Project
	{
		public override string ProjectType {
			get { return ""; }
		}

		public override SolutionItemConfiguration CreateConfiguration (string name)
		{
			return null;
		}
	}
}
