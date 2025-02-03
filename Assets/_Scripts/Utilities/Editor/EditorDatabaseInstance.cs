//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;

//namespace com.game.utilities.editor
//{
//    public class EditorDatabaseInstanceInterfaceBased<T> : EditorDatabaseInstance<T> where T : UnityEngine.Object, IDatabaseMember
//    {
//        public EditorDatabaseInstanceInterfaceBased() : base()
//        {
//        }

//        public EditorDatabaseInstanceInterfaceBased(string[] searchInFolders) : base(searchInFolders)
//        {
//        }

//        protected override bool TryGenerateKey(T target, out string output)
//        {
//            output = target.GetDatabaseKey();
//            return true;
//        }
//    }

//    public abstract class EditorDatabaseInstance<T> : IDatabaseInstance<T> where T : UnityEngine.Object
//    {
//        protected Dictionary<string, T> m_dictionary;
//        public int Size => m_dictionary.Count;

//        protected string[] m_targetFolders;

//        public EditorDatabaseInstance()
//        {
//            m_dictionary = new Dictionary<string, T>();
//            m_targetFolders = null;
//            Refresh();
//        }

//        public EditorDatabaseInstance(string[] searchInFolders)
//        {
//            m_dictionary = new Dictionary<string, T>();
//            m_targetFolders = searchInFolders;
//            Refresh();
//        }

//        public bool Contains(string key)
//        {
//            return m_dictionary.ContainsKey(key);
//        }

//        public T Get(string key)
//        {
//            return m_dictionary[key];
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            return m_dictionary.Values.GetEnumerator();
//        }

//        public void Refresh()
//        {
//            m_dictionary.Clear();
//            string[] guids = null;

//            if (m_targetFolders == null) guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
//            else guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", m_targetFolders);

//            if (guids.Length == 0)
//                return;

//            foreach (string guid in guids)
//            {
//                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
//                T assetLoaded = AssetDatabase.LoadAssetAtPath<T>(assetPath);

//                string key = null;
//                if (!TryGenerateKey(assetLoaded, out key))
//                    continue;

//                m_dictionary.Add(key, assetLoaded);
//            }
//        }

//        protected abstract bool TryGenerateKey(T assetLoaded, out string output);

//        public bool TryGet(string key, out T output)
//        {
//            bool result = m_dictionary.TryGetValue(key, out output);
//            return result;
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return m_dictionary.GetEnumerator();
//        }
//    }
//}
