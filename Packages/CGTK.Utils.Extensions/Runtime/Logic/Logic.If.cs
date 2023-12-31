using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace CGTK.Utils.Extensions.Logic
{
	namespace Extensions
	{
		/// <summary>
		/// These are extensions useful for chaining logic in a single line.
		/// Don't use them if performance is important, these are just to make certain parts of your code more readable.
		/// </summary>
		public static partial class Logic
		{
			[PublicAPI]
			[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
			public static void If(this Boolean statement, in Action action)
			{
				if (statement)
				{
					action?.Invoke();
				}
			}
		}
	}
}