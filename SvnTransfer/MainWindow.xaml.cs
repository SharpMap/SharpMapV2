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
        private const String RetrieveLogsCommand = "logs";
        private const String InfoCommand = "info";
        private const String DiffSummaryCommand = "diff summary";
        private const String DiffCommand = "diff";
        private const String ExportCommand = "export";
        private const String PatchCommand = "patch";
        private const String CopyBinariesCommand = "copyBinaries";
        private const String CommitCommand = "commit";
        private const String CommandKey = "SvnTransfer_Command";
        private const String RevisionKey = "SvnTransfer_Revision";

        private static Regex _findBinaryFiles = new Regex(@"^Index:\w*(?<IndexName>.*)\r\n=*\r\nCannot display: file marked as a binary type.", RegexOptions.Multiline | RegexOptions.Compiled);

        //private ProcessStartInfo _svnDiffSummaryStart;
        //private ProcessStartInfo _svnDiffStart;
        //private ProcessStartInfo _svnGetInfoStart;

        private Dictionary<Process, StringBuilder> _dataBufferMap = new Dictionary<Process, StringBuilder>();
        private Dictionary<Process, StringBuilder> _errorBufferMap = new Dictionary<Process, StringBuilder>();
        private Int32 _toRev;
        private Int32 _fromRev;
        private String _from;
        private String _to;
        private Dictionary<String, ChangeType> _changeTypes = new Dictionary<String, ChangeType>();
        private SortedList<Int32, ChangeSet> _changeSets = new SortedList<Int32, ChangeSet>();
        private SortedList<Int32, LogMessage> _logs = new SortedList<int, LogMessage>();
        private String _rootDir;
        private ProcessSync _sync = new ProcessSync();

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
                foreach (var dir in Directory.GetDirectories(_rootDir, "*", SearchOption.AllDirectories))
                {
                    new DirectoryInfo(dir).Attributes = FileAttributes.Normal;
                }

                foreach (var file in Directory.GetFiles(_rootDir, "*", SearchOption.AllDirectories))
                {
                    new FileInfo(file).Attributes = FileAttributes.Normal;
                }

                // HACK: not sure why the first attempt is only mostly successful...
                try
                {
                    Directory.Delete(_rootDir, true);
                }
                catch (Exception)
                {
                    Directory.Delete(_rootDir, true);
                }
            }

            Directory.CreateDirectory(_rootDir);
        }

        private static ProcessStartInfo createDeleteStartInfo(String delPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("delete {0}", delPath)
            };

            return startInfo;
        }

        private static ProcessStartInfo createAddStartInfo(String addPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("add {0}", addPath)
            };

            return startInfo;
        }

        private static ProcessStartInfo createInfoStartInfo(String repository)
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

        private static ProcessStartInfo createDiffSummaryStartInfo(String repository, Int32 rev)
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

        private static ProcessStartInfo createDiffStartInfo(String repository, Int32 rev)
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

        private static ProcessStartInfo createCommitStartInfo(String wc, String logMessage)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("commit {0} --message \"{1}\"", wc, logMessage)
            };

            return startInfo;
        }

        private static ProcessStartInfo createExportStartInfo(String toPath, String file, Int32 rev)
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

            startInfo.EnvironmentVariables[CommandKey] = ExportCommand;
            return startInfo;
        }

        private static ProcessStartInfo createPatchStartInfo(String workingCopy, String patchFile)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("merge.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("--unified {0} /diff {1}", patchFile, workingCopy)
            };

            startInfo.EnvironmentVariables[CommandKey] = PatchCommand;
            return startInfo;
        }

        private static ProcessStartInfo createCheckoutStartInfo(String repository, String workingDir)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("checkout {0} {1}", repository, workingDir)
            };

            return startInfo;
        }

        private static ProcessStartInfo createRetrieveLogsStartInfo(String repository)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("svn.exe")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = String.Format("log {0} --xml", repository)
            };

            startInfo.EnvironmentVariables[CommandKey] = RetrieveLogsCommand;
            return startInfo;
        }

        private void processDiffs(object state)
        {
            if (String.IsNullOrEmpty(_from))
            {
                Dispatcher.Invoke((Action)setFromRepository);
            }

            if (String.IsNullOrEmpty(_to))
            {
                Dispatcher.Invoke((Action)setToRepository);
            }

            retrieveLogs();

            for (Int32 rev = _fromRev + 1; rev < _toRev; rev++)
            {
                ChangeSet changeSet = getChangeSet(rev);
                _changeSets.Add(rev, changeSet);

                String diff = getDiff(rev);
                changeSet.LogMessage = _logs[rev];

                if (diff == null)
                {
                    continue;
                }

                addPatchAndBinaryFiles(diff, rev);
            }
            
            String wc = checkoutWorkingCopy();

            foreach (var set in _changeSets)
            {
                Int32 rev = set.Key;
                ChangeSet changeSet = set.Value;

                foreach (Diff diff in changeSet.Diffs)
                {
                    switch (diff.Change)
                    {
                        case ChangeType.Added:
                            if (String.IsNullOrEmpty(diff.BinaryFilePath))
                            {
                                break;
                            }

                            String addPath = Path.Combine(wc, diff.RelativePath.Replace('/', '\\'));

                            if (diff.Kind == TargetKind.Dir)
                            {
                                Directory.CreateDirectory(addPath);
                            }
                            else 
                            {
                                File.Create(addPath).Close();
                            }

                            runSvn(createAddStartInfo(addPath));
                            break;
                        case ChangeType.Deleted:
                            String delPath = Path.Combine(wc, diff.RelativePath.Replace('/', '\\'));

                            runSvn(createDeleteStartInfo(delPath));
                            break;
                    }
                }

                if (!String.IsNullOrEmpty(changeSet.UnifiedPatch))
                {
                    runSvn(createPatchStartInfo(wc, changeSet.UnifiedPatch));
                    String downloadPath = Path.Combine(_rootDir, rev.ToString());
                    copyBinariesToTarget(downloadPath, rev, wc);
                }

                runSvn(createCommitStartInfo(wc, changeSet.LogMessage.Message));
            }
        }

        private void copyBinariesToTarget(String root, Int32 rev, String wc)
        {
            String srcRoot = Path.Combine(Path.Combine(root, rev.ToString()), "binaries");

            foreach (String srcFile in Directory.GetFiles(srcRoot, "*", SearchOption.AllDirectories))
            {
                String dstFile = srcFile.Remove(srcRoot.Length);
                dstFile = Path.Combine(wc, dstFile);
                File.Copy(srcFile, dstFile, true);
                runSvn(createAddStartInfo(dstFile));
            }
        }

        private String checkoutWorkingCopy()
        {
            String workingCopyPath = Path.Combine(_rootDir, "ToRepositoryWorkingCopy");
            Directory.CreateDirectory(workingCopyPath);

            ProcessStartInfo startInfo = createCheckoutStartInfo(_to, workingCopyPath);
            Process process = startProcess(startInfo);
            process.WaitForExit();
            closeProcess(process);

            return workingCopyPath;
        }

        private String getDiff(int rev)
        {
            String data = runSvn(createDiffStartInfo(_from, rev));

            return String.IsNullOrEmpty(data) ? null : data;
        }

        private String runSvn(ProcessStartInfo startInfo) 
        {
            Process process = startProcess(startInfo);

            String data = process.StandardOutput.ReadToEnd();
            String error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            closeProcess(process);

            if (!String.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            return data;
        }

        private void addPatchAndBinaryFiles(String patch, int rev)
        {
            var binaryFiles = findBinaryFiles(patch);
            var changeSet = _changeSets[rev];

            String downloadPath = Path.Combine(_rootDir, rev.ToString());
            Directory.CreateDirectory(downloadPath);

            changeSet.UnifiedPatch = Path.Combine(downloadPath, rev + ".patch");
            File.WriteAllText(changeSet.UnifiedPatch, patch);

            foreach (var file in binaryFiles)
            {
                String sourceFile = _from + file;
                String localFile = file.Replace('/', '\\');
                downloadPath = Path.Combine(downloadPath, "binaries");
                createPath(downloadPath, Path.GetDirectoryName(localFile));
                downloadPath = Path.Combine(downloadPath, localFile);

                runSvn(createExportStartInfo(downloadPath, sourceFile, rev));

                Diff diff = changeSet.Diffs.Where(d => d.FullPath == sourceFile).FirstOrDefault();

                if (diff != null)
                {
                    diff.BinaryFilePath = downloadPath;
                }
            }
        }

        private void createPath(String root, String file)
        {
            String[] components = file.Split(Path.PathSeparator);

            foreach (String component in components)
            {
                root = Path.Combine(root, component);
                Directory.CreateDirectory(root);
            }
        }

        private ChangeSet getChangeSet(int rev)
        {
            String data = runSvn(createDiffSummaryStartInfo(_from, rev));

            XDocument doc;

            doc = XDocument.Parse(data);
            var diffs = from elem in doc.Descendants()
                        where elem.Name == "path"
                        let hasPropChanges = elem.Attribute("props").Value != "none"
                        let kind = elem.Attribute("kind").Value
                        let changeType = elem.Attribute("item").Value
                        let path = elem.Value
                        select new Diff
                                   {
                                       Kind = kind == "file" ? TargetKind.File : TargetKind.Dir,
                                       Change = _changeTypes[changeType],
                                       FullPath = path,
                                       RelativePath = path.Remove(0, _from.Length)
                                   };

            return new ChangeSet {Diffs = diffs.ToList()};
        }

        private void retrieveLogs()
        {
            ProcessStartInfo startInfo = createRetrieveLogsStartInfo(_from);
            Process process = startProcess(startInfo);
            String data = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            XDocument doc = XDocument.Parse(data);
            var entries = from e in doc.Element("log").Elements()
                          select new
                          {
                              Rev = Int32.Parse(e.Attribute("revision").Value),
                              Message = new LogMessage
                              {
                                  Author = e.Element("author").Value,
                                  Message = e.Element("msg").Value
                              }
                          };

            foreach (var entry in entries)
            {
                _logs[entry.Rev] = entry.Message;
            }
        }

        private void handleButtonClick(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(processDiffs);
        }

        private void handleLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender == FromRepositoryEntry)
            {
                setFromRepository();
            }
            else if (sender == ToRepositoryEntry)
            {
                setToRepository();
            }
        }

        private void setToRepository() 
        {
            if (String.IsNullOrEmpty(ToRepositoryEntry.Text))
            {
                return;
            }

            if (ToRepositoryEntry.Text.Last() != '/')
            {
                ToRepositoryEntry.Text += "/";
            }

            if (String.Equals(_to, ToRepositoryEntry.Text))
            {
                return;
            }

            _to = ToRepositoryEntry.Text;
        }

        private void setFromRepository() 
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

            _fromRev = Int32.Parse(FromRepositoryFromRevEntry.Text);


            String data = runSvn(createInfoStartInfo(_from));

            XDocument doc = XDocument.Parse(data);

            if (String.IsNullOrEmpty(FromRepositoryToRevEntry.Text))
            {
                String rev = (from elem in doc.Descendants()
                              where elem.Name == "entry" && elem.Attribute("revision") != null
                              select elem.Attribute("revision").Value).FirstOrDefault();
                _toRev = Int32.Parse(rev);
            }
            else
            {
                _toRev = Int32.Parse(FromRepositoryToRevEntry.Text);
            }

            FromRepositoryToRevEntry.Text = _toRev.ToString();

            String path = (from elem in doc.Descendants()
                           where elem.Name == "entry" && elem.Attribute("path") != null
                           select elem.Attribute("path").Value).FirstOrDefault();
            _rootDir = Path.Combine(_rootDir, path);
            Directory.CreateDirectory(_rootDir);
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

        private void handleProcessExit(object sender, EventArgs args)
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
                    case RetrieveLogsCommand:
                        {
                            doc = XDocument.Parse(dataBuffer.ToString());
                            var entries = from e in doc.Element("log").Elements()
                                          select new
                                          {
                                              Rev = Int32.Parse(e.Attribute("revision").Value),
                                              Message = new LogMessage
                                              {
                                                  Author = e.Element("author").Value,
                                                  Message = e.Element("msg").Value
                                              }
                                          };

                            foreach (var entry in entries)
                            {
                                _logs[entry.Rev] = entry.Message;
                            }

                            break;
                        }
                    //case InfoCommand:
                    //    {
                    //        doc = XDocument.Parse(dataBuffer.ToString());
                    //        String rev = (from elem in doc.Descendants()
                    //                      where elem.Name == "entry" && elem.Attribute("revision") != null
                    //                      select elem.Attribute("revision").Value).FirstOrDefault();
                    //        _toRev = Int32.Parse(rev);
                    //        String path = (from elem in doc.Descendants()
                    //                       where elem.Name == "entry" && elem.Attribute("path") != null
                    //                       select elem.Attribute("path").Value).FirstOrDefault();
                    //        _rootDir = Path.Combine(_rootDir, path);
                    //        Directory.CreateDirectory(_rootDir);
                    //        FromRepositoryToRevEntry.Dispatcher.BeginInvoke(
                    //            (Action)(() => FromRepositoryToRevEntry.Text = rev));
                    //        break;
                    //    }
                    //case DiffSummaryCommand:
                    //    {
                    //        Int32 rev = Int32.Parse(process.StartInfo.EnvironmentVariables[RevisionKey]);
                    //        doc = XDocument.Parse(dataBuffer.ToString());
                    //        var diffs = from elem in doc.Descendants()
                    //                    where elem.Name == "path"
                    //                    let hasPropChanges = elem.Attribute("props").Value != "none"
                    //                    let kind = elem.Attribute("kind").Value
                    //                    let changeType = elem.Attribute("item").Value
                    //                    let path = elem.Value
                    //                    select new Diff
                    //                               {
                    //                                   Kind = kind == "file" ? TargetKind.File : TargetKind.Dir,
                    //                                   Change = _changeTypes[changeType],
                    //                                   Path = path
                    //                               };

                    //        _changeSets.Add(rev, new ChangeSet { Diffs = diffs.ToList() });

                    //        var startInfo = createDiffStartInfo(_from, rev);
                    //        startProcess(startInfo, true);
                    //        break;
                    //    }
                    //case DiffCommand:
                    //    {
                    //        String data = dataBuffer.ToString();

                    //        if (String.IsNullOrEmpty(data))
                    //        {
                    //            break;
                    //        }

                    //        Int32 rev = Int32.Parse(process.StartInfo.EnvironmentVariables[RevisionKey]);
                    //        var binaryFiles = findBinaryFiles(data);
                    //        ChangeSet changeSet = _changeSets[rev];

                    //        String downloadPath = Path.Combine(_rootDir, rev.ToString());
                    //        Directory.CreateDirectory(downloadPath);

                    //        foreach (var file in binaryFiles)
                    //        {
                    //            String sourceFile = _from + file;
                    //            downloadPath = Path.Combine(downloadPath, Path.GetFileName(file));

                    //            var startInfo = createExportStartInfo(downloadPath, sourceFile, rev);
                    //            startProcess(startInfo, true);

                    //            Diff diff = changeSet.Diffs.Where(d => d.Path == sourceFile).FirstOrDefault();

                    //            if (diff != null)
                    //            {
                    //                diff.BinaryFilePath = downloadPath;
                    //            }
                    //        }

                    //        break;
                    //    }
                    case ExportCommand:
                        {
                            break;
                        }
                    case PatchCommand:
                        {
                            break;
                        }
                    case CopyBinariesCommand:
                        {
                            break;
                        }
                    case CommitCommand:
                        {
                            break;
                        }
                }
            }

            closeProcess(process);
        }

        private static IEnumerable<String> findBinaryFiles(String data)
        {
            MatchCollection matches = _findBinaryFiles.Matches(data);

            foreach (Match match in matches)
            {
                String indexName = match.Groups["IndexName"].Value;
                yield return indexName.Trim();
            }
        }

        private Process startProcess(ProcessStartInfo startInfo)
        {
            Process process = new Process { StartInfo = startInfo };
            process.Start();
            return process;
        }

        private void startProcessAsync(ProcessStartInfo startInfo, Boolean enlistSync)
        {
            Process process = new Process();

            if (enlistSync)
            {
                _sync.Enlist(process);
            }

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
            _sync.Retire(process);
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
        public LogMessage LogMessage { get; set; }
        public IList<Diff> Diffs { get; set; }
        public String UnifiedPatch { get; set; }
    }

    internal class Diff
    {
        public String FullPath { get; set; }
        public String RelativePath { get; set; }
        public TargetKind Kind { get; set; }
        public ChangeType Change { get; set; }
        public String BinaryFilePath { get; set; }
    }

    internal class LogMessage
    {
        public String Message { get; set; }
        public String Author { get; set; }
    }

    internal class ProcessSync
    {
        private readonly Object _enlistSync = new Object();
        private readonly List<WeakReference> _enlistedProcesses = new List<WeakReference>();
        private Int32 _processesRunning;
        private readonly ManualResetEvent _waitEvent = new ManualResetEvent(false);

        public void Enlist(Process process)
        {
            lock (_enlistSync)
            {
                Interlocked.Increment(ref _processesRunning);
                _enlistedProcesses.Add(new WeakReference(process));
            }
        }

        public void Retire(Process process)
        {
            lock (_enlistSync)
            {
                if (!_enlistedProcesses.Exists(w => ReferenceEquals(w.Target, process)))
                {
                    return;
                }

                Int32 running = Interlocked.Decrement(ref _processesRunning);

                if (running == 0)
                {
                    _waitEvent.Set();
                }
            }
        }

        public void WaitForAll()
        {
            _waitEvent.WaitOne();
        }
    }
}
