﻿//  --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="Cyrille DUPUYDAUBY">
//   Copyright 2013 Cyrille DUPUYDAUBY
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//  --------------------------------------------------------------------------------------------------------------------
namespace RAFTiNG.Tests
{
    using System.Collections.Generic;

    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;

    internal static class Helpers
    {
        public static NodeSettings BuildNodeSettings(string nodeId, IEnumerable<string> nodes)
        {
            List<string> workNodes;
            if (nodes != null)
            {
                workNodes = new List<string>(nodes);
                if (workNodes.Contains(nodeId))
                {
                    workNodes.Remove(nodeId);
                }
            }
            else
            {
                workNodes = new List<string>();
            }

            var settings = new NodeSettings
            {
                NodeId = nodeId,
                TimeoutInMs = 10,
                Nodes = workNodes.ToArray()
            };
            return settings;
        }

        internal static void InitLog4Net()
        {
            var appender = new ConsoleAppender();
            appender.Layout =
                new PatternLayout("%date{HH:mm:ss,fff} %-5level - %message (%logger) [%thread]%newline");
            appender.Threshold = Level.Trace;
            appender.ActivateOptions();

            // Configure the root logger.
            var h = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            var rootLogger = h.Root;
            rootLogger.Level = h.LevelMap["TRACE"];
            BasicConfigurator.Configure(appender);
        }
    }
}