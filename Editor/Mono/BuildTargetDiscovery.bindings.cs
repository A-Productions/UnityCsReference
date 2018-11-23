// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEditor.Build;
using System.Runtime.InteropServices;
using GraphicsDeviceType = UnityEngine.Rendering.GraphicsDeviceType;

namespace UnityEditor
{
    [StaticAccessor("BuildTargetDiscovery::GetInstance()", StaticAccessorType.Dot)]
    [NativeHeader("Editor/Src/BuildPipeline/BuildTargetDiscovery.h")]
    internal static class BuildTargetDiscovery
    {
        const int kShortNameIndex = 0;

        [Flags]
        public enum TargetAttributes
        {
            None                            = 0,
            IsDeprecated                    = (1 << 0),
            IsMobile                        = (1 << 1),
            IsConsole                       = (1 << 2),
            IsX64                           = (1 << 3),
            IsStandalonePlatform            = (1 << 4),
            DynamicBatchingDisabled         = (1 << 5),
            CompressedGPUSkinningDisabled   = (1 << 6),
            UseForsythOptimizedMeshData     = (1 << 7),
            DisableEnlighten                = (1 << 8),
            ReflectionEmitDisabled          = (1 << 9),
            OSFontsDisabled                 = (1 << 10),
            NoDefaultUnityFonts             = (1 << 11),
            SupportsFacebook                = (1 << 12),
            WarnForExpensiveQualitySettings = (1 << 13),
            WarnForMouseEvents              = (1 << 14),
            HideInUI                        = (1 << 15),
            GPUSkinningNotSupported         = (1 << 16),
            StrippingNotSupported           = (1 << 17),
            Il2CPPRequiresLatestScripting   = (1 << 18),
            IsMTRenderingDisabledByDefault  = (1 << 19)
        }

        [Flags]
        public enum SupportedTextureCompression
        {
            None   = 0,
            ETC    = (1 << 0),
            ETC2   = (1 << 1),
            PVRTC  = (1 << 2),
            ASTC   = (1 << 3),
            DXTC   = (1 << 4),
            DXT5nm = (1 << 5)
        }

        [Flags]
        public enum VRAttributes
        {
            None                                = 0,
            SupportSinglePassStereoRendering    = (1 << 0),
            SupportStereoInstancingRendering    = (1 << 1),
            SupportStereoMultiviewRendering     = (1 << 2),
            SupportStereo360Capture             = (1 << 3),
            SupportVuforia                      = (1 << 4),
            SupportTango                        = (1 << 5)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DiscoveredTargetInfo
        {
            public string path;
            public string dllName;
            public string dirName;
            public string platformDefine;
            public string niceName;
            public string iconName;
            public string assemblyName;

            public BuildTarget buildTgtPlatformVal;

            // Build targets can have many names to identify them
            public string[] nameList;

            // Build targets can sometimes support more than one renderer
            public int[] rendererList;

            public TargetAttributes flags;

            public VRAttributes vrFlags;

            public bool HasFlag(TargetAttributes flag) { return (flags & flag) == flag; }
        }

        public static extern bool PlatformHasFlag(BuildTarget platform, TargetAttributes flag);

        public static extern bool PlatformGroupHasFlag(BuildTargetGroup group, TargetAttributes flag);

        public static extern bool PlatformGroupHasVRFlag(BuildTargetGroup group, VRAttributes flag);

        public static extern DiscoveredTargetInfo[] GetBuildTargetInfoList();

        public static extern int[] GetRenderList(BuildTarget platform);

        private static extern string GetNiceNameByBuildTarget(BuildTarget platform);

        public static bool BuildTargetSupportsRenderer(BuildPlatform platform, GraphicsDeviceType type)
        {
            BuildTarget buildTarget = platform.defaultTarget;
            if (platform.targetGroup == BuildTargetGroup.Standalone)
                buildTarget = DesktopStandaloneBuildWindowExtension.GetBestStandaloneTarget(buildTarget);

            foreach (int var in GetRenderList(buildTarget))
            {
                if ((GraphicsDeviceType)var == type)
                    return true;
            }

            return false;
        }

        public static string GetBuildTargetNiceName(BuildTarget platform, BuildTargetGroup buildTargetGroup = BuildTargetGroup.Unknown)
        {
            if (PlatformHasFlag(platform, TargetAttributes.SupportsFacebook) && buildTargetGroup == BuildTargetGroup.Facebook)
            {
                return "Facebook";
            }

            return GetNiceNameByBuildTarget(platform);
        }

        public static string GetScriptAssemblyName(DiscoveredTargetInfo btInfo)
        {
            if (!String.IsNullOrEmpty(btInfo.assemblyName))
                return btInfo.assemblyName;

            // Use shortname if assemblyName isn't set
            return btInfo.nameList[kShortNameIndex];
        }
    }
}