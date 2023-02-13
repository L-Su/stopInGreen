using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace stopInGreen
{
    public class GamePlayView : GameStateView
    {
        ContentManager m_content;
        private GameModel m_gameModel;

        public override void initializeSession()
        {
            m_gameModel = new GameModel(m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight, m_graphics.GraphicsDevice);
            m_gameModel.Initialize(m_content, m_spriteBatch);
        }

        public override void loadContent(ContentManager content)
        {
            m_content = content;
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GameModel.returnMain)
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.GamePlay;
        }

        public override void render(GameTime gameTime)
        {
            m_gameModel.Draw(gameTime);
        }

        public override void update(GameTime gameTime)
        {
            m_gameModel.Update(gameTime);
        }
    }
}
