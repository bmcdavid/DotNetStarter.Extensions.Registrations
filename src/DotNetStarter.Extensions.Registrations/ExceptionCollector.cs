using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Access to any exceptions during external dependency registrations
    /// </summary>
    public static class ExceptionCollector
    {
        private static readonly List<Exception> _exceptions = new List<Exception>();

        /// <summary>
        /// Adds exception
        /// </summary>
        /// <param name="e"></param>
        public static void AddException(Exception e)
        {
            _exceptions.Add(e);
        }

        /// <summary>
        /// Exception list
        /// </summary>
        /// <returns></returns>
        public static ICollection<Exception> GetExceptions() => new ReadOnlyCollection<Exception>(_exceptions);
    }
}
