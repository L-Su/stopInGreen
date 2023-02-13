using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace stopInGreen

{
    public class CreditsView : GameStateView
    {
        private SpriteFont m_font;
        private SpriteFont m_fontNote;
        private SpriteFont m_fontNoteBold;
        private const string TITLE = "CREDICTS";
        private const string DEVELOPER = "DEVELOPER";
        private const string ART = "ART";
        //private const string SOUND = "SOUND from Freesound";
        private const string ART_SOURCE = "PNGTREE";
        //private const string SOUND_SOURCE = "V-ktor    myfox14    Leszek_Szary  adcbicycle  MentosLat";
        private const string ME = "Leilin Su";

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/title");
            m_fontNote = contentManager.Load<SpriteFont>("Fonts/text");
            m_fontNoteBold = contentManager.Load<SpriteFont>("Fonts/textBold");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.Credits;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            Vector2 stringTitleSize = m_font.MeasureString(TITLE);
            Vector2 stringDevelopSize = m_fontNoteBold.MeasureString(DEVELOPER);
            Vector2 stringDeveloperSize = m_fontNote.MeasureString(ME);
            Vector2 stringArtSize = m_fontNoteBold.MeasureString(ART);
            Vector2 stringArtSourceSize = m_fontNote.MeasureString(ART_SOURCE);
            //Vector2 stringSoundSize = m_fontNoteBold.MeasureString(SOUND);
            //Vector2 stringSoundSourceSize = m_fontNote.MeasureString(SOUND_SOURCE);
            m_spriteBatch.DrawString(m_font, TITLE,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringTitleSize.X / 2, m_graphics.PreferredBackBufferHeight / 6 - stringTitleSize.Y), Color.Purple);
            m_spriteBatch.DrawString(m_fontNoteBold, DEVELOPER,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringDevelopSize.X / 2, m_graphics.PreferredBackBufferHeight / 3 - stringDevelopSize.Y), Color.White);
            m_spriteBatch.DrawString(m_fontNote, ME,
                 new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringDeveloperSize.X / 2, m_graphics.PreferredBackBufferHeight / 3 - stringDeveloperSize.Y + m_graphics.PreferredBackBufferHeight / 10), Color.White); 
            m_spriteBatch.DrawString(m_fontNoteBold, ART,
                 new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringArtSize.X / 2, m_graphics.PreferredBackBufferHeight / 3 - stringArtSize.Y + m_graphics.PreferredBackBufferHeight / 5), Color.White);
            m_spriteBatch.DrawString(m_fontNote, ART_SOURCE,
                 new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringArtSourceSize.X / 2, m_graphics.PreferredBackBufferHeight / 3 - stringArtSourceSize.Y + m_graphics.PreferredBackBufferHeight / 10 * 3), Color.White);
            //m_spriteBatch.DrawString(m_fontNoteBold, SOUND,
                 //new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSoundSize.X / 2, m_graphics.PreferredBackBufferHeight / 3 - stringSoundSize.Y + m_graphics.PreferredBackBufferHeight / 5 * 2), Color.White);
            //m_spriteBatch.DrawString(m_fontNote, SOUND_SOURCE,
                 //new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSoundSourceSize.X / 2, m_graphics.PreferredBackBufferHeight / 3 - stringSoundSourceSize.Y + m_graphics.PreferredBackBufferHeight / 2), Color.White);
            
            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {
        }
    }
}
