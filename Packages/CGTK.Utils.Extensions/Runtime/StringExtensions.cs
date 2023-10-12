using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace CGTK.Utils.Extensions
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	[PublicAPI]
    public static class StringExtensions
	{
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static Boolean IsNullOrWhiteSpace(this String input) 
			=> String.IsNullOrWhiteSpace(value: input);
		
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static Boolean IsNullOrEmpty(this String input) 
			=> String.IsNullOrEmpty(value: input);
		
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static Boolean NotNullOrWhiteSpace(this String input) 
			=> !String.IsNullOrWhiteSpace(value: input);
		
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static Boolean NotNullOrEmpty(this String input) 
			=> !String.IsNullOrEmpty(value: input);

		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static Boolean StartsWithAny(this String input, params String[] options)
		{
			foreach (String __option in options)
			{
				if (input.StartsWith(value: __option)) return true;
			}

			return false;
		}
		
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static (String matchingPart, String nonMatchingLHS, String nonMatchingRHS) SplitAtDeviation(this String lhs, in String rhs)
		{
			if(lhs.IsNullOrWhiteSpace() || rhs.IsNullOrWhiteSpace()) return (null, lhs, rhs);
            
			String __shortestString = (lhs.Length >= rhs.Length) ? rhs : lhs;

			String 
				__matchingPart = null,
				__nonMatchingA = lhs,
				__nonMatchingB = rhs;
            
			for(Int32 __index = 0; __index < __shortestString.Length; __index++)
			{
				/*
				UnityEngine.Debug.Log(message:  "\n" +
										$"A = {lhs}\n" +
										$"B = {b}\n" +
										$"Index = {__index}\n" +
										$"Char A = {lhs[__index]}\n" +
										$"Char B = {b[__index]}");
										*/
                
				if(lhs[index: __index] == rhs[index: __index]) continue;

				__matchingPart = __shortestString.Substring(startIndex: 0, length: __index);

				__nonMatchingA = lhs.Substring(startIndex: __index, length: lhs.Length-__index);
                    
				__nonMatchingB = rhs.Substring(startIndex: __index, length: rhs.Length-__index);
                    
				break;
			}

			return (__matchingPart, __nonMatchingA, __nonMatchingB);
		}

		public static String ToPathFormatting(this String value) => value.Replace(oldChar: Path.AltDirectorySeparatorChar, newChar: Path.DirectorySeparatorChar);
		
		public static String ToUnityFormatting(this String value) => value.Replace(oldChar: Path.DirectorySeparatorChar, newChar: Path.AltDirectorySeparatorChar);

		/// <summary> Re-bases lhs path to be relative to the "<paramref name="to"></paramref>" path. </summary>
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static String MakeRelative(this String value, String to)
		{
			if (value.IsNullOrEmpty()) throw new ArgumentNullException(paramName: nameof(value));
			if (to.IsNullOrEmpty()) throw new ArgumentNullException(paramName: nameof(to));

			value = value.ToPathFormatting().AppendDirectorySeparator();
			to    = to.ToPathFormatting().AppendDirectorySeparator();

			Debug.Log(message: $"from = {value}, to = {to}");

			Uri __fromURI = new (uriString: value);
			Uri __toURI   = new (uriString: to);
			
			if (__fromURI.Scheme != __toURI.Scheme) return to;

			Uri    __relativeUri  = __fromURI.MakeRelativeUri(uri: __toURI);
			String __relativePath = Uri.UnescapeDataString(stringToUnescape: __relativeUri.ToString());
			
			if (String.Equals(a: __toURI.Scheme, b: Uri.UriSchemeFile, comparisonType: StringComparison.OrdinalIgnoreCase))
			{
				__relativePath = __relativePath.Replace(oldChar: Path.AltDirectorySeparatorChar, newChar:Path.DirectorySeparatorChar);
			}

			return __relativePath;

			//return Uri.UnescapeDataString(stringToUnescape: __relativeTo.MakeRelativeUri(__fullPath).ToString());
			//return Uri.UnescapeDataString(stringToUnescape: __relativeTo.MakeRelativeUri(__fullPath).ToString()).ToPathFormatting();
		}

		public static Boolean IsValidDirectory(this String path) => Directory.Exists(path: path);
		
		public static Boolean NotValidDirectory(this String path) => !Directory.Exists(path: path);
		
		public static String AppendDirectorySeparator(this String path)
		{
			Boolean __isFile       = Path.HasExtension(path: path);
			Boolean __hasSeparator = path.EndsWith(value: Path.DirectorySeparatorChar.ToString()) || path.EndsWith(value: Path.AltDirectorySeparatorChar.ToString());

			if (__isFile || __hasSeparator) return path;
			
			return path + Path.DirectorySeparatorChar;
		}
		
		public static String Remove(this String value, in String text) => value.Replace(oldValue: text, newValue: String.Empty);

		/// <summary> Re-bases lhs path to be relative to the "<paramref name="to"></paramref>" path. </summary>
		[MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
		public static String MakeRelativeTo(this String from, in String to)
		{
			StringBuilder __path = new (capacity: 260); // MAX_PATH
			if(PathRelativePathTo(builder: __path, @from: from, attrFrom: __GetPathAttribute(path: from), to: to, attrTo: __GetPathAttribute(path: to)))
			{
				return __path.ToString();
			}
			
			throw new ArgumentException(message: "Paths must have lhs common prefix");
			
			
			static Int32 __GetPathAttribute(in String path)
			{
				if(IsDirectory(path: path)) return 0x10;
				if(IsFile(path: path))      return 0x80;
				throw new FileNotFoundException();
			}
		}
		
		[DllImport(dllName: "shlwapi.dll", SetLastError = true)]
		private static extern Boolean PathRelativePathTo(StringBuilder builder, String from, Int32 attrFrom, String to, Int32 attrTo);

		public static Boolean IsDirectory(String path)
		{
			DirectoryInfo __dir = new (path: path);
			return __dir.Exists;
		}

		public static Boolean IsFile(String path)
		{
			FileInfo __file = new (fileName: path);
			return __file.Exists;
		}

		public static String RemoveInvalidFileNameCharacters(this String input)
			=> String.Concat(values: input.Split(separator: Path.GetInvalidFileNameChars()));
		
		public static String ReplaceInvalidFileNameCharacters(this String input, in Char replaceWith = '_')
			=> String.Join(separator: replaceWith.ToString(), value: input.Split(separator: Path.GetInvalidFileNameChars()));

		public static String MakeValidScriptName(this String input)
			=> input.RemoveInvalidFileNameCharacters().Replace(oldChar: ' ', newChar: '_');

		/*
		//Stolen from this blog, for learning purposes, but I found it useful.
		public static int Count(this string text, char c)
		{
		    ReadOnlySpan<char> span = text.AsSpan();
		    ref char r0 = ref MemoryMarshal.GetReference(span);
		    int length = span.Length;
		    int i = 0, result;
				
		    if (Vector.IsHardwareAccelerated)
		    {
		        int end = length - Vector<ushort>.Count;

		        // SIMD register all set to 0, to store partial results
		        Vector<ushort> partials = Vector<ushort>.Zero;

		        // SIMD register with the target character c copied in every position
		        Vector<ushort> vc = new Vector<ushort>(c);

		        for (; i <= end; i += Vector<ushort>.Count)
		        {
		            // Get the reference to the current characters chunk
		            ref char ri = ref Unsafe.Add(ref r0, i);

					// vi = { text[i], ..., text[i + Vector<char>.Count - 1] }
		            Vector<ushort> vi = Unsafe.As<char, Vector<ushort>>(ref ri);


		            Vector<ushort> ve = Vector.Equals(vi, vc);


		            Vector<ushort> va = Vector.BitwiseAnd(ve, Vector<ushort>.One);

		            // Accumulate the partial results in each position
		            partials += va;
		        }
				
		        result = Vector.Dot(partials, Vector<ushort>.One);
		    }
		    else result = 0;

		    // Iterate over the remaining characters and count those that match
		    for (; i < length; i++)
		    {
		        bool equals = Unsafe.Add(ref r0, i) == c;
		        result += Unsafe.As<bool, byte>(ref equals);
		    }

		    return result;
		}
		*/
		
		 #region IsValidPath

        private static readonly HashSet<Char> _invalidFilenameChars = new (collection: Path.GetInvalidFileNameChars());

        /// <summary>Checks if the path is a valid Unity path.</summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if the path is a valid Unity path.</returns>
        [PublicAPI] public static Boolean IsValidPath(this String path)
        {
            return path
                .Split(separator: '/')
                .All(predicate: filename => ! filename.Any(
	                                            predicate: character => _invalidFilenameChars.Contains(item: character)));
        }

        #endregion

        #region IsValidIdentifier

        // definition of a valid C# identifier: https://www.programiz.com/csharp-programming/keywords-identifiers
        private const String FormattingCharacter = @"\p{Cf}";
        private const String ConnectingCharacter = @"\p{Pc}";
        private const String DecimalDigitCharacter = @"\p{Nd}";
        private const String CombiningCharacter = @"\p{Mn}|\p{Mc}";
        private const String LetterCharacter = @"\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}";

        private const String IdentifierPartCharacter = LetterCharacter + "|" +
                                                       DecimalDigitCharacter + "|" +
                                                       ConnectingCharacter + "|" +
                                                       CombiningCharacter + "|" +
                                                       FormattingCharacter;

        private const String IdentifierPartCharacters = "(" + IdentifierPartCharacter + ")+";
        private const String IdentifierStartCharacter = "(" + LetterCharacter + "|_)";

        private const String IdentifierOrKeyword = IdentifierStartCharacter + "(" +
                                                   IdentifierPartCharacters + ")*";

        // C# keywords: http://msdn.microsoft.com/en-us/library/x53a06bb(v=vs.71).aspx
        private static readonly HashSet<String> _keywords = new()
        {
            "abstract",  "event",      "new",        "struct",
            "as",        "explicit",   "null",       "switch",
            "base",      "extern",     "object",     "this",
            "bool",      "false",      "operator",   "throw",
            "break",     "finally",    "out",        "true",
            "byte",      "fixed",      "override",   "try",
            "case",      "float",      "params",     "typeof",
            "catch",     "for",        "private",    "uint",
            "char",      "foreach",    "protected",  "ulong",
            "checked",   "goto",       "public",     "unchecked",
            "class",     "if",         "readonly",   "unsafe",
            "const",     "implicit",   "ref",        "ushort",
            "continue",  "in",         "return",     "using",
            "decimal",   "int",        "sbyte",      "virtual",
            "default",   "interface",  "sealed",     "volatile",
            "delegate",  "internal",   "short",      "void",
            "do",        "is",         "sizeof",     "while",
            "double",    "lock",       "stackalloc",
            "else",      "long",       "static",
            "enum",      "namespace",  "string"
        };

        private static readonly Regex _validIdentifierRegex = new (pattern: "^" + IdentifierOrKeyword + "$", options: RegexOptions.Compiled);

        /// <summary>Checks whether a string is a valid identifier (class name, namespace name, etc.)</summary>
        /// <param name="identifier">The string to check.</param>
        /// <returns><see langword="true"/> if the string is a valid identifier.</returns>
        [PublicAPI, Pure]
        public static Boolean IsValidIdentifier(this String identifier)
        {
            return identifier.Contains(value: '.')
                ? identifier.Split(separator: '.').All(predicate: IsValidIdentifierInternal)
                : IsValidIdentifierInternal(identifier: identifier);
        }

        // This is the pure IsValidIdentifier method that does not accept dot-separated identifiers.
        private static Boolean IsValidIdentifierInternal(String identifier)
        {
            if (String.IsNullOrWhiteSpace(value: identifier))
                return false;

            String normalizedIdentifier = identifier.Normalize();

            // 1. check that the identifier matches the valid identifier regex and it's not a C# keyword
            if (_validIdentifierRegex.IsMatch(input: normalizedIdentifier) && ! _keywords.Contains(item: normalizedIdentifier))
                return true;

            // 2. check if the identifier starts with @
            return normalizedIdentifier.StartsWith(value: "@") && _validIdentifierRegex.IsMatch(input: normalizedIdentifier.Substring(startIndex: 1));
        }

        #endregion

        /// <summary>
        /// Returns a substring that follows the last occurence of <paramref name="character"/>.
        /// </summary>
        /// <param name="text">The string to search in.</param>
        /// <param name="character">The char to search for.</param>
        /// <returns>A substring that follows the last occurence of <paramref name="character"/>.</returns>
        [PublicAPI, Pure]
        public static String GetSubstringAfterLast(this String text, Char character)
        {
            Int32 lastCharIndex = text.LastIndexOf(value: character);
            return lastCharIndex == -1 ? text : text.Substring(startIndex: lastCharIndex + 1, length: text.Length - lastCharIndex - 1);
        }

        [PublicAPI, Pure]
        public static String GetSubstringBeforeLast(this String text, Char character)
        {
            Int32 lastCharIndex = text.LastIndexOf(value: character);
            return lastCharIndex == -1 ? text : text.Substring(startIndex: 0, length: lastCharIndex);
        }

        [PublicAPI, Pure]
        public static String GetSubstringBefore(this String text, Char character)
        {
            Int32 charIndex = text.IndexOf(value: character);
            return charIndex == -1 ? text : text.Substring(startIndex: 0, length: charIndex);
        }
        
        [PublicAPI, Pure]
        public static String GetSubstringAfter(this String text, Char character)
        {
            Int32 charIndex = text.IndexOf(value: character);
            return charIndex == -1 ? text : text.Substring(startIndex: charIndex + 1, length: text.Length - charIndex - 1);
        }

        /// <summary>
        /// Counts the number of times <paramref name="substring"/> occured in <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The string to search in.</param>
        /// <param name="substring">The substring to search for.</param>
        /// <returns>The number of times <paramref name="substring"/> occured in <paramref name="text"/>.</returns>
        [PublicAPI, Pure]
        public static Int32 CountSubstrings(this String text, String substring) =>
            (text.Length - text.Replace(oldValue: substring, newValue: String.Empty).Length) / substring.Length;

        public static Int32 CountChars(this String text, Char character)
        {
            Int32 count = 0;
            Int32 textLength = text.Length;

            for (Int32 i = 0; i < textLength; i++)
            {
                if (text[index: i] == character)
                    count++;
            }

            return count;
        }

        [PublicAPI]
        public static Int32 IndexOfNth(this String str, Char chr, Int32 nth = 0)
        {
            if (nth < 0)
                throw new ArgumentException(message: "Can not find a negative index of substring in string. Must start with 0");

            Int32 offset = str.IndexOf(value: chr);
            for (Int32 i = 0; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(value: chr, startIndex: offset + 1);
            }

            return offset;
        }
	}
}
