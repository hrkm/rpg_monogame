using Microsoft.Xna.Framework;

namespace RPG_ver_2
{
    /// <summary>
    /// Interfejs definiujący różne zachowania, które
    /// można dołączyć do obiektów GameObject.
    /// </summary>
    public interface IBehaviour
    {
        void Update(GameTime gameTime);
        void Apply(GameObject gameObject);
    }
}
