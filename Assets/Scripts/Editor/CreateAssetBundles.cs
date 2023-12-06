
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace ASSETSBUNDLE
{
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        public static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            if(!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            /*
             * BuildAssetBundleOptions try to remain to None, unless to debug error use dryrunbuild. strictmode will stop build if ab failed to build.
             * Uncompressedassetbundle is not useful as the disk will take more space.
             * EditrorUserBuildSettings try will build according to your build setting. Is a specific platform to build assets bundle based on the platform.
             * Example: if you have ios and android. You need two assets bundle on server to get the actual correct bundle.
             */
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem("Assets/Build Android AssetBundles")]
        public static void BuildAndroidBundles()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            if(!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            /*
             * BuildAssetBundleOptions try to remain to None, unless to debug error use dryrunbuild. strictmode will stop build if ab failed to build.
             * Uncompressedassetbundle is not useful as the disk will take more space.
             * EditrorUserBuildSettings try will build according to your build setting. Is a specific platform to build assets bundle based on the platform.
             * Example: if you have ios and android. You need two assets bundle on server to get the actual correct bundle.
             */
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
        }

        [MenuItem("Assets/Build IOS AssetBundles")]
        public static void BuildIOSBundles()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            if(!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            /*
             * BuildAssetBundleOptions try to remain to None, unless to debug error use dryrunbuild. strictmode will stop build if ab failed to build.
             * Uncompressedassetbundle is not useful as the disk will take more space.
             * EditrorUserBuildSettings try will build according to your build setting. Is a specific platform to build assets bundle based on the platform.
             * Example: if you have ios and android. You need two assets bundle on server to get the actual correct bundle.
             */
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);
        }
    }
}
#endif