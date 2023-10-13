using System;
using System.IO;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace CGTK.Utils.Extensions
{
	public static class DirectoryInfoExtensions
	{
		[PublicAPI]
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static void RemoveFiles(this DirectoryInfo directory)
		{
			foreach (FileInfo __file in directory.GetFiles())
			{
				__file.Delete();  
			}
		}

		[PublicAPI]
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static void RemoveFiles(this DirectoryInfo directory, in String fileExtensionToRemove)
		{
			if (String.IsNullOrEmpty(value: fileExtensionToRemove))
			{
				throw new ArgumentNullException(paramName: nameof(fileExtensionToRemove));
				//RemoveFiles(directory);
				//return;
			}

			foreach (FileInfo __file in directory.GetFiles())
			{
				if (__file.Extension == fileExtensionToRemove)
				{
					__file.Delete();       
				}
			}
		}
	}
}