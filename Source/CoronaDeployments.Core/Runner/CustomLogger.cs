using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoronaDeployments.Core.Runner
{
    public class CustomLogger
    {
        private readonly StringBuilder log;

        public CustomLogger()
        {
            log = new StringBuilder();
        }

        public void Error(string m)
        {
            log.AppendFormat($"{DateTime.UtcNow}: Error: {{0}}<br />", m);
            Log.Error(m);
        }

        public void Error(Exception e)
        {
            log.AppendFormat($"{DateTime.UtcNow}: Error: {{0}}<br />", e);
            Log.Error(e, string.Empty);
        }

        public void Information(string m)
        {
            log.AppendFormat($"{DateTime.UtcNow}: Info: {{0}}<br />", m);
            Log.Information(m);
        }

        public void Clear()
        {
            log.Clear();
        }

        public override string ToString()
        {
            return log.ToString();
        }
    }
}
