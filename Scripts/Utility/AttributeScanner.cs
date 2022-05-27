using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hibzz.Frames
{
	public static class AttributeScanner
	{
		/// <summary>
		/// A binding flag representing all the type of fields
		/// </summary>
		public static readonly BindingFlags BindingFlags_ALL = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

		/// <summary>
		/// A list of assemblies to additionally ignore when scanning for the attribute
		/// </summary>
		public static List<string> IgnoreAssemblyList;

		/// <summary>
		/// Get all methods across all assemblies with the given attribute
		/// </summary>
		/// <typeparam name="T">The attribute to scan</typeparam>
		/// <returns>A list of methods with the given attribute</returns>
		public static List<MethodInfo> GetAllMethods<T>() where T : Attribute
		{
			var methods = new List<MethodInfo>();
			var assemblies = GetValidAssemblies();

			// loop through all valid assemblies, scan for attributes and add it to the return list
			foreach (var assembly in assemblies)
			{
				methods.AddRange(GetAllMethods<T>(assembly));
			}

			return methods;
		}

		/// <summary>
		/// Get all methods with the given attribute from the requested assembly
		/// </summary>
		/// <typeparam name="T">The attribute to scan for</typeparam>
		/// <param name="assemblyName">The name of the assembly to scan in</param>
		/// <returns>A list of methods with the given attribute</returns>
		public static List<MethodInfo> GetAllMethods<T>(string assemblyName) where T : Attribute
		{
			Assembly assembly = Assembly.Load(assemblyName);
			return GetAllMethods<T>(assembly);
		}

		/// <summary>
		/// Get all methods with the given attribute from the requested assembly
		/// </summary>
		/// <typeparam name="T">The attribute to look for</typeparam>
		/// <param name="assembly">The assembly to scan in</param>
		/// <returns>A list of methods with the given attribute</returns>
		public static List<MethodInfo> GetAllMethods<T>(Assembly assembly) where T : Attribute
		{
			// variable used to strore methods with the given attribute
			List<MethodInfo> result = new List<MethodInfo>();

			// Scan through each type available in the assembly
			var types = assembly.GetTypes();
			foreach(var type in types)
			{
				// Get methods of all kind
				var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

				// Get the methods that have the requested attribute and add it to the list
				var methodsWithAttribute = methods.Where((method) => Contains<T>(method));
				result.AddRange(methodsWithAttribute);
			}

			return result;
		}

		/// <summary>
		/// Generic attribute scanner for all members types
		/// </summary>
		/// <typeparam name="T">The attribute type</typeparam>
		/// <param name="assembly">The assembly to scan through</param>
		/// <param name="memberType">The type of member to look through</param>
		/// <returns>A list of members with the requested attribute of type in the given assmebly</returns>
		public static List<MemberInfo> GetAllMembers<T>(Assembly assembly, MemberTypes? memberType = null) where T : Attribute
		{
			// variable used to strore members with the given attribute
			List<MemberInfo> result = new List<MemberInfo>();

			// scan through each type available in the given assembly
			var types = assembly.GetTypes();
			foreach(var type in types)
			{
				// Get members of all kind
				var members = type.GetMembers(BindingFlags_ALL);

				// Get the valid members with requested specs
				IEnumerable<MemberInfo> membersWithAttribute; 
				if(memberType != null)
				{
					// member type is requested
					membersWithAttribute = members.Where((member) => member.MemberType.HasFlag(memberType) && Contains<T>(member));
				}
				else
				{
					// member type is not requested
					membersWithAttribute = members.Where((member) => Contains<T>(member));
				}

				// Now add it to the results
				result.AddRange(membersWithAttribute);
			}

			return result;
		}

		/// <summary>
		/// Check if the given member has the given attribute
		/// </summary>
		/// <typeparam name="T">The type of attribute to check</typeparam>
		/// <param name="member">The member to check</param>
		/// <returns>Does the given member have the given attribute?</returns>
		public static bool Contains<T>(MemberInfo member) where T : Attribute
		{
			var attributes = member.GetCustomAttributes();
			return attributes.Any((attribute) => attribute is T);
		}

		/// <summary>
		/// Get a list of valid assemblies that needs to be scanned through
		/// </summary>
		/// <returns>A list of valid assemblies that can be scanned</returns>
		private static List<Assembly> GetValidAssemblies()
		{
			// Get all assemblies in a list
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

			// Remove native unity engine assemblies if the script is being used in Unity
			#region Remove native unity assemblies
			#if UNITY_5_3_OR_NEWER
			
			assemblies.RemoveAll((assembly) => assembly.GetName().Name.Contains("UnityEngine"));
			assemblies.RemoveAll((assembly) => assembly.GetName().Name.Contains("UnityEditor"));
			assemblies.RemoveAll((assembly) => assembly.GetName().Name.Contains("System"));
			assemblies.RemoveAll((assembly) => assembly.GetName().Name.Contains("mscorlib"));
			assemblies.RemoveAll((assembly) => assembly.GetName().Name.Contains("Mono.Security"));
			assemblies.RemoveAll((assembly) => assembly.GetName().Name.Contains("Bee.BeeDriver"));

			#endif
			#endregion

			// Remove any other ignore assemblies given
			foreach(var ignoreElement in IgnoreAssemblyList)
			{
				assemblies.RemoveAll((assembly) => assembly.GetName().Name.Equals(ignoreElement));
			}

			return assemblies;
		}
	}
}
