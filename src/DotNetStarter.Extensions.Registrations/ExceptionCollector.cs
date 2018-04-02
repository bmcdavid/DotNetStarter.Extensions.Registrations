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
        private static readonly List<Exception> Exceptions = new List<Exception>();

        /// <summary>
        /// Adds exception
        /// </summary>
        /// <param name="e"></param>
        public static void AddException(Exception e)
        {
            Exceptions.Add(e);
        }

        /// <summary>
        /// Exception list
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyCollection<Exception> GetExceptions() => new ReadOnlyCollection<Exception>(Exceptions);
    }
}
