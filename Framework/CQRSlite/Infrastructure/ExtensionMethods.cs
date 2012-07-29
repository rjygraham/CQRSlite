using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CQRSlite.Infrastructure
{

    public static class ExtensionMethods
    {

        /// <summary>
        /// Extension method for safely retrieving all loadable types in an assembly.
        /// </summary>
        /// <remarks>Taken from Phil Haack's post: Get All Types in an Assembly
        /// http://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx</remarks>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }

}