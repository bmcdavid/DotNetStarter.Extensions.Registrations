using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Default dependency sorter
    /// </summary>
    public class RegistrationSorter : IRegistrationSorter
    {
        private readonly IComparer<DependentRegistration> _dependencyComparer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dependencyComparer"></param>
        public RegistrationSorter(IComparer<DependentRegistration> dependencyComparer = null)
        {
            _dependencyComparer = dependencyComparer ?? new DependentRegistrationComparer();
        }

        /// <summary>
        /// Default sorter
        /// </summary>
        /// <param name="dependentRegistrations"></param>
        /// <returns></returns>
        public virtual void Sort(ICollection<DependentRegistration> dependentRegistrations)
        {
            var unresolved = dependentRegistrations.OrderBy(x => x, _dependencyComparer)
                .SkipWhile(x => x.Registration == null)
                .ToList();

            var resolved = new List<DependentRegistration>();
            var hashSet = new HashSet<object>();
            var count = unresolved.Count;
            var index = 0;

            while (unresolved.Count > 0)
            {
                var dependentRegistration = unresolved[index];

                if (hashSet.IsSupersetOf(dependentRegistration.Registration.Dependencies))
                {
                    resolved.Add(dependentRegistration);
                    hashSet.Add(dependentRegistration.Implementation);
                    unresolved.RemoveAt(index--);
                }

                if (++index < unresolved.Count) continue;

                if (count == unresolved.Count)
                {
                    var names = string.Join(Environment.NewLine, unresolved.Select(x => x.Implementation.FullName));
                    throw new InvalidOperationException($"Cannot resolve registrations for the following: {names}, please check their dependencies!");
                }

                index = 0;
                count = unresolved.Count;
            }

            // update registrations to sorted values
            dependentRegistrations.Clear();

            foreach (var r in resolved)
            {
                dependentRegistrations.Add(r);
            }
        }        
    }
}