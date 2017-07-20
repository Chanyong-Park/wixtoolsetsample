using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileWatcher
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Start();
        }

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        string filePath = null;

        private void Start()
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                Log($"arg: [{arg}]");
            }
            System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher();
            watcher.Path = @"d:\watch";
            watcher.NotifyFilter = System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.Size;

            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += Watcher_Renamed;
            watcher.EnableRaisingEvents = true;

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (s, args) =>
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (IsFileLocked(filePath))
                        Log($"File Locked");
                    else
                    {
                        Log($"File unlocked");
                        timer.Stop();

                        // OpenFile > Read File > Check if all files copied
                        // > version comparison > backup working folder
                        // > copy new version to working folder
                    }
                }
            };
        }

        private void Watcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            Log($"{e.ChangeType}, File: {e.FullPath}, Old File: {e.OldFullPath}");
        }

        private void Watcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            Log($"{e.ChangeType}, File: {e.FullPath}");
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            Log($"{e.ChangeType}, File: {e.FullPath}");
            timer.Start();
        }

        private void Watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            filePath = e.FullPath;
            Log($"{e.ChangeType}, File: {filePath}, Length: {new System.IO.FileInfo(filePath).Length}");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Log(string str)
        {
            this.Dispatcher.Invoke(() =>
            {
                txt.Text += str + "\r\n";
            });
        }

        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;
        private bool IsFileLocked(string file)
        {
            //check that problem is not in destination file
            if (File.Exists(file) == true)
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex2)
                {
                    //_log.WriteLog(ex2, "Error in checking whether file is locked " + file);
                    int errorCode = Marshal.GetHRForException(ex2) & ((1 << 16) - 1);
                    if ((ex2 is IOException) && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }
    }
}
