﻿using Philips.Analogy.LogLoaders;
using Philips.Analogy.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Philips.Analogy.Interfaces.DataTypes;
using Philips.Analogy.Properties;

namespace Philips.Analogy.DataSources
{
    public class EventLogDataProvider : IAnalogyOfflineDataProvider
    {
        public UCLogs LogWindow { get; set; }
        public Guid ID { get; } = new Guid("465F4963-71F3-4E50-8253-FA286BF5692B");
        public void InitDataProvider()
        {

        }

        public bool CanSaveToLogFile { get; }
        public string FileOpenDialogFilters { get; } = "Windows Event log files (*.evtx)|*.evtx";
        public string FileSaveDialogFilters { get; }
        public IEnumerable<string> SupportFormats { get; } = new[] { "*.evtx" };

        public string InitialFolderFullPath { get; } =
            Path.Combine(Environment.ExpandEnvironmentVariables("%SystemRoot%"), "System32", "Winevt", "Logs");
        public Image OptionalOpenFolderImage { get; } = Resources.OperatingSystem_16x16;
        public Image OptionalOpenFilesImage { get; } = Resources.OperatingSystem_16x16;

        public async Task<IEnumerable<AnalogyLogMessage>> Process(string fileName, CancellationToken token, ILogMessageCreatedHandler messagesHandler)
        {
            if (CanOpenFile(fileName))
            {
                EventViewerLogLoader logLoader = new EventViewerLogLoader(token);
                var messages = await logLoader.ReadFromFile(fileName, messagesHandler).ConfigureAwait(false);
                return messages;
            }
            AnalogyLogMessage m = new AnalogyLogMessage();
            m.Text = $"Unsupported file: {fileName}. Skipping file";
            m.Level = AnalogyLogLevel.Critical;
            m.Source = "Analogy";
            m.Module = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            m.ProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;
            m.Class = AnalogyLogClass.General;
            m.User = Environment.UserName;
            m.Date = DateTime.Now;
            messagesHandler.AppendMessage(m, Environment.MachineName);
            return new List<AnalogyLogMessage>() { m };
        }

        public IEnumerable<FileInfo> GetSupportedFiles(DirectoryInfo dirInfo, bool recursiveLoad)
        {
            return GetSupportedFilesInternal(dirInfo, recursiveLoad);
        }

        public Task SaveAsync(List<AnalogyLogMessage> messages, string fileName)
        {
            throw new NotSupportedException("Saving is not supported for evtx files");
        }

        public bool CanOpenFile(string fileName)=> fileName.EndsWith(".evtx", StringComparison.InvariantCultureIgnoreCase);
        public bool CanOpenAllFiles(IEnumerable<string> fileNames) => fileNames.All(CanOpenFile);

        public static List<FileInfo> GetSupportedFilesInternal(DirectoryInfo dirInfo, bool recursive)
        {
            List<FileInfo> files = dirInfo.GetFiles("*.evtx").ToList();
            if (!recursive)
                return files;
            try
            {
                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    files.AddRange(GetSupportedFilesInternal(dir, true));
                }
            }
            catch (Exception)
            {
                return files;
            }

            return files;
        }

    }
}