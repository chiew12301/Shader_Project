namespace KC_Custom
{
    public abstract class ASimpleMenu<T> : AMenu<T> where T :ASimpleMenu<T>
    {
        public static void Show()
        {
            Open();
        }

        public static void Hide()
        {
            Close();
        }
    }
}