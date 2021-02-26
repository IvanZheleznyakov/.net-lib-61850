using System;
using System.IO;

namespace lib61850net
{
    internal class Logger
    {
        internal enum Severity
        {
            Debug,
            Information,
            Warning,
            Error
        }

        string tmpFile;
        Stream stream;
        TextWriter writer;
        Severity verbosity;

        static Logger sLog;
        internal static Logger getLogger()
        {
            if (sLog == null)
                new Logger();
            return sLog;
        }

        internal Logger()
        {
            verbosity = Severity.Information;
            //verbosity = Severity.Debug;
            try
            {
                tmpFile = /*Path.Combine(Path.GetTempPath(),*/ "MMS_log_file.txt";
                stream = new FileStream(tmpFile, FileMode.Create, FileAccess.Write, FileShare.Read);
                //  writer = new StreamWriter(stream);
                writer = Console.Out;
                sLog = this;
            }
            catch { }
        }

        ~Logger()
        {
            if (stream != null) stream.Close();
        }

        internal virtual void Log(Severity severity, string message)
        {
            //Console.WriteLine(string.Format("{0}: {1}", severity.ToString(), message));

            //try
            //{
            //    string msg = string.Format("[{0}.{1}] {2}: {3}", DateTime.Now, DateTime.Now.Millisecond.ToString("D3"), severity.ToString(), message);
            //    writer.WriteLine(msg);
            //    writer.Flush();
            //    if (OnLogMessage != null)
            //        OnLogMessage(msg); //, null, null);
            //}
            //catch { }
        }

        internal void LogDebug(string message)
        {
            if (verbosity == Severity.Debug)
                Log(Severity.Debug, message);
        }

        internal void LogDebugBuffer(string message, byte[] buffer, long logFrom, long logLength)
        {
            //if (verbosity == Severity.Debug)
            //{
            //    string s = message + " (Len=" + logLength + ")>";
            //    for (long i = logFrom; i < logFrom + logLength; i++)
            //        s += String.Format("{0:x2} ", buffer[i]);
            //    Log(Severity.Debug, s);
            //}
        }

        internal void LogReport (string rptdVarQualityLog, string rptdVarTimestampLog, string rptdVarPathLog, string rptdVarDescriptionLog, string rptdVarValueLog)
        {
            if (OnLogReport != null)
                OnLogReport(rptdVarQualityLog, rptdVarTimestampLog, rptdVarPathLog, rptdVarDescriptionLog, rptdVarValueLog);
        }

        internal void LogInfo(string message)
        {
            if (verbosity <= Severity.Information)
                Log(Severity.Information, message);
        }

        internal void LogWarning(string message)
        {
            if (verbosity <= Severity.Warning)
                Log(Severity.Warning, message);
        }

        internal void LogError(string message)
        {
            Log(Severity.Error, message);
        }

         internal void ClearLog()
        {
            if (OnClearLog != null)
                OnClearLog();
        }

       internal Severity Verbosity
        {
            get { return verbosity; }
            set { verbosity = value; Log(Severity.Information, "Verbosity selected: " + verbosity.ToString()); }
        }

        internal delegate void OnLogMessageDelegate(string message);
        internal event OnLogMessageDelegate OnLogMessage;

        internal delegate void OnClearLogDelegate();
        internal event OnClearLogDelegate OnClearLog;

        internal delegate void OnLogReportDelegate (string rptdVarQualityLog, string rptdVarTimestampLog, string rptdVarPathLogstring, string rptdVarDescriptionLog, string rptdVarValueLog);
        internal event OnLogReportDelegate OnLogReport;

    }
}
