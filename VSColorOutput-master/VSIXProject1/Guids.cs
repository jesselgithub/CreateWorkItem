// Guids.cs
// MUST match guids.h
using System;

namespace VSIXProject1
{
    static class GuidList
    {
        public const string c_GuidVSPackageTestPkgString = "c762d0a9-9718-42a5-8955-e65aa8c80e10";
        private const string c_GuidVSPackageTestCmdSetString = "91ffe747-d720-4146-9485-9a5ac94e3898";

        public static readonly Guid s_GuidVSPackageTestCmdSet = new Guid(c_GuidVSPackageTestCmdSetString);
    };
}