// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="LazyChecks.cs" company="">
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
namespace NFluent
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using NFluent.Extensibility;

    /// <summary>
    /// Allows to add lazy fluent checks to execute them afterward all in once via an <see cref="Execute"/> method.
    /// </summary>
    public class LazyChecks
    {
        private readonly List<Action> lazyChecksLambdas;

        // private readonly LazyCheckerAggregator lazyCheckerAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyChecks"/> class.
        /// </summary>
        public LazyChecks()
        {
            this.lazyChecksLambdas = new List<Action>();
            // this.lazyCheckerAggregator = new LazyCheckerAggregator();
        }

        /// <summary>
        /// Returns a <see cref="ICheck{T}" /> instance that will provide check methods to be executed on a given value.
        /// </summary>
        /// <typeparam name="T">Type of the value to be tested.</typeparam>
        /// <param name="value">The value to be tested.</param>
        /// <returns>
        /// A <see cref="ICheck{T}" /> instance to use in order to check things on the given value.
        /// </returns>
        /// <remarks>
        /// Every method of the returned <see cref="ICheck{T}" /> instance will throw a <see cref="FluentCheckException" /> when failing.
        /// </remarks>
        public ICheck<T> That<T>(T value)
        {
            //Action newCheck = () => Check.That(value);
            var lazyCheck = new LazyFluentCheck<T>(value);
            //this.lazyCheckerAggregator.Add(lazyCheck.Checker);
            
            this.lazyChecksLambdas.Add(lazyCheck.Execute);

            return lazyCheck;
        }

        /// <summary>
        /// Executes all the lazy checks added through the <see cref="That{T}"/> method.
        /// </summary>
        /// <exception cref="FluentCheckException">At least one of the lazy check registered has raised a <see cref="FluentCheckException"/>.</exception>
        public void Execute()
        {
            var fluentExceptions = new List<FluentCheckException>();

            foreach (var lazyCheckLambda in this.lazyChecksLambdas)
            {
                try
                {
                    lazyCheckLambda();
                }
                catch (FluentCheckException fcex)
                {
                    fluentExceptions.Add(fcex);
                }
            }

            if (fluentExceptions.Count > 0)
            {
                var exceptionStatus = new StringBuilder(fluentExceptions.Count.ToString());
                if (fluentExceptions.Count > 1)
                {
                    exceptionStatus.Append(" lazy checks failed:\n-----------");
                }
                else
                {
                    exceptionStatus.Append(" lazy check failed:\n-----------");
                }

                foreach (var fluentCheckException in fluentExceptions)
                {
                    exceptionStatus.Append(fluentCheckException.Message);
                    exceptionStatus.Append("\n-----------");
                }
                throw new FluentCheckException(exceptionStatus.ToString());    
            }
        }
    }

    ///// <summary>
    ///// Aggregates <see cref="LazyChecker{T,TC}"/> instances to control them all.
    ///// </summary>
    //internal class LazyCheckerAggregator
    //{
    //    private readonly List<LazyChecker<object, ICheck<object>>> lazyCheckers = new List<LazyChecker<object, ICheck<object>>>();

    //    public void Add(LazyChecker<object, ICheck<object>> checker)
    //    {
    //        this.lazyCheckers.Add(checker);
    //    }
    //}
}