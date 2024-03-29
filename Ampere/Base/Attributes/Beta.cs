﻿using System;

namespace Ampere.Base.Attributes
{
    /// <summary>
    /// The BetaCmdlet attribute represents any Classes, Structs, Methods, Interfaces, or Enums which are functional,
    /// but may result in unintended behavior do to its "beta" state.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class |
                           AttributeTargets.Struct |
                           AttributeTargets.Method |
                           AttributeTargets.Interface |
                           AttributeTargets.Enum)]
    internal class Beta : Attribute
    {
        /// <summary>
        /// The message attributed to this BetaCmdlet, if any.
        /// </summary>
        public string Msg { get; } = string.Empty;

        /// <summary>
        /// The default warning message for this Attribute.
        /// </summary>
        internal const string WarningMessage = "This is a Beta utility - it may cause unexpected behavior";

        /// <summary>
        /// Creates a new BetaCmdlet with no message. 
        /// </summary>
        internal Beta() { }

        /// <summary>
        /// Creates a new BetaCmdlet with the specified message.
        /// </summary>
        /// <param name="msg">A message specifying or representing the state of the cmdlet</param>
        internal Beta(string msg)
        {
            Msg = msg;
        }
    }
}
