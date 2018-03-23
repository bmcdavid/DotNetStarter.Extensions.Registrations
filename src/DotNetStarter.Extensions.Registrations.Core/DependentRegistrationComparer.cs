using System;
using System.Collections.Generic;

namespace DotNetStarter.Extensions.Registrations.Core
{
    /// <summary>
    /// Sorts registrations, null attributes first
    /// </summary>
    public class DependentRegistrationComparer : IComparer<DependentRegistration>
    {
        /// <summary>
        /// Compares registrations for sorting
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(DependentRegistration x, DependentRegistration y)
        {
            if (x == null || y == null) return 0;
            var xDep = x.Registration?.Dependencies.Length ?? -1;
            var yDep = y.Registration?.Dependencies.Length ?? -1;
            var num = xDep - yDep;

            return num == 0 ?
                string.Compare(x.Implementation.FullName, y.Implementation.FullName, StringComparison.Ordinal) :
                num;
        }
    }
}