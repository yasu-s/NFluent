// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="LazyChecksTests.cs" company="">
// //   Copyright 2014 Thomas PIERRAIN
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace NFluent.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class LazyChecksTests
    {
        [Test]
        public void ShouldExecuteAllRegisteredChecksInOnce()
        {
            var bienOuBien = true;
            var lazyChecks = new LazyChecks();

            lazyChecks.That(bienOuBien).IsTrue();
            lazyChecks.That('c').IsALetter();

            lazyChecks.Execute();
        }

        [Test]
        [ExpectedException(typeof(FluentCheckException), ExpectedMessage = "2 lazy checks failed:\n-----------\nThe checked boolean is true whereas it must be false.\nThe checked boolean:\n\t[True]\n-----------\nThe checked char is not a letter.\nThe checked char:\n\t['.']\n-----------")]
        public void ShouldRaiseAllFailuresInOneShotAtTheEnd()
        {
            var bienOuBien = true;
            var lazyChecks = new LazyChecks();

            lazyChecks.That(bienOuBien).IsFalse();
            lazyChecks.That('.').IsALetter();

            lazyChecks.Execute();
        }

        [Test]
        public void ShouldExecuteAllRegisteredChecksInOnceEvenWithNotOperatorInvolved()
        {
            var bienOuBien = true;
            var lazyChecks = new LazyChecks();

            lazyChecks.That(bienOuBien).Not.IsFalse();
            lazyChecks.That('.').Not.IsALetter();
            
            lazyChecks.Execute();
        }


        [Test]
        [ExpectedException(typeof(FluentCheckException))]
        public void ShouldThrowIfARegisteredCheckFailedWithNotOperatorInvolved()
        {
            var lazyChecks = new LazyChecks();

            lazyChecks.That('C').Not.IsALetter();

            lazyChecks.Execute();
        }

        [Test]
        public void ShouldNotWorkProperlyWhenAtLeastOneLazyCheckIsImplementedWithoutChecker()
        {
            var hero = new Person() { Age = 40, Name = "Thomas", Nationality = Nationality.American };
            var bienOuBien = true;
            var lazyChecks = new LazyChecks();
           
            lazyChecks.That(bienOuBien).IsTrue();
            lazyChecks.That('c').IsALetter();
            lazyChecks.That(hero.Name).Contains("oma");
            lazyChecks.That(hero).IsInstanceOf<Person>();
            // lazyChecks.That(hero).IsNotNull().And.Not.IsInstanceOf<Person>();

            lazyChecks.Execute();
        }

        //// Another draft from Cyrille:
        ////var mine = Contract.CreateOn<int>();

        ////mine.AddRule().IsPositive();
        ////mine.AddRule().IsOdd();

        ////mine.CheckThat(4);



        ////var mine = Check.CreateComplex();

        ////mine.CheckThat(x).
        ////mine.CheckThat(y)...
    }
}
