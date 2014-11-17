// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="LazyFluentCheck.cs" company="">
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
    using System.Diagnostics.CodeAnalysis;

    using NFluent.Extensibility;
    using NFluent.Extensions;
    using NFluent.Helpers;

    /// <summary>
    /// Provides lazy fluent check methods to be executed on a given value.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the value to assert on.
    /// </typeparam>
    internal class LazyFluentCheck<T> : IForkableCheck, ICheck<T>, ICheckForExtensibility<T, ICheck<T>>
    {
        private readonly LazyChecker<T, ICheck<T>> checker;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyFluentCheck{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public LazyFluentCheck(T value) : this(value, !CheckContext.DefaulNegated)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyFluentCheck{T}" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="negated">
        /// A boolean value indicating whether the check should be negated or not.
        /// </param>
        public LazyFluentCheck(T value, bool negated)
        {
            this.Value = value;
            this.Negated = negated;
            this.checker = new LazyChecker<T, ICheck<T>>(this);
        }

        #region Explicit Interface Methods

        /// <summary>
        /// Creates a new instance of the same fluent check type, injecting the same Value property
        /// (i.e. the system under test), but with a false Negated property in any case.
        /// </summary>
        /// <returns>
        /// A new instance of the same fluent check type, with the same Value property.
        /// </returns>
        /// <remarks>
        /// This method is used during the chaining of multiple checks.
        /// </remarks>
        object IForkableCheck.ForkInstance()
        {
            this.Negated = !CheckContext.DefaulNegated;
            return this;
        }

        #endregion

        /// <summary>
        /// Negates the next check.
        /// </summary>
        /// <value>
        /// The next check negated.
        /// </value>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:PropertySummaryDocumentationMustMatchAccessors", Justification = "Reviewed. Suppression is OK here since we want to trick and improve the auto-completion experience here.")]
        public ICheck<T> Not
        {
            get
            {
                return new LazyFluentCheck<T>(this.Value, CheckContext.DefaulNegated);
            }
        }


        public ICheckLink<ICheck<T>> IsInstanceOf<TU>()
        {
            if (typeof(T).IsNullable())
            {
                return this.checker.ExecuteCheck(
                    () => IsInstanceHelper.IsSameType(typeof(T), typeof(TU), this.Value),
                    IsInstanceHelper.BuildErrorMessageForNullable(typeof(T), typeof(TU), this.Value, true));
            }

            return this.checker.ExecuteCheck(() => IsInstanceHelper.IsInstanceOf(this.Value, typeof(TU)), IsInstanceHelper.BuildErrorMessage(this.Value, typeof(TU), true));
        }

        public ICheckLink<ICheck<T>> IsNotInstanceOf<TU>()
        {
            if (typeof(T).IsNullable())
            {
                return this.checker.ExecuteCheck(() => IsInstanceHelper.IsDifferentType(typeof(T), typeof(TU), this.Value), IsInstanceHelper.BuildErrorMessageForNullable(typeof(T), typeof(TU), this.Value, false));
            }

            return this.checker.ExecuteCheck(() => IsInstanceHelper.IsNotInstanceOf(this.Value, typeof(TU)), IsInstanceHelper.BuildErrorMessage(this.Value, typeof(TU), false));
        }

        /// <summary>
        /// Gets the value to be tested (provided for any extension method to be able to test it).
        /// </summary>
        /// <value>
        /// The value to be tested by any fluent check extension method.
        /// </value>
        public T Value { get; private set; }

        public IChecker<T, ICheck<T>> Checker
        {
            get
            {
                return this.checker;
            }
        }

        public bool Negated { get; private set; }

        public void Execute()
        {
            this.checker.LazyExecuteForReal();
        }

        /// <summary>
        /// Checks whether the specified <see cref="System.Object"/> is equal to this instance or not.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; throws a <see cref="FluentCheckException"/> otherwise.
        /// </returns>
        /// <exception cref="FluentCheckException">
        /// The specified <see cref="System.Object"/> is not equal to this instance.
        /// </exception>
        public new bool Equals(object obj)
        {
            this.checker.ExecuteCheck(() => EqualityHelper.IsEqualTo(this.checker, obj), EqualityHelper.BuildErrorMessage(this.checker, obj, true));

            return true;
        }
    }
}