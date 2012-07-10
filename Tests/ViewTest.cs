using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
	[TestClass]
	public class ViewTest
	{
		public TestContext TestContext { get; set; }

		public enum Layer
		{
			// Order is important!
			Data,
			Domain,
			UI
		}

		[TestMethod]
		public void ValidateLayerUsage()
		{
			var relatedNamespaces = new[] { "PRI.Data", "PRI.Domain", "PRI.FrontEnd", "PRI.ViewModels" };

			var levelMap = new Dictionary<string, Layer>
			{
			               	{relatedNamespaces[0], Layer.Data},
			               	{relatedNamespaces[1], Layer.Domain},
			               	{relatedNamespaces[2], Layer.UI},
			               	{relatedNamespaces[3], Layer.UI},
			               };

			var assemblyFileName = "ClassLibrary.dll";
			ValidateLayerRelationships(levelMap, assemblyFileName);
		}

		private static void ValidateLayerRelationships(Dictionary<string, Layer> levelMap, string assemblyFileName)
		{
			// can't use ReflectionOnlyLoadFrom because we want to peek at attributes
			var groups = from t in Assembly.LoadFrom(assemblyFileName).GetTypes()
			             where levelMap.Keys.Contains(t.Namespace)
			             group t by t.Namespace
			             into g
			             orderby levelMap[g.Key]
			             select g;

			var levelsWithClasses = groups.Count();
			Assert.IsTrue(levelsWithClasses > 1, "Need more than two layers to validate relationships.");

			var errors = new List<string>();
			foreach (var g in groups)
			{
				var layer = levelMap[g.Key];
				// verify this level only accesses things from the adjacent lower layer (or layers)
				var offLimitSubsets = from g1 in groups where !new[] {layer - 1, layer}.Contains(levelMap[g1.Key]) select g1;
				var offLimitTypes = offLimitSubsets.SelectMany(x => x).ToList();
				foreach (Type t in g)
				{
					foreach (MethodInfo m in t.GetAllMethods())
					{
						var methodBody = m.GetMethodBody();
						if (methodBody != null)
							foreach (LocalVariableInfo v in methodBody
								.LocalVariables
								.Where(v => offLimitTypes
								            	.Contains(v.LocalType)))
							{
								errors.Add(
									string.Format(
										"Method \"{0}\" has local variable of type {1} from a layer it shouldn't.",
										m.Name,
										v.LocalType.FullName));
							}
						foreach (ParameterInfo p in m
							.GetParameters()
							.Where(p => offLimitTypes
							            	.Contains(p.ParameterType)))
						{
							errors.Add(
								string.Format(
									"Method \"{0}\" parameter {2} uses parameter type {1} from a layer it shouldn't.",
									m.Name,
									p.ParameterType.FullName,
									p.Name));
						}
						if (offLimitTypes.Contains(m.ReturnType))
						{
							errors.Add(
								string.Format(
									"Method \"{0}\" uses return type {1} from a layer it shouldn't.",
									m.Name,
									m.ReturnType.FullName));
						}
					}
					foreach (PropertyInfo p in t
						.GetAllProperties()
						.Where(p => offLimitTypes.Contains(p.PropertyType)))
					{
						errors.Add(
							string.Format(
								"Type \"{0}\" has a property \"{1}\" of type {2} from a layer it shouldn't.",
								t.FullName,
								p.Name,
								p.PropertyType.FullName));
					}
					foreach(FieldInfo f in t.GetAllFields().Where(f=>offLimitTypes.Contains(f.FieldType)))
					{
						errors.Add(
							string.Format(
								"Type \"{0}\" has a field \"{1}\" of type {2} from a layer it shouldn't.",
								t.FullName,
								f.Name,
								f.FieldType.FullName));
					}
				}
			}
			if (errors.Count > 0)
				Assert.Fail(String.Join(Environment.NewLine, new[] {"Layering violation."}.Concat(errors)));
		}
	}
}
