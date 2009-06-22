using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Xml.Linq;

namespace SvnTransfer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const String InfoCommand = "info";
        private const String DiffSummaryCommand = "diff summary";
        private const String DiffCommand = "diff";
        private const String CommandKey = "SvnTransfer_Command";
        private const String RevisionKey = "SvnTransfer_Revision";

        //private ProcessStartInfo _svnDiffSummaryStart;
        //private ProcessStartInfo _svnDiffStart;
        //private ProcessStartInfo _svnGetInfoStart;

        private Dictionary<Process, StringBuilder> _dataBufferMap = new Dictionary<Process, StringBuilder>();
        private Dictionary<Process, StringBuilder> _errorBufferMap = new Dictionary<Process, StringBuilder>();
        private Int32 _toRev;
        private String _from;
        private String _to;
        private Dictionary<String, ChangeType> _changeTypes = new Dictionary<String, ChangeType>();
        private SortedList<Int32, ChangeSet> _changeSets = new SortedList<Int32, ChangeSet>();
        private String _rootDir;

        public MainWindow()
        {
            InitializeComponent();

            _changeTypes["added"] = ChangeType.Added;
            _changeTypes["modified"] = ChangeType.Modified;
            _changeTypes["deleted"] = ChangeType.Deleted;

            createTempDir();
        }

        private void createTempDir()
        {
            _rootDir = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"),
                                          "SvnTransferDownloads");
            if (Directory.Exists(_rootDir))
            {
                Directory.Delete(_rootDir, true);
            }

            Directory.CreateDirectory(_rootDir);
        }

        private ProcessStartInfo createInfoStartInfo(String repository)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("info {0} --xml", repository)
            };

            startInfo.EnvironmentVariables[CommandKey] = InfoCommand;
            return startInfo;
        }

        private ProcessStartInfo createDiffSummaryStartInfo(String repository, Int32 rev)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("diff {0}@{1} {0}@{2} --xml --summarize", repository, rev - 1, rev)
            };

            startInfo.EnvironmentVariables[CommandKey] = DiffSummaryCommand;
            startInfo.EnvironmentVariables[RevisionKey] = rev.ToString();
            return startInfo;
        }

        private ProcessStartInfo createDiffStartInfo(String repository, Int32 rev)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
                                             {
                                                 CreateNoWindow = true,
                                                 RedirectStandardOutput = true,
                                                 RedirectStandardError = true,
                                                 RedirectStandardInput = true,
                                                 UseShellExecute = false,
                                                 Arguments = String.Format("diff {0}@{1} {0}@{2}", repository, rev - 1, rev)
                                             };

            startInfo.EnvironmentVariables[CommandKey] = DiffCommand;
            startInfo.EnvironmentVariables[RevisionKey] = rev.ToString();
            return startInfo;
        }

        private ProcessStartInfo createExportStartInfo(string toPath, string file, Int32 rev)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("export -r {0} {1} {2}", rev, file, toPath)
            };

            return startInfo;
        }

        private void handleButtonClick(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(processDiffs);
        }

        private void processDiffs(object state)
        {
            for (Int32 rev = 1; rev < _toRev; rev++)
            {
                ProcessStartInfo startInfo = createDiffSummaryStartInfo(_from, rev);
                startProcess(startInfo);

                Thread.Sleep(2000);
            }
        }

        private void handleLostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(FromRepositoryEntry.Text))
            {
                return;
            }

            if (FromRepositoryEntry.Text.Last() != '/')
            {
                FromRepositoryEntry.Text += "/";
            }

            if (String.Equals(_from, FromRepositoryEntry.Text))
            {
                return;
            }

            _from = FromRepositoryEntry.Text;
            ProcessStartInfo startInfo = createInfoStartInfo(_from);
            startProcess(startInfo);
        }

        private void handleProcessExit(object sender, EventArgs e)
        {
            Process process = sender as Process;

            if (process == null)
            {
                throw new InvalidOperationException("Unexpectedly, the sender of process exit event is not a process.");
            }

            StringBuilder dataBuffer;
            XDocument doc;

            if (_dataBufferMap.TryGetValue(process, out dataBuffer))
            {
                switch (process.StartInfo.EnvironmentVariables[CommandKey])
                {
                    case InfoCommand:
                        {
                            doc = XDocument.Parse(dataBuffer.ToString());
                            String rev = (from elem in doc.Descendants()
                                          where elem.Name == "entry" && elem.Attribute("revision") != null
                                          select elem.Attribute("revision").Value).FirstOrDefault();
                            _toRev = Int32.Parse(rev);
                            String path = (from elem in doc.Descendants()
                                           where elem.Name == "entry" && elem.Attribute("path") != null
                                           select elem.Attribute("path").Value).FirstOrDefault();
                            _rootDir = Path.Combine(_rootDir, path);
                            Directory.CreateDirectory(_rootDir);
                            FromRepositoryToRevEntry.Dispatcher.BeginInvoke(
                                (Action)(() => FromRepositoryToRevEntry.Text = rev));
                            break;
                        }
                    case DiffSummaryCommand:
                        {
                            doc = XDocument.Parse(dataBuffer.ToString());
                            Int32 rev = Int32.Parse(process.StartInfo.EnvironmentVariables[RevisionKey]);
                            var diffs = from elem in doc.Descendants()
                                        where elem.Name == "path"
                                        let hasPropChanges = elem.Attribute("props").Value != "none"
                                        let kind = elem.Attribute("kind").Value
                                        let changeType = elem.Attribute("item").Value
                                        let path = elem.Value
                                        select new Diff()
                                                   {
                                                       Kind = kind == "file" ? TargetKind.File : TargetKind.Dir,
                                                       Change = _changeTypes[changeType],
                                                       Path = path
                                                   };

                            _changeSets.Add(rev, new ChangeSet() { Diffs = diffs.ToList() });

                            var startInfo = createDiffStartInfo(_from, rev);
                            startProcess(startInfo);
                            break;
                        }
                    case DiffCommand:
                        {
                            String data = dataBuffer.ToString();

                            if (String.IsNullOrEmpty(data))
                            {
                                break;
                            }

                            Int32 rev = Int32.Parse(process.StartInfo.EnvironmentVariables[RevisionKey]);
                            var binaryFiles = findBinaryFiles(data);
                            ChangeSet changeSet = _changeSets[rev];

                            String downloadPath = Path.Combine(_rootDir, rev.ToString());
                            Directory.CreateDirectory(downloadPath);

                            foreach (var file in binaryFiles)
                            {
                                String sourceFile = _from + file;
                                downloadPath = Path.Combine(downloadPath, Path.GetFileName(file));
                                download(downloadPath, sourceFile, rev);

                                Diff diff = changeSet.Diffs.Where(d => d.Path == sourceFile).FirstOrDefault();

                                if (diff != null)
                                {
                                    diff.BinaryFilePath = downloadPath;
                                }
                            }

                            break;
                        }
                }
            }

            closeProcess(process);
        }

        private void download(string toPath, string file, Int32 rev)
        {
            ProcessStartInfo startInfo = createExportStartInfo(toPath, file, rev);
            startProcess(startInfo);
        }

        private static Regex _findBinaryFiles = new Regex(@"^Index:\w*(?<IndexName>.*)\r\n=*\r\nCannot display: file marked as a binary type.", RegexOptions.Multiline | RegexOptions.Compiled);

        private static IEnumerable<String> findBinaryFiles(String data)
        {
            MatchCollection matches = _findBinaryFiles.Matches(data);

            foreach (Match match in matches)
            {
                String indexName = match.Groups["IndexName"].Value;
                yield return indexName.Trim();
            }
        }

        private void handleOutputData(object sender, DataReceivedEventArgs e)
        {
            Process process = sender as Process;

            if (process == null)
            {
                throw new InvalidOperationException("Unexpectedly, the sender of process exit event is not a process.");
            }

            StringBuilder dataBuffer;

            if (_dataBufferMap.TryGetValue(process, out dataBuffer))
            {
                String data = e.Data;
                //Debug.WriteLine(process.StartInfo.FileName + ": " + data ?? "");
                if (!String.IsNullOrEmpty(data))
                {
                    dataBuffer.AppendLine(data);
                }
            }
        }

        private void handleErrorData(object sender, DataReceivedEventArgs e)
        {
            Process process = sender as Process;

            if (process == null)
            {
                throw new InvalidOperationException("Unexpectedly, the sender of process exit event is not a process.");
            }

            StringBuilder errorBuffer;

            if (_errorBufferMap.TryGetValue(process, out errorBuffer))
            {
                String data = e.Data;

                if (!String.IsNullOrEmpty(data))
                {
                    errorBuffer.AppendLine(data);
                }
            }
        }

        private void startProcess(ProcessStartInfo startInfo)
        {
            Process process = new Process();

            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;
            process.Exited += handleProcessExit;
            process.OutputDataReceived += handleOutputData;
            process.ErrorDataReceived += handleErrorData;
            _dataBufferMap[process] = new StringBuilder(4096);
            _errorBufferMap[process] = new StringBuilder(1024);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void closeProcess(Process process)
        {
            _dataBufferMap.Remove(process);
            _errorBufferMap.Remove(process);
            process.Exited -= handleProcessExit;
            process.Dispose();
        }
    }

    internal enum ChangeType
    {
        None,
        Added,
        Modified,
        Deleted
    }

    internal enum TargetKind
    {
        File,
        Dir
    }

    internal class ChangeSet
    {
        public IList<Diff> Diffs { get; set; }
        public String UnifiedPatch { get; set; }
    }

    internal class Diff
    {
        public String Path { get; set; }
        public TargetKind Kind { get; set; }
        public ChangeType Change { get; set; }
        public String BinaryFilePath { get; set; }
    }
}
