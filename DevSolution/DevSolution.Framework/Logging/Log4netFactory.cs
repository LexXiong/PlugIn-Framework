﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using log4net;
using log4net.Config;

namespace DevSolution.Logging
{
    public class Log4netFactory : AbstractLoggerFactory
    {
        private static bool _isFileWatched = false;

        public Log4netFactory() 
            : this(ConfigurationManager.AppSettings["log4net.Config"]) { }

        public Log4netFactory(string configFilename)
        {
            if (!_isFileWatched && !string.IsNullOrWhiteSpace(configFilename))
            {
                // Only monitor configuration file in full trust
                XmlConfigurator.ConfigureAndWatch(GetConfigFile(configFilename));
                _isFileWatched = true;
            }
        }

        public override Castle.Core.Logging.ILogger Create(string name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }

        public override Castle.Core.Logging.ILogger Create(string name)
        {
            return new Log4netLogger(LogManager.GetLogger(name), this);
        }

    }
}