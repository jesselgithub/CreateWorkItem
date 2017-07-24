using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace VSIXProject1
{
    public class BuildEvents
    {
        private readonly DTE2 _dte2;
        private readonly Events _events;
        private readonly EnvDTE.BuildEvents _buildEvents;
        private readonly DTEEvents _dteEvents;
        private DateTime _buildStartTime;
        private readonly List<string> _projectsBuildReport;
        public bool StopOnBuildErrorEnabled { get; set; }
        public bool ShowElapsedBuildTimeEnabled { get; set; }
        public bool ShowBuildReport { get; set; }
        public bool ShowDebugWindowOnDebug { get; set; }

        public BuildEvents(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                return;
            }
            _dte2 = serviceProvider.GetService(typeof(DTE)) as DTE2;
            if (_dte2 != null)
            {
                // These event sources have to be rooted or the GC will collect them.
                // http://social.msdn.microsoft.com/Forums/en-US/vsx/thread/fd2f9108-1df3-4d96-a65d-67a69347ca27
                _events = _dte2.Events;
                //_dte2.Commands.
                _buildEvents = _events.BuildEvents;
                _dteEvents = _events.DTEEvents;

                _buildEvents.OnBuildBegin += OnBuildBegin;
                _buildEvents.OnBuildDone += OnBuildDone;
                _buildEvents.OnBuildProjConfigDone += OnBuildProjectDone;
                _dteEvents.ModeChanged += OnModeChanged;
            }
            _projectsBuildReport = new List<string>();
        }

        private void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            _projectsBuildReport.Clear();
            _buildStartTime = DateTime.Now;

            OutputWindowPane SimianRunOutputPane = GetSimianPaneByName("Simian");
            if (SimianRunOutputPane != null)
                SimianRunOutputPane.Clear();
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            //string activePaneGuid = _dte2.ToolWindows.OutputWindow.ActivePane.Guid;
            //OutputWindowPane SimianRunOutputPane = GetSimianPaneByName("Simian");

            //SimianRunOutputPane.Clear();
            //RunAndWriteToOutputPane(SimianRunOutputPane);

            //SetActivePane(activePaneGuid);

            OutputWindowPane BuildOutputPane = null;
            foreach (OutputWindowPane pane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (pane.Guid == VSConstants.OutputWindowPaneGuid.BuildOutputPane_string)
                {
                    BuildOutputPane = pane;
                    break;
                }
            }

            if (BuildOutputPane == null)
            {
                return;
            }

            if (ShowBuildReport)
            {
                BuildOutputPane.OutputString("\r\nProjects build report:\r\n");
                BuildOutputPane.OutputString("  Status    | Project [Config|platform]\r\n");
                BuildOutputPane.OutputString(" -----------|---------------------------------------------------------------------------------------------------\r\n");
                foreach (string ReportItem in _projectsBuildReport)
                {
                    BuildOutputPane.OutputString(ReportItem + "\r\n");
                    //BuildOutputPane.OutputString("Added by Jessel" + "\r\n");
                }
            }

            if (ShowElapsedBuildTimeEnabled)
            {
                var elapsed = DateTime.Now - _buildStartTime;
                var time = elapsed.ToString(@"hh\:mm\:ss\.ff");
                var text = string.Format("Time Elapsed {0}", time);
                BuildOutputPane.OutputString("\r\n" + text + "\r\n");
            }
        }

        private void RunAndWriteToOutputPane(OutputWindowPane SimianRunOutputPane, string csproj, bool runsimian)
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
            string exe = Path.Combine(dir, @"payload\simian-2.3.33.exe");
            //foreach (string csproj in GetCsprojs())
            {
                if (!string.IsNullOrEmpty(csproj))
                    RunProcess(csproj, exe, SimianRunOutputPane);
            }
        }

        private string GetAbsolutePath(string csproj)
        {
            DTE dte1 = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte1 != null && dte1.ActiveSolutionProjects != null)
            {
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(((EnvDTE.SolutionClass)(dte1.Solution)).FullName), csproj));
            }
            return csproj;
        }

        private void RunProcess(string csproj, string exe, OutputWindowPane SimianRunOutputPane)
        {
            int timeout = 1 * 60 * 1000;
            using (var process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = exe;
                process.StartInfo.Arguments = GetArgs(csproj);
                if (string.IsNullOrEmpty(process.StartInfo.Arguments)) return;

                #region test

                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
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
                    else
                    {
                        // Timed out.
                    }
                }

                SimianRunOutputPane.OutputString(
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

        private string FormatString(string p)
        {
            //Between lines 486 and 493 in F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs
            //F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs(144,21): error CS0219: Warning as Error: The variable 'b' is assigned but its value is never used
            //F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs(486,1): Duplicate between lines 486 and 493
            StringBuilder strb = new StringBuilder();
            foreach (var line in p.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("Between lines ") && line.Contains(" in "))
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
                    strb.AppendLine(fileName + "(" + lineParts[2] + ",1): Duplicate found between lines " + lineParts[2] +
                                    " " + lineParts[3] + " " + lineParts[4]);
                }
                else
                {
                    strb.AppendLine(line);
                }
            }
            return strb.ToString();
        }

        private string GetArgs(string csproj)
        {
            //string csproj = GetCsproj();
            //string csproj = @"D:\WS\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Hwcn.Migration.csproj";
            string listOfcsFiles = GetCSFilesFromCsProj(csproj);
            return listOfcsFiles;
            //return @"F:\ws\W2_WinCC_HWCNRQ_HW\src\HWCN\MIG\HwcnMigration\Migration\HwcnUtility.cs";
        }

        //private string[] GetCsprojs()
        //{
        //    List<string> csprojs = new List<string>();
        //    string nam = string.Empty;
        //    DTE dte1 = Package.GetGlobalService(typeof(DTE)) as DTE;
        //    if (dte1 != null && dte1.ActiveSolutionProjects != null)
        //    {
        //        for (int i = 1; i <= (((EnvDTE.SolutionClass)(dte1.Solution))).Projects.Count; i++)
        //        {
        //            nam = ((((EnvDTE.SolutionClass)(dte1.Solution))).Projects).Item(i).FullName;
        //            csprojs.Add(nam);
        //        }
        //    }
        //    return csprojs.ToArray();
        //}

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

        private OutputWindowPane GetSimianPaneByName(string name)
        {
            OutputWindowPane SimianRunOutputPane = null;
            foreach (OutputWindowPane spane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (spane.Name == name)
                {
                    SimianRunOutputPane = spane;
                    break;
                }
            }
            SimianRunOutputPane = SimianRunOutputPane == null
                                      ? _dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(name)
                                      : SimianRunOutputPane;
            return SimianRunOutputPane;
        }

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

        private void OnBuildProjectDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            SimianCall(project);

            if (StopOnBuildErrorEnabled && success == false)
            {
                const string cancelBuildCommand = "Build.Cancel";
                _dte2.ExecuteCommand(cancelBuildCommand);
            }

            if (ShowBuildReport)
            {
                _projectsBuildReport.Add("  " + (success ? "Succeeded" : "Failed   ") + " | " + project + " [" + projectConfig + "|" + platform + "]");
                //_projectsBuildReport.Add("Added by Me");
            }
        }

        private void SimianCall(string project)
        {
            string activePaneGuid = _dte2.ToolWindows.OutputWindow.ActivePane.Guid;
            OutputWindowPane SimianRunOutputPane = GetSimianPaneByName("Simian");

            //SimianRunOutputPane.Clear();
            RunAndWriteToOutputPane(SimianRunOutputPane, project, ShowBuildReport);

            SetActivePane(activePaneGuid);
        }

        private void OnModeChanged(vsIDEMode lastMode)
        {
            if (lastMode == vsIDEMode.vsIDEModeDesign && ShowDebugWindowOnDebug)
            {
                _dte2.ToolWindows.OutputWindow.Parent.Activate();
                foreach (OutputWindowPane pane in _dte2.ToolWindows.OutputWindow.OutputWindowPanes)
                {
                    if (pane.Guid == VSConstants.OutputWindowPaneGuid.DebugPane_string)
                    {
                        pane.Activate();
                        break;
                    }
                }
            }
        }

    }
}