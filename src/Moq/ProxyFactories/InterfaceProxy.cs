// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Moq.Internals
{
	/// <summary>Do not use. (Moq requires this class so that <see langword="object"/> methods can be set up on interface mocks.)</summary>
	// NOTE: This type is actually specific to our DynamicProxy implementation of `ProxyFactory`.
	// This type needs to be accessible to the generated interface proxy types because they will inherit from it. If user code
	// is not strong-named, then DynamicProxy will put at least some generated types inside a weak-named assembly. Strong-named
	// assemblies such as Moq's cannot reference weak-named assemblies, so we cannot use `[assembly: InternalsVisibleTo]`
	// to make this type accessible. Therefore we need to declare it as public.
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class InterfaceProxy
	{
		private static MethodInfo equalsMethod = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance);
		private static MethodInfo getHashCodeMethod = typeof(object).GetMethod("GetHashCode", BindingFlags.Public | BindingFlags.Instance);
		private static MethodInfo toStringMethod = typeof(object).GetMethod("ToString", BindingFlags.Public | BindingFlags.Instance);

		/// <summary/>
		[DebuggerHidden]
		public sealed override bool Equals(object obj)
		{
			// Forward this call to the interceptor, so that `object.Equals` can be set up.
			var invocation = new Invocation(equalsMethod, obj);
			((IInterceptor)((IProxy)this).Interceptor).Intercept(invocation);
			return (bool)invocation.ReturnValue;
		}

		/// <summary/>
		[DebuggerHidden]
		public sealed override int GetHashCode()
		{
			// Forward this call to the interceptor, so that `object.GetHashCode` can be set up.
			var invocation = new Invocation(getHashCodeMethod);
			((IInterceptor)((IProxy)this).Interceptor).Intercept(invocation);
			return (int)invocation.ReturnValue;
		}

		/// <summary/>
		[DebuggerHidden]
		public sealed override string ToString()
		{
			// Forward this call to the interceptor, so that `object.ToString` can be set up.
			var invocation = new Invocation(toStringMethod);
			((IInterceptor)((IProxy)this).Interceptor).Intercept(invocation);
			return (string)invocation.ReturnValue;
		}

		private sealed class Invocation : Moq.Invocation
		{
			private static object[] noArguments = new object[0];

			public Invocation(MethodInfo method, params object[] arguments)
				: base(method, arguments)
			{
			}

			public Invocation(MethodInfo method)
				: base(method, noArguments)
			{
			}

			public override void Return() { }

			public override void Return(object value)
			{
				this.SetReturnValue(value);
			}

			public override void ReturnBase() { }
		}
	}
}