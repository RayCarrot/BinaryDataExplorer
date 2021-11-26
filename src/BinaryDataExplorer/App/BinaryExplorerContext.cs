using BinarySerializer;
using System;
using System.IO;
using System.Text;

namespace BinaryDataExplorer;

public class BinaryExplorerContext : Context
{
    public BinaryExplorerContext(string basePath, bool noLog = false) : base(basePath, null, noLog ? null : new EditorSerializerLog()) { }

    public class EditorSerializerLog : ISerializerLog
    {
        private static bool _hasBeenCreated;
        public bool IsEnabled => Services.App.UserData.Serializer_EnableLog;

        private StreamWriter _logWriter;

        protected StreamWriter LogWriter => _logWriter ??= GetFile();

        public string OverrideLogPath { get; set; }
        public string LogFile => OverrideLogPath ?? Services.App.Path_SerializerLogFile;

        public StreamWriter GetFile()
        {
            var w = new StreamWriter(File.Open(LogFile, _hasBeenCreated ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
            _hasBeenCreated = true;
            return w;
        }

        public void Log(object obj)
        {
            if (IsEnabled)
                LogWriter.WriteLine(obj != null ? obj.ToString() : String.Empty);
        }

        public void Dispose()
        {
            _logWriter?.Dispose();
            _logWriter = null;
        }
    }
}