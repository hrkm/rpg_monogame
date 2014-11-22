namespace RPG_ver_3
{
    /// <summary>
    /// Interfejs definiujący różne zachowania, które
    /// można dołączyć do obiektów GameObject.
    /// </summary>
    public interface IBehaviour : IUpdateable
    {
        void Apply(GameObject gameObject);
    }
}
