using System;
using System.Collections.Generic;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Common.Types
{
    public class DateTimeRange : ValueObject
    {
        public DateTime Start { get; init; }
        public DateTime End { get; init; }

        public DateTimeRange(DateTime start, DateTime end)
        {
            if (start > end)
                throw new ArgumentException($"{nameof(start)} date can't be greater than {nameof(end)} date");

            Start = start;
            End = end;
        }

        public bool IsWithinRange(DateTime date)
        {
            return Start <= date && date <= End;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }

        public override string ToString()
        {
            return $"{Start}÷{End}";
        }

        public string ToString(string format)
        {
            return $"{Start.ToString(format)}÷{End.ToString(format)}";
        }
    }
}
