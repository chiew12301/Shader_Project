using UnityEngine;

namespace KC_Custom
{
    [CreateAssetMenu(fileName = "SO_Menu", menuName = "KC_Custom/SO_Menu")]
    public class SO_Menu : SingletonScriptableobject<SO_Menu>
    {
        [SerializeField] private AMenu[] m_menuPrefabs;

        public T GetMenuPrefab<T>() where T : AMenu
        {
            for (int i = 0; i < this.m_menuPrefabs.Length; i++)
            {
                AMenu prefab = this.m_menuPrefabs[i];
                if( prefab.GetType() == typeof( T ) )
                    return prefab as T;
            }
            
            throw new MissingReferenceException( "Prefab not found for type " + typeof(T)) ;
        }

        [ContextMenu("Sort")]
        private void SortMenu()
        {
            System.Array.Sort( this.m_menuPrefabs, new System.Comparison<AMenu>( ( menuA, menuB ) => menuA.name.CompareTo( menuB.name ) ) );
        }
    }
}