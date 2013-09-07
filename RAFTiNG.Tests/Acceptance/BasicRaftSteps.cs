﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicRaftSteps.cs" company="Cyrille DUPUYDAUBY">
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
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RAFTiNG.Tests
{
    using System.Threading;

    using TechTalk.SpecFlow;

    [Binding]
    public class BasicRaftSteps
    {
        [Given(@"I have deployed (.*) instances")]
        public void GivenIHaveDeployedInstances(int p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I start instances (.*), (.*) and (.*)")]
        public void WhenIStartInstancesAnd(int p0, int p1, int p2)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"there is (.*) leader")]
        public void ThenThereIsLeader(int p0)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I wait (.*) seconde")]
        public void WhenIWaitSeconde(int p0)
        {
            Thread.Sleep(p0 * 1000);
        }
    }
}