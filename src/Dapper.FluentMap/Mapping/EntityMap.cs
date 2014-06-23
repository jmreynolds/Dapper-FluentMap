﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.FluentMap.Utils;

namespace Dapper.FluentMap.Mapping
{
    /// <summary>
    /// Represents a non-typed mapping of an entity.
    /// This class supports the internal infrastructure and should not be used directly in code.
    /// </summary>
    public abstract class EntityMap
    {
        /// <remarks>
        /// The constructor is internal so classes outside this assembly can't derive from it.
        /// </remarks>
        internal EntityMap()
        {
            PropertyMaps = new List<PropertyMap>();
        }

        /// <summary>
        /// Gets the collection of mapped properties.
        /// </summary>
        internal IList<PropertyMap> PropertyMaps { get; private set; }
    }

    /// <summary>
    /// Represents a typed mapping of an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to configure the mapping for.</typeparam>
    public abstract class EntityMap<TEntity> : EntityMap
        where TEntity : class
    {
        /// <summary>
        /// Returns an instance of <see cref="T:Dapper.FluentMap.Mapping.PropertyMap"/> which can perform custom mapping
        /// for the specified property on <typeparamref name="TEntity"/>.
        /// </summary>
        /// <param name="expression">Expression to the property on <typeparamref name="TEntity"/>.</param>
        /// <returns>The created <see cref="T:Dapper.FluentMap.Mapping.PropertyMap"/> instance. This enables a fluent API.</returns>
        /// <exception cref="T:System.Exception">when a duplicate mapping is provided.</exception>
        protected PropertyMap Map(Expression<Func<TEntity, object>> expression)
        {
            var info = (PropertyInfo)ReflectionHelper.GetMemberInfo(expression);

            var propertyMap = new PropertyMap(info);

            ThrowIfDuplicateMapping(propertyMap);

            PropertyMaps.Add(propertyMap);
            return propertyMap;
        }

        private void ThrowIfDuplicateMapping(PropertyMap map)
        {
            if (PropertyMaps.Any(p => p.PropertyInfo.Name == map.PropertyInfo.Name))
            {
                throw new Exception(string.Format("Duplicate mapping. Property '{0}' is already mapped to column '{1}'.", map.PropertyInfo.Name, map.ColumnName));
            }
        }
    }
}