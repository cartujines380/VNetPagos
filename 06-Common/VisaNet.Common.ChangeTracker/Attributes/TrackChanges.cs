﻿// ReSharper disable once CheckNamespace
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Enables tracking of Entity tables.
    /// Place this attribute on a entity class which you want to track for audit.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TrackChanges : Attribute
    {
        public bool Enabled { get; set; }

        public TrackChanges(bool trackChnages = true)
        {
            Enabled = trackChnages;
        }
    }
}
