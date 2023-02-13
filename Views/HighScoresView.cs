using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace stopInGreen
{
    public class HighScoresView : GameStateView
    {
        private SpriteFont m_font;
        private const string TITLE = "HIGH SCORES";
        private const string NUM1 = "1. ";
        private const string NUM2 = "2. ";
        private const string NUM3 = "3. ";
        private const string NUM4 = "4. ";
        private const string NUM5 = "5. ";
        private bool loading = false;
        private int[] m_loadedScores = new int[5];

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/text");
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.HighScores;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            // title
            Vector2 stringSize = m_font.MeasureString(TITLE);
            m_spriteBatch.DrawString(m_font, TITLE,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 6 - stringSize.Y), Color.Purple);
            // rank column 
            Vector2 numStringSize = m_font.MeasureString(NUM1);
            m_spriteBatch.DrawString(m_font, NUM1, new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 5 - numStringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4), Color.White);
            m_spriteBatch.DrawString(m_font, NUM2, new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 5 - numStringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + numStringSize.Y / 2 * 3), Color.White);
            m_spriteBatch.DrawString(m_font, NUM3, new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 5 - numStringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + numStringSize.Y * 3), Color.White);
            m_spriteBatch.DrawString(m_font, NUM4, new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 5 - numStringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + numStringSize.Y / 2 * 9), Color.White);
            m_spriteBatch.DrawString(m_font, NUM5, new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 5 - numStringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + numStringSize.Y * 6), Color.White);

            // scores column
            

            Vector2 score1StringSize = m_font.MeasureString(m_loadedScores[0].ToString());
            Vector2 score2StringSize = m_font.MeasureString(m_loadedScores[1].ToString());
            Vector2 score3StringSize = m_font.MeasureString(m_loadedScores[2].ToString());
            Vector2 score4StringSize = m_font.MeasureString(m_loadedScores[3].ToString());
            Vector2 score5StringSize = m_font.MeasureString(m_loadedScores[4].ToString());

            
            m_spriteBatch.DrawString(m_font, m_loadedScores[0].ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 7 - score1StringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4), Color.White);
            m_spriteBatch.DrawString(m_font, m_loadedScores[1].ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 7 - score2StringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + score2StringSize.Y / 2 * 3), Color.White);
            m_spriteBatch.DrawString(m_font, m_loadedScores[2].ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 7 - score3StringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + score3StringSize.Y * 3), Color.White);
            m_spriteBatch.DrawString(m_font, m_loadedScores[3].ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 7 - score4StringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + score4StringSize.Y / 2 * 9), Color.White);
            m_spriteBatch.DrawString(m_font, m_loadedScores[4].ToString(), new Vector2(m_graphics.PreferredBackBufferWidth / 12 * 7 - score5StringSize.X / 2, m_graphics.PreferredBackBufferHeight / 4 + score5StringSize.Y * 6), Color.White);


            m_spriteBatch.End();
        }

        public override void update(GameTime gameTime)
        {

            LoadScores();
            if (m_loadedScores == null)
            {
                Debug.WriteLine("not initilized");

                m_loadedScores = new int[]{0, 0, 0, 0, 0};
            }
        }

        private void LoadScores()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    finalizeLoadAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
        }

        private async Task finalizeLoadAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("HighScores.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(int[]));
                                    
                                    m_loadedScores = (int[])mySerializer.Deserialize(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Debug.WriteLine("failed at load in high score");
                    }
                }

                this.loading = false;
            });
        }
    }
}
