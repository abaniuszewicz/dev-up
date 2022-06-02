﻿namespace DevUp.Common
{
    public class DateTimeRange
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