//------------------------------------------------------------------------------
// <copyright file="VSPackage1.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace VSIXProject1
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(VSPackage1.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class VSPackage1 : Package
    {
        /// <summary>
        /// VSPackage1 GUID string.
        /// </summary>
        public const string PackageGuidString = "106c895f-9f99-499f-929d-e5a96629ecbd";

        /// <summary>
        /// Initializes a new instance of the <see cref="VSPackage1"/> class.
        /// </summary>
        public VSPackage1()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }
        private string args;

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                var menuCommandID = new CommandID(GuidList.s_GuidVSPackageTestCmdSet, (int)PkgCmdIDList.c_CmdCommand1);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
            Command1.Initialize(this);
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            // Show a Message Box to prove we were here
            //var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            //Guid clsid = Guid.Empty;

            var dte1 = GetGlobalService(typeof(DTE)) as DTE;
            //STARTING POINT

            args = string.Empty;
            if (dte1 != null)
            {
                foreach (SelectedItem item in dte1.SelectedItems)
                {
                    string itemm = item.Name;
                    if (!string.IsNullOrEmpty(itemm))
                    {
                        if (item.ProjectItem != null)
                        {
                            //args = string.IsNullOrEmpty(args) ? string.Empty : args;
                            if (item.ProjectItem.FileNames[0].Contains(itemm))
                                args += (" \"" + item.ProjectItem.FileNames[0]) + "\"";
                        }
                        else
                        {
                            if (item.Project.ProjectItems != null)
                            {
                                foreach (ProjectItem projectItem in item.Project.ProjectItems)
                                {
                                    try
                                    {
                                        const short i = 0;
                                        //for (short i = 0; i < 1; i++)
                                        //{
                                        string itemms = projectItem.FileNames[i];

                                        if (!string.IsNullOrEmpty(itemms))
                                        {
                                            if (IsCodeFile(itemms))
                                            {
                                                args += (" \"" + projectItem.FileNames[0]) + "\"";
                                            }

                                            foreach (ProjectItem internalProjItem in projectItem.ProjectItems)
                                            {
                                                try
                                                {
                                                    GetValue(internalProjItem);
                                                }
                                                catch (Exception eee)
                                                {
                                                    Console.WriteLine(eee.StackTrace);
                                                }
                                            }
                                        }
                                        //}
                                    }

                                    catch (Exception ee)
                                    {
                                        Console.WriteLine(ee.StackTrace);
                                    }
                                }
                            }
                        }
                    }
                    //else
                    //{

                    //}
                }
            }
            OutputWindowPane simianRunOutputPane = GetSimianPaneByName("Simian");
            if (simianRunOutputPane != null)
                simianRunOutputPane.Clear();

            if (dte1 != null)
            {
                string slnName = ((SolutionClass)(dte1.Solution)).FullName;
                if (!string.IsNullOrEmpty(slnName))
                {
                    foreach (Project project in ((SolutionClass)(dte1.Solution)).Projects)
                    {
                        SimianCall(project.FullName);
                    }
                }
                else
                {
                    MessageBox.Show(Resources.VSPackageTestPackage_MenuItemCallback_Please_Open_a_Solution_or_project_and_Click_again_, Resources.VSPackageTestPackage_MenuItemCallback_No_Project_or_Solution);
                }
            }
            //int result;
            //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
            //           0,
            //           ref clsid,
            //           "Test Package",
            //           string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
            //           string.Empty,
            //           0,
            //           OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
            //           OLEMSGICON.OLEMSGICON_INFO,
            //           0,        // false
            //           out result));
        }

        private static bool IsCodeFile(string itemms)
        {
            return itemms.EndsWith(".cs")
                   || itemms.EndsWith(".h")
                   || itemms.EndsWith(".cpp")
                   || itemms.EndsWith(".c")
                   || itemms.EndsWith(".vb")
                   || itemms.EndsWith(".mdd")
                   || itemms.EndsWith(".mdd")
                   || itemms.EndsWith(".resx");
        }

        private void GetValue(ProjectItem projItem)
        {
            const short ii = 0;
            //for (short ii = 0; ii < 1; ii++)
            {
                string itemmSs = projItem.FileNames[ii];

                if (!string.IsNullOrEmpty(itemmSs))
                {
                    if (IsCodeFile(itemmSs))
                    {
                        args += (" \"" + projItem.FileNames[0]) + "\"";
                    }
                }
            }
        }

        private void SimianCall(string project)
        {
            //string activePaneGuid = _dte2.ToolWindows.OutputWindow.ActivePane.Guid;
            OutputWindowPane simianRunOutputPane = GetSimianPaneByName("Simian");

            //SimianRunOutputPane.Clear();
            RunAndWriteToOutputPane(simianRunOutputPane, project);

            //SetActivePane(activePaneGuid);
        }

        private void RunAndWriteToOutputPane(OutputWindowPane simianRunOutputPane, string csproj)
        {
            if (!string.IsNullOrEmpty(csproj))
            {
                csproj = GetAbsolutePath(csproj);
                //if (!runsimian) return;
                //SimianRunOutputPane.OutputString(
                //    @"C:\Users\IC007161\Documents\Visual Studio 2010\Projects\ExpTestSln\ExpTestSln\Program.cs(17,17): warning CS0219: The variable 'a' is assigned but its value is never used\r\n");
                //SimianRunOutputPane.OutputString(@"C:\Users\IC007161\Documents\Visual Studio 2010\Projects\ExpTestSln\ExpTestSln\Program.cs\r\n");
                //SimianRunOutputPane.OutputString("Ran Simian at: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString()+ "\r\n");
                //SimianRunOutputPane.OutputString("Default Dir "+ Environment.CurrentDirectory + "\r\n");
                //SimianRunOutputPane.OutputString("Assembly Dir " + Assembly.GetExecutingAssembly().Location + "\r\n");

                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (dir != null)
                {
                    string exe = Path.Combine(dir, @"payload\simian-2.3.33.exe");

                    //foreach (string csproj in GetCsprojs())
                    {
                        if (!string.IsNullOrEmpty(csproj))
                            RunProcess(exe, simianRunOutputPane);
                    }
                }
            }
        }

        private void RunProcess(string exe, OutputWindowPane simianRunOutputPane)
        {
            const int timeout = 1 * 60 * 1000;
            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = exe;
                process.StartInfo.Arguments = args;
                ///"-threshold=10 " +  args;
                //GetArgs(csproj);
                if (string.IsNullOrEmpty(process.StartInfo.Arguments)) return;

                #region test

                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                var output = new StringBuilder();
                var error = new StringBuilder();

                using (var outputWaitHandle = new AutoResetEvent(false))
                using (var errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            // ReSharper disable AccessToDisposedClosure
                            outputWaitHandle.Set();

                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                            // ReSharper restore AccessToDisposedClosure
                        }
                        else
                        {
                            error.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(timeout) &&
                        outputWaitHandle.WaitOne(timeout) &&
                        errorWaitHandle.WaitOne(timeout))
                    {
                        // Process completed. Check process.ExitCode here.
                    }
                    // ReSharper disable RedundantIfElseBlock
                    else
                    {
                        // Timed out.
                    }
                    // ReSharper restore RedundantIfElseBlock
                }

                simianRunOutputPane.OutputString(
                    //"Call to exe = " + exe + " " + process.StartInfo.Arguments + Environment.NewLine +
                    FormatString(output.ToString()) + Environment.NewLine
                    );

                #endregion

                #region old

                //pp.StartInfo.CreateNoWindow = true;
                //process.StartInfo.UseShellExecute = false;
                //process.StartInfo.RedirectStandardOutput = true;                    
                //process.Start();
                //using (StreamReader streamReader = process.StandardOutput)//new StreamReader(pp.StandardOutput as Stream, pp.StandardOutput.CurrentEncoding, false, Int32.MaxValue - 10))
                //{
                //    string x = streamReader.ReadLine();
                //    process.WaitForExit();
                //    SimianRunOutputPane.OutputString("Call to exe = " + exe + " " + process.StartInfo.Arguments
                //                                     + Environment.NewLine + FormatString(x + Environment.NewLine + streamReader.ReadToEnd()) +
                //                                     Environment.NewLine);
                //}

                #endregion

            }
        }

        private string GetAbsolutePath(string csproj)
        {
            var dte1 = GetGlobalService(typeof(DTE)) as DTE;
            if (dte1 != null && dte1.ActiveSolutionProjects != null)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(((SolutionClass)(dte1.Solution)).FullName), csproj));
                // ReSharper restore AssignNullToNotNullAttribute
            }
            return csproj;
        }


        private string FormatString(string p)
        {
            //Between lines 486 and 493 in F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs
            //F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs(144,21): error CS0219: Warning as Error: The variable 'b' is assigned but its value is never used
            //F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs(486,1): Duplicate between lines 486 and 493
            var strb = new StringBuilder();
            foreach (var line in p.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (
                       line.Contains(Resources.Exception_01)
                    || line.Contains(Resources.Exception_02)
                    || line.Contains(Resources.Exception_03)
                    || (line.StartsWith("{") && line.EndsWith("}"))
                    )
                {
                    //Do nothing
                }
                else if (line.Contains(Resources.Check_Between_Lines) && line.Contains(Resources.Check_in))
                {
                    string[] lineParts = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    string fileName = lineParts[6];
                    if (lineParts.Length > 7)
                    {
                        fileName = "";
                        for (int i = 6; i < lineParts.Length; i++)
                        {
                            fileName += (" " + lineParts[i]);
                        }
                    }
                    strb.AppendLine(fileName + "(" + lineParts[2] + Resources.Check_Duplicate_found_between_lines + lineParts[2] +
                                    " " + lineParts[3] + " " + lineParts[4]);
                }
                else
                {
                    strb.AppendLine(line);
                }
            }
            return strb.ToString();
        }

        /*
                private string GetArgs(string csproj)
                {
                    //string csproj = GetCsproj();
                    //string csproj = @"D:\WS\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Hwcn.Migration.csproj";
                    string listOfcsFiles = GetCSFilesFromCsProj(csproj);
                    return listOfcsFiles;
                    //return @"F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs";
                }
        */

        /*
                private string[] GetCsprojs()
                {
                    List<string> csprojs = new List<string>();
                    string nam = string.Empty;
                    DTE dte1 = Package.GetGlobalService(typeof(DTE)) as DTE;
                    if (dte1 != null && dte1.ActiveSolutionProjects != null)
                    {
                        for (int i = 1; i <= (((EnvDTE.SolutionClass)(dte1.Solution))).Projects.Count; i++)
                        {
                            nam = ((((EnvDTE.SolutionClass)(dte1.Solution))).Projects).Item(i).FullName;
                            csprojs.Add(nam);
                        }
                    }
                    return csprojs.ToArray();
                }
        */

        /*
                private string GetCSFilesFromCsProj(string csproj)
                {
                    StringBuilder strb = new StringBuilder();
                    string basePath = Path.GetDirectoryName(csproj);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(csproj);
                    XmlNodeList compileNodes = doc.GetElementsByTagName("Compile");
                    foreach (XmlNode compileNode in compileNodes)
                    {
                        string file = compileNode.Attributes["Include"].Value;
                        string fullName = Path.Combine(basePath, file);
                        strb.Append("\"" + fullName + "\" ");
                    }

                    return strb.ToString();
                }
        */

        private DTE2 dte2;

        private OutputWindowPane GetSimianPaneByName(string name)
        {
            dte2 = GetGlobalService(typeof(DTE)) as DTE2;

            //_dte2 = serviceProvider.GetService(typeof(DTE)) as DTE2;
            //if (_dte2 != null)
            if (dte2 != null)
            {
                OutputWindowPane simianRunOutputPane = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Cast<OutputWindowPane>().FirstOrDefault(spane => spane.Name == name);
                simianRunOutputPane = simianRunOutputPane ?? dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(name);
                return simianRunOutputPane;
            }
            return null;
        }

        /*
                private void SetActivePane(string activePaneGuid)
                {
                    foreach (OutputWindowPane sspane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
                    {
                        if (sspane.Guid == activePaneGuid)
                        {
                            sspane.Activate();
                            break;
                        }
                    }
                }
        */


    }
}
