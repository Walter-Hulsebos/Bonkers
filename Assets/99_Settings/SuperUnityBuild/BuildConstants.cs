using System;

// This file is auto-generated. Do not modify or move this file.

namespace SuperUnityBuild.Generated
{
    public enum ReleaseType
    {
        None,
        Indev,
    }

    public enum Platform
    {
        None,
        PC,
    }

    public enum ScriptingBackend
    {
        None,
        Mono,
        IL2CPP,
    }

    public enum Architecture
    {
        None,
        Windows_x86,
    }

    public enum Distribution
    {
        None,
    }

    public static class BuildConstants
    {
        public static readonly DateTime buildDate = new DateTime(638324612975601820);
        public const string version = "0.0.0.1";
        public const ReleaseType releaseType = ReleaseType.Indev;
        public const Platform platform = Platform.PC;
        public const ScriptingBackend scriptingBackend = ScriptingBackend.IL2CPP;
        public const Architecture architecture = Architecture.Windows_x86;
        public const Distribution distribution = Distribution.None;
    }
}

