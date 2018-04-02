using System;
using DotNetStarter.Abstractions;

namespace DotNetStarter.Extensions.Registrations
{
    /// <summary>
    /// Used for storing attributes with the implementing type
    /// </summary>
    public abstract class AttributeDependentBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="attribute"></param>
        protected AttributeDependentBase(Type implementation, Attribute attribute)
        {
            Implementation = implementation;
            Attribute = attribute;
        }

        /// <summary>
        /// Type with Registration Attribute
        /// </summary>
        public Type Implementation { get; }

        /// <summary>
        /// Startup Attribute
        /// </summary>
        protected virtual Attribute Attribute { get; }
    }

    /// <summary>
    /// Used for storing attributes with the implementing type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AttributeDependentBase<T> : AttributeDependentBase where T : StartupDependencyBaseAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="startupAttribute"></param>
        protected AttributeDependentBase(Type implementation, T startupAttribute) : base(implementation, startupAttribute)
        {
            Attribute = startupAttribute;
        }

        /// <summary>
        /// Startup Attribute
        /// </summary>
        protected new T Attribute { get; }
    }
}