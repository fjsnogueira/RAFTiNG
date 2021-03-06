﻿//  --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogReplicationTest.cs" company="Cyrille DUPUYDAUBY">
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

namespace RAFTiNG.Tests.Unit
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using Michonne.Implementation;

    using NFluent;

    using NUnit.Framework;

    using RAFTiNG.Messages;
    using RAFTiNG.Tests.Services;

    [TestFixture]
    public class LogReplicationTest
    {
        private readonly object synchro = new object();

        private object lastMessage;

        [Test]
        public void EmptyLogIsfilledTest()
        {
            using (Helpers.InitLog4Net())
            {
                var middleware = new Middleware();
                var settings = Helpers.BuildNodeSettings("1", new[] { "1", "2" });
                settings.TimeoutInMs = 10;

                using (var leader = new Node<string>(TestHelpers.GetPool().BuildSequencer(), settings, middleware, new StateMachine()))
                {
                    // we inject
                    leader.State.AppendEntries(
                        -1, new[] { new LogEntry<string>("one"), new LogEntry<string>("two"), });

                    middleware.RegisterEndPoint("2", this.OnMessage);
                    lock (this.synchro)
                    {
                        leader.Initialize();
                        const int MaxDelay = 3000;
                        var initIndex = this.WaitForLogSynchro(MaxDelay, middleware, leader);
                        Check.That(initIndex).IsEqualTo(2);
                    }
                }
            }
        }

        [Test]
        public void CommitIndexIsProperlyEstablished()
        {
            using (Helpers.InitLog4Net())
            {
                var middleware = new Middleware();
                var settings = Helpers.BuildNodeSettings("1", new[] { "1", "2" });
                settings.TimeoutInMs = 1;

                using (var leader = new Node<string>(TestHelpers.GetPool().BuildSequencer(), settings, middleware, new StateMachine()))
                {
                    // we inject
                    leader.State.AppendEntries(-1, new[] { new LogEntry<string>("one"), new LogEntry<string>("two"), new LogEntry<string>("3") });

                    middleware.RegisterEndPoint("2", this.OnMessage);
                    lock (this.synchro)
                    {
                        leader.Initialize();
                        const int MaxDelay = 300000;
                        var initIndex = this.WaitForLogSynchro(MaxDelay, middleware, leader);
                        Check.That(initIndex).IsEqualTo(3);

                        // let sometime for the leader to commit entries
                        Thread.Sleep(50);
                        Check.That(leader.LastCommit).IsEqualTo(2);
                    }
                }
            }
        }
        
        private int WaitForLogSynchro(int maxDelay, Middleware middleware, Node<string> leader)
        {
            var initIndex = 0;
            var timer = new Stopwatch();
            timer.Start();
            for (;;)
            {
                var millisecondsTimeout = Math.Max(maxDelay - (int)timer.ElapsedMilliseconds, 0);
                if (Debugger.IsAttached)
                {
                    millisecondsTimeout = Timeout.Infinite;
                }

                if (this.lastMessage == null && !Monitor.Wait(this.synchro, millisecondsTimeout))
                {
                    break;
                }

                var message = this.lastMessage;
                this.lastMessage = null;
                if (message is RequestVote)
                {
                    // we vote for the leader
                    middleware.SendMessage("1", new GrantVote(true, "2", 0));
                }

                if (message is AppendEntries<string>)
                {
                    var appendMessage = message as AppendEntries<string>;
                    if (appendMessage.PrevLogIndex != initIndex - 1)
                    {
                        middleware.SendMessage("1", new AppendEntriesAck("2", 0, false));
                    }
                    else
                    {
                        initIndex += (int)appendMessage.Entries.Count();
                        middleware.SendMessage("1", new AppendEntriesAck("2", 0, true));
                        if (initIndex == leader.State.LogEntries.Count)
                        {
                            break;
                        }
                    }
                }

                if (millisecondsTimeout == 0)
                {
                    break;
                }
            }

            return initIndex;
        }

        private void OnMessage(object obj)
        {
            lock (this.synchro)
            {
                this.lastMessage = obj;
                Monitor.Pulse(this.synchro);
            }
        }
    }
}
