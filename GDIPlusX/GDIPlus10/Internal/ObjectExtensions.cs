//////////////////////////////////////////////////////////////////////////////////
//	GDI+ Extensions
//	Written by Aaron Lee Murgatroyd (http://home.exetel.com.au/amurgshere/)
//	A CodePlex project (http://csharpgdiplus11.codeplex.com/)
//  Released under the Microsoft Public License (Ms-PL) .
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;

namespace GDIPlusX.GDIPlus10.Internal
{
    /// <summary>
    /// Provides internal extensions for the Object class.
    /// </summary>
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Gets the value of a non-public property value for an Object.
        /// </summary>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <param name="obj">The Object to get the property value for.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The get value of the property.</returns>
        /// <exception cref="System.InvalidOperationException">Property could not be located.</exception>
        public static TResult GetPrivateProperty<TResult>(this object obj, string propertyName)
        {
            if (obj == null) return default(TResult);

            Type ltType = obj.GetType();

            PropertyInfo lpiPropInfo =
                ltType.GetProperty(
                    propertyName,
                    System.Reflection.BindingFlags.GetProperty |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);

            if (lpiPropInfo != null)
                return (TResult)lpiPropInfo.GetValue(obj, null);
            else
                throw new InvalidOperationException(
                    string.Format(
                        "Instance property '{0}' could not be located in object of type '{1}'.",
                        propertyName, obj.GetType().FullName)); 
        }

        /// <summary>
        /// Gets the value of a static property value for a type.
        /// </summary>
        /// <typeparam name="TResult">The type of the property.</typeparam>
        /// <param name="type">The type to get the static property value for.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The get value of the property.</returns>
        /// <exception cref="System.InvalidOperationException">Property could not be located.</exception>
        internal static TResult GetStaticProperty<TResult>(this Type type, string propertyName)
        {
            if (type == null) return default(TResult);

            PropertyInfo lpiPropInfo =
                type.GetProperty(
                    propertyName,
                    BindingFlags.GetProperty |
                    BindingFlags.Static  |
                    BindingFlags.NonPublic | 
                    BindingFlags.Public | 
                    BindingFlags.FlattenHierarchy);

            if (lpiPropInfo != null)
                return (TResult)lpiPropInfo.GetValue(type, null);
            else
                throw new InvalidOperationException(
                    string.Format(
                        "Static property '{0}' could not be located in type '{1}'.",
                        propertyName, type.FullName));
        }

        /// <summary>
        /// Gets the value of a non-public field value for an Object.
        /// </summary>
        /// <typeparam name="TResult">The type of the field.</typeparam>
        /// <param name="obj">The Object to get the field value for.</param>
        /// <param name="fieldName">The name of the property.</param>
        /// <returns>The value for the field.</returns>
        /// <exception cref="System.InvalidOperationException">Field could not be located.</exception>
        internal static TResult GetPrivateField<TResult>(this object obj, string fieldName)
        {
            if (obj == null) return default(TResult);

            Type ltType = obj.GetType();

            FieldInfo lfiFieldInfo =
                ltType.GetField(
                    fieldName,
                    System.Reflection.BindingFlags.GetField |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);

            if (lfiFieldInfo != null)
                return (TResult)lfiFieldInfo.GetValue(obj);
            else
                throw new InvalidOperationException(
                    string.Format(
                        "Instance field '{0}' could not be located in object of type '{1}'.",
                        fieldName, obj.GetType().FullName));
        }

        /// <summary>
        /// Invokes a non-public static method for an Object.
        /// </summary>
        /// <typeparam name="TResult">The return type of the static method.</typeparam>
        /// <param name="obj">The Object to invoke the static method for.</param>
        /// <param name="methodName">The name of the static method.</param>
        /// <param name="args">The arguments for the static method.</param>
        /// <returns>The return value of the static method.</returns>
        /// <exception cref="System.InvalidOperationException">Static method could not be located.</exception>
        internal static TResult InvokeStaticPrivateMethod<TResult>(this object obj, string methodName, params object[] args)
        {
            return obj.GetType().InvokeStaticPrivateMethod<TResult>(methodName, args);
        }
    }
}
