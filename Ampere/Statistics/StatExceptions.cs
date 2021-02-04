using System;

namespace Ampere.Statistics
{

    /// <summary>
    /// An exception that is thrown when the data set is not large enough to compute a statistic.
    /// </summary>
    [Serializable]
    public class InsufficientDataSetException : Exception
    {
        /// <summary>
        /// Creates a new InsufficientDataSetException.
        /// </summary>
        public InsufficientDataSetException()
        { }

        /// <summary>
        /// Creates a new overloaded InsufficientDataSetException containing a message.
        /// </summary>
        /// <param name="message">The message of this exception type</param>
        public InsufficientDataSetException(string message) : base(message) { }

        /// <summary>
        /// Creates a new overloaded InsufficientDataSetException containing a message and an inner Exception.
        /// </summary>
        /// <param name="message">The message of this exception type</param>
        /// <param name="inner">The inner Exception</param>
        public InsufficientDataSetException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Creates a new overloaded InsufficientDataSetException containing a <see cref="System.Runtime.Serialization.SerializationInfo"/> instance
        /// and a <see cref="System.Runtime.Serialization.StreamingContext"/> instance.
        /// </summary>
        /// <param name="info">The SerializationInfo instance</param>
        /// <param name="context">The StreamingContext instance</param>
        protected InsufficientDataSetException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// An exception that is thrown when no Mode exists in the data set.
    /// </summary>
    [Serializable]
    public class NoModeException : Exception
    {
        /// <summary>
        /// Creates a new NoModeException.
        /// </summary>
        public NoModeException()
        { }

        /// <summary>
        /// Creates a new overloaded NoModeException containing a message.
        /// </summary>
        /// <param name="message">The message of this exception type</param>
        public NoModeException(string message) : base(message) { }

        /// <summary>
        /// Creates a new overloaded NoModeException containing a message and an inner Exception.
        /// </summary>
        /// <param name="message">The message of this exception type</param>
        /// <param name="inner">The inner Exception</param>
        public NoModeException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Creates a new overloaded NoModeException containing a <see cref="System.Runtime.Serialization.SerializationInfo"/> instance
        /// and a <see cref="System.Runtime.Serialization.StreamingContext"/> instance.
        /// </summary>
        /// <param name="info">The SerializationInfo instance</param>
        /// <param name="context">The StreamingContext instance</param>
        protected NoModeException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

