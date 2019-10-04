using System;

namespace StratisRpc.Performance
{
    public class UnitOfTimeFormatter
    {
        public enum TimeUnit
        {
            Second = 0,
            Millisecond = 1
        }

        public static UnitOfTimeFormatter Default = new UnitOfTimeFormatter();

        // the format to apply
        public TimeUnit Unit { get; set; }


        /// <summary>
        /// The time format to apply when formatting times
        /// </summary>
        private string timeFormat;

        private int decimals;
        /// <summary>
        /// Gets or sets the number of decimals to show when formatting times.
        /// </summary>
        public int Decimals
        {
            get { return decimals; }
            set
            {
                decimals = value;
                string unit = this.Unit == TimeUnit.Millisecond ? "ms" : "sec";
                this.timeFormat = value == 0 ? $"0  {unit}" : $"0.{"0".PadLeft(value, '0')} {unit}";
            }
        }


        public UnitOfTimeFormatter()
        {
            this.Unit = TimeUnit.Millisecond;
            this.Decimals = 0;
        }

        public string Format(TimeSpan value)
        {
            switch (this.Unit)
            {
                case TimeUnit.Second:
                    return value.TotalSeconds.ToString(this.timeFormat);
                case TimeUnit.Millisecond:
                    return value.TotalMilliseconds.ToString(this.timeFormat);
                default:
                    throw new ArgumentException($"Cannot format unknown TimeUnit {value}");
            }
        }
    }
}
