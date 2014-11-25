using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG_ver_6
{
    public interface IDrawable
    {
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
