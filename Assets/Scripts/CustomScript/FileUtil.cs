using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.AddressableAssets.HostingServices;
using UnityEngine;

namespace KC_Custom
{
    public enum EETEXTFOLDER
    {
        XML = 0,
        JSON
    }

    public sealed class FileUtil
    {
        //=======================================================

        #region GLOBAL_FILE_PATH

        #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        public static string GAME_FOLDER { get { return ""; } }
#else
        public static string GAME_FOLDER { get { return string.Format( "{0}/", Application.persistentDataPath ; } }
#endif

        public static string SAVE_FOLDER { get { return FileUtil.EnsureDirectoryExists(FileUtil.GAME_FOLDER + "Saves/"); } }
        public static string SNAPSHOT_FOLDER { get { return FileUtil.EnsureDirectoryExists(FileUtil.SAVE_FOLDER + "Screenshot/"); } }

        public static string XML_FOLDER { get { return "Xml/"; } }
        public static string JSON_FOLDER { get { return "Json/"; } }
        public static string RESOURCE_FOLDER { get { return "Assets/Resources/"; } }

        public static string EnsureDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }


        #endregion GLOBAL_FILE_PATH

        //=======================================================

        #region FILE_METHODS

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void DeleteFile(string path)
        {
            if(File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                Debug.LogWarning(string.Format("[Serializer DeleteFile Error] : File '{0}' does not exists.", path));
            }
        }

        public static int GetTotalOfFiles( string relativeDirectory, [DefaultValue("XML")] EETEXTFOLDER folderType)
        {
            string path = GetFolderPath(folderType) + relativeDirectory;
            TextAsset[] textAssets = Resources.LoadAll<TextAsset>(path);

            if (null == textAssets)
            {
                return 0;
            }

            return textAssets.Length;
        }

        /// <summary>
        /// Get Folder Path from FileUtil, default return will return Json Folder Path.
        /// </summary>
        /// <param name="folderType"></param>
        /// <returns></returns>
        public static string GetFolderPath(EETEXTFOLDER folderType)
        {
            switch(folderType)
            {
                case EETEXTFOLDER.XML:
                    return FileUtil.XML_FOLDER;
                default:
                    return FileUtil.JSON_FOLDER;
            }
        }

        #endregion FILE_METHODS

        //=======================================================

        #region SAVE_METHODS

        public static bool SaveData<T>(string path, T data)
        {
            if( !data.GetType().IsSerializable )
            {
                Debug.LogError(string.Format("[Serializer SaveData] : Error saving NON-serializable datas " +
                                              "( filepath: '{0}', datatype: {1} ).", path, data.GetType()));

                return false;
            }

            try
            {
                FileStream stream = File.Open(path, FileMode.OpenOrCreate);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);

                stream.Flush();
                stream.Close();
                stream.Dispose();
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }

        #endregion SAVE_METHODS

        //=======================================================

        #region LOAD_METHOD

        public static void LoadData<T>(string path, out T loadedData)
        {
            loadedData = LoadData<T>(path);
        }

        public static T LoadData<T>(string path)
        {
            T data = default(T);
            if(!typeof(T).IsSerializable)
            {
                Debug.LogError(string.Format("[Serializer LoadData] : Error loading NON-serializable datas " +
                                              "( filepath: '{0}', datatype: {1} ).", path, typeof(T)));
                return data;
            }

            try
            {
                FileStream stream = File.Open(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                data = (T) Convert.ChangeType(formatter.Deserialize(stream), typeof(T));
                stream.Flush();
                stream.Close();
                stream.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return data;
        }

        #endregion LOAD_METHOD

        //=======================================================
    }

}