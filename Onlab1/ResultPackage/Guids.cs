// Guids.cs
// MUST match guids.h
using System;

namespace Company.ResultPackage
{
    static class GuidList
    {
        public const string guidResultPackagePkgString = "c6626c8b-1e61-4f5d-931d-5ec710577046";
        public const string guidResultPackageCmdSetString = "653aaf19-87b6-435e-82bf-40058c0456c7";
        public const string guidToolWindowPersistanceString = "06ee94df-462a-410e-a307-d96c56052213";

        public static readonly Guid guidResultPackageCmdSet = new Guid(guidResultPackageCmdSetString);
    };
}