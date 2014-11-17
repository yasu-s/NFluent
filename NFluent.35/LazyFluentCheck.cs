namespace NFluent
{
    using System;
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
        public LazyFluentCheck(T value)
        {
            this.Value = value;
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

        public ICheck<T> Not { get; private set; }

        public ICheckLink<ICheck<T>> IsInstanceOf<TU>()
        {
            if (typeof(T).IsNullable())
            {
                return this.Checker.ExecuteCheck(() => IsInstanceHelper.IsSameType(typeof(T), typeof(TU), this.Value), IsInstanceHelper.BuildErrorMessageForNullable(typeof(T), typeof(TU), this.Value, true));
            }

            return this.Checker.ExecuteCheck(() => IsInstanceHelper.IsInstanceOf(this.Value, typeof(TU)), IsInstanceHelper.BuildErrorMessage(this.Value, typeof(TU), true));
        }

        public ICheckLink<ICheck<T>> IsNotInstanceOf<TU>()
        {
            throw new NotImplementedException();
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

        public LazyChecker<T, ICheck<T>> LazyChecker
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
    }
}