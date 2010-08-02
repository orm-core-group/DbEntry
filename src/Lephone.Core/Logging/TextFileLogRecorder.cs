﻿using System;
using System.Text;
using System.IO;
using Lephone.Core.Setting;

namespace Lephone.Core.Logging
{
	public class TextFileLogRecorder : ILogRecorder
	{
        protected object SyncRoot = new object();

		protected string LogFileName;

		public TextFileLogRecorder()
		{
		    string s = CoreSettings.LogFileName;
			Init(s);
		}

        public TextFileLogRecorder(string logFileName)
		{
			Init(logFileName);
		}

		protected void Init(string logFileName)
		{
			if ( logFileName == "" )
			{
				throw new SettingException();
			}
			this.LogFileName = string.Format(logFileName, SystemHelper.BaseDirectory,
                SystemHelper.ExeFileName, SystemHelper.GetDateTimeString());
		}

        public void ProcessLog(SysLogType type, string source, string name, string message, Exception exception)
        {
            lock (SyncRoot)
            {
                using (var sw = new StreamWriter(LogFileName, true, Encoding.Default))
                {
                	WriteLog(sw, type, source, name, message, exception);
                }
            }
        }

        protected virtual void WriteLog(StreamWriter sw, SysLogType type, string source, string name, string message, Exception exception)
        {
            sw.WriteLine("{0},{1},{2},{3},{4},{5}", type, source, name, message, exception, DateTime.Now);
        }
    }
}
