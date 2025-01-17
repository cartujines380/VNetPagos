﻿// ReSharper disable once CheckNamespace
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Allows skipping of tracking of columns.
    /// Place this attributer on the entity property which you dont wish to track for audit.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SkipTracking : Attribute
    {
        public bool Enabled { get; set; }
        public SkipTracking(bool skipTracking = true)
        {
            Enabled = skipTracking;
        }

    }
}
