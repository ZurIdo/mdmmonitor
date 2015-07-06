using System;
using System.IO;

public static class DashboardLogger
{

    public static void writeToLogFile(string logMessage)
    {
        string strLogMessage = string.Empty;
        string strLogFile = System.Configuration.ConfigurationManager.AppSettings["logFilePath"].ToString();
        StreamWriter swLog;

        strLogMessage = string.Format("{0}: {1}", DateTime.Now, logMessage);

        if (!File.Exists(strLogFile))
        {
            swLog = new StreamWriter(strLogFile);
        }
        else
        {
            swLog = File.AppendText(strLogFile);
        }

        swLog.WriteLine(strLogMessage);
        swLog.WriteLine();

        swLog.Close();
    }
}
