﻿namespace GLSoft.DoubleEntryHomeAccounting.Common.Exceptions
{
    public class InvalidCurrencyRateException : ApplicationBaseException
    {
        private const string InnerMessage = "Invalid currency rate '{0}'. Rate must be greate then zero. Also i must be 1 for main currency.";

        public decimal Rate { get; }

        public InvalidCurrencyRateException(decimal rate) : this(rate, null)
        {
        }

        public InvalidCurrencyRateException(decimal rate, Exception innerException)
            : base(string.Format(InnerMessage, rate), innerException)
        {
            Rate = rate;
        }
    }
}