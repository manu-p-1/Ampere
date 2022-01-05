namespace Ampere.Base
{
    ///<inheritdoc cref="IRangify{T}"/>
    public class Range<T> : IRangify<T> where T : System.IComparable<T>
    {
        ///<inheritdoc cref="IRangify{T}"/>
        public T Minimum { get; set; }

        ///<inheritdoc cref="IRangify{T}"/>
        public T Maximum { get; set; }

        /// <summary>
        /// Creates a new instance of the Range class. This class is mutable - for the immutable
        /// version, see <see cref="ImmutableRange{T}"/>
        /// </summary>
        /// <param name="minimum">The minimum value</param>
        /// <param name="maximum">The maximum value</param>
        protected Range(T minimum, T maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        ///<inheritdoc cref="IRangify{T}"/>
        public override string ToString() => $"[{Minimum} - {Maximum}]";

        ///<inheritdoc cref="IRangify{T}"/>
        public bool IsValid() => Minimum.CompareTo(Maximum) <= 0;

        ///<inheritdoc cref="IRangify{T}"/>
        public bool ContainsValue(T value) => Minimum.CompareTo(value) <= 0 && value.CompareTo(Maximum) <= 0;

        ///<inheritdoc cref="IRangify{T}"/>
        public bool IsInsideRange(IRangify<T> range) =>
            IsValid() && range.IsValid() && range.ContainsValue(Minimum) && range.ContainsValue(Maximum);

        ///<inheritdoc cref="IRangify{T}"/>
        public bool ContainsRange(IRangify<T> range) =>
            IsValid() && range.IsValid() && ContainsValue(range.Minimum) && ContainsValue(range.Maximum);
    }
}