// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel
{
    /// <summary>
    ///    <para>Specifies the display name for a property or event.  The default is the name of the property or event.</para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Method)]
    public class DisplayNameAttribute : Attribute, IIsDefaultAttribute
    {
        /// <summary>
        /// <para>Specifies the default value for the <see cref='System.ComponentModel.DisplayNameAttribute'/> , which is an
        ///    empty string (""). This <see langword='static'/> field is read-only.</para>
        /// </summary>
        public static readonly DisplayNameAttribute Default = new DisplayNameAttribute();
        private string _displayName;

        /// <summary>
        ///    <para>[To be supplied.]</para>
        /// </summary>
        public DisplayNameAttribute() : this(string.Empty)
        {
        }

        /// <summary>
        ///    <para>Initializes a new instance of the <see cref='System.ComponentModel.DisplayNameAttribute'/> class.</para>
        /// </summary>
        public DisplayNameAttribute(string displayName)
        {
            _displayName = displayName;
        }

        /// <summary>
        ///    <para>Gets the description stored in this attribute.</para>
        /// </summary>
        public virtual string DisplayName
        {
            get
            {
                return DisplayNameValue;
            }
        }

        /// <summary>
        ///     Read/Write property that directly modifies the string stored
        ///     in the description attribute. The default implementation
        ///     of the Description property simply returns this value.
        /// </summary>
        protected string DisplayNameValue
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            DisplayNameAttribute other = obj as DisplayNameAttribute;

            return (other != null) && other.DisplayName == DisplayName;
        }

        public override int GetHashCode()
        {
            return DisplayName.GetHashCode();
        }

        /// <internalonly/>
        /// <summary>
        /// </summary>
        bool IIsDefaultAttribute.IsDefaultAttribute()
        {
            return this.Equals(Default);
        }
    }
}
