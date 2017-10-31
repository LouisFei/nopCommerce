using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface provide information about types 
    /// to various services in the Nop engine.
    /// 
    /// 类型查找抽象接口
    /// </summary>
    public interface ITypeFinder
    {
        /// <summary>
        /// 获得所有程序集
        /// </summary>
        /// <returns></returns>
        IList<Assembly> GetAssemblies();

        /// <summary>
        /// 获得
        /// </summary>
        /// <param name="assignTypeFrom"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns></returns>
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }
}
