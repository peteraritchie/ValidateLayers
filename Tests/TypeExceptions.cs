using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tests
{
	public static class TypeExceptions
	{
		public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return
				type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public).Where(
					m => !m.GetCustomAttributes(true).Any(a => a is CompilerGeneratedAttribute));
		}
		public static IEnumerable<FieldInfo> GetAllFields(this Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public)
				.Where(f => !f.GetCustomAttributes(true).Any(a => a is CompilerGeneratedAttribute));
		}
		public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return
				type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public).Where
					(p => !p.GetCustomAttributes(true).Any(a => a is CompilerGeneratedAttribute));
		}
	}
}