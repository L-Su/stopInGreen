
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stopInGreen.Input;
using stopInGreen.Particles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace stopInGreen
{
    class GameModel
    {
        private readonly int WINDOW_WIDTH;
        private readonly int WINDOW_HEIGHT;
        private readonly GraphicsDevice GRAPHICS_DEVICE;
        private SpriteBatch m_spriteBatch;
        private ContentManager ctMng;

        private SpriteFont m_font;

        private Texture2D m_textMeter;
        private Texture2D m_textShade;
        private Texture2D m_textMarker;
        private Texture2D m_textBorder;
        private Texture2D m_textGameOver;

        private Rectangle m_rectMeter;
        private Rectangle m_rectShade;
        private Rectangle m_rectMarker;
        private Rectangle m_rectBorder;
        private Rectangle m_rectGameOver;

        private KeyboardState kbd;
        private KeyboardState prevKbd;

        private ParticleEmitter m_emitter1;
        private ParticleEmitter m_emitter2;
        private ParticleEmitter m_emitter3;
        private ParticleEmitter m_emitter4;
        private List<ParticleEmitter> emitters;
        private List<ParticleEmitter> rightEmitter;
        private List<ParticleEmitter> bottomEmitter;
        private List<ParticleEmitter> topEmitter;


        private TimeSpan particleTimer = new TimeSpan(0, 0, 0, 1);
        private TimeSpan toMainTimer = new TimeSpan(0, 0, 0, 0, 700);
        private TimeSpan HitPause = new TimeSpan(0, 0, 0, 1);

        private MyRandom m_rand;

        private string stringGameOver = "GAME OVER";
        private const string UR_SCORE_MSG = "YOUR SCORE";

        private int meterHeight;
        private int shadeWidth;
        private int markerWidth;

        private float markerSpeed;

        private bool stopMarker = false;
        private bool particleOn = false;
        private bool gameOver = false;
        public static bool returnMain = false;
        private bool refreshed = false;
        private bool canPress = true;

        private int level;
        private int levelCount;

        private KeyboardInput m_inputKeyboard;

        private int score;
        private int[] highScores = new int[5];
        private int[] m_loadedHighScores = new int[5];
        private bool loadedControls = false;
        private int[] m_loadedControls = null;

        private bool saving = false;
        private bool loading = false;
        private bool loaded = false;
        private bool loadingControl = false;

        public GameModel(int width, int height, GraphicsDevice gd)
            {
            this.WINDOW_WIDTH = width;
            this.WINDOW_HEIGHT = height;
            this.GRAPHICS_DEVICE = gd;
        }

        public void Initialize(ContentManager content, SpriteBatch spriteBatch)
        {
            m_spriteBatch = spriteBatch;

            ctMng = content;

            //var texSquare = content.Load<Texture2D>("Images/square");
            LoadHighScores();
            LoadControls();
            if (m_loadedHighScores == null)
            {
                Debug.WriteLine("load nothing");
                highScores = new int[] { 0, 0, 0, 0, 0 };
            }
            else
            {
                highScores = m_loadedHighScores;
            }

            /*if (m_loadedControls == null)
            {
                Debug.WriteLine("control load is none in play when loadcontent");
                m_loadedControls = new int[] { 37, 39, 38, 40, 32 };
            }
            m_inputKeyboard = new KeyboardInput();*/
/*            m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_loadedControls[0].GetHashCode()), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { onMoveLeft(gameTime); }), "left");
            m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_loadedControls[1].GetHashCode()), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { onMoveRight(gameTime); }), "right");
            m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_loadedControls[2].GetHashCode()), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { onMoveUp(gameTime); }), "up");
            m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_loadedControls[3].GetHashCode()), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { onMoveDown(gameTime); }), "down");
            m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_loadedControls[4].GetHashCode()), true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { onFire(); }), "fire");*/

            // font
            m_font = content.Load<SpriteFont>("Fonts/text");


            m_textMeter = new Texture2D(GRAPHICS_DEVICE, 1, 1);
            m_textMeter.SetData(new[] { Color.Blue });
            m_textShade = new Texture2D(GRAPHICS_DEVICE, 1, 1);
            m_textShade.SetData(new[] { Color.Green });
            m_textMarker = new Texture2D(GRAPHICS_DEVICE, 1, 1);
            m_textMarker.SetData(new[] { Color.Yellow });
            m_textBorder = new Texture2D(GRAPHICS_DEVICE, 1, 1);
            m_textBorder.SetData(new[] { Color.Black });

            m_textGameOver = new Texture2D(GRAPHICS_DEVICE, 1, 1);
            m_textGameOver.SetData(new[] { Color.Gray });

            meterHeight = WINDOW_HEIGHT / 10;
            shadeWidth = WINDOW_WIDTH / 5 * 3;
            markerWidth = shadeWidth / 50;


            m_rectMeter = new Rectangle(0, WINDOW_HEIGHT / 2 - meterHeight / 2, WINDOW_WIDTH, meterHeight);
            m_rectShade = new Rectangle(WINDOW_WIDTH / 2 - shadeWidth / 2, WINDOW_HEIGHT / 2 - meterHeight / 2, shadeWidth, meterHeight);

            m_rectMarker = new Rectangle(0, WINDOW_HEIGHT / 2 - meterHeight / 2, markerWidth, meterHeight);
            m_rectMeter = new Rectangle(0, WINDOW_HEIGHT / 2 - meterHeight / 2, WINDOW_WIDTH, meterHeight);

            m_rectGameOver = new Rectangle((WINDOW_WIDTH - WINDOW_WIDTH / 3 * 2) / 2, (WINDOW_HEIGHT - WINDOW_HEIGHT / 3) / 2, WINDOW_WIDTH / 3 * 2, WINDOW_HEIGHT / 3);

            stopMarker = false;
            particleOn = false;
            gameOver = false;
            returnMain = false;
            refreshed = false;
            canPress = true;

            level = 1;
            levelCount = 6;
            score = 0;

            // 1s to pass
            markerSpeed = WINDOW_WIDTH / 1000.0f;

            m_rand = new MyRandom();

            bottomEmitter = new List<ParticleEmitter>();
            topEmitter = new List<ParticleEmitter>();
            emitters = new List<ParticleEmitter>();
            rightEmitter = new List<ParticleEmitter>();
        }

        public void Update(GameTime gameTime)
        {

            if (!loaded)
            {
                LoadHighScores();
                if (m_loadedHighScores == null)
                {
                    Debug.WriteLine("load nothing");
                    highScores = new int[] { 0, 0, 0, 0, 0 };
                }
                else
                {
                    highScores = m_loadedHighScores;

                }
                loaded = true;
            }

            MoveMarker(gameTime);
            PressSpace();

            if (stopMarker)
            {
                if (m_rectMarker.X >= m_rectShade.X && m_rectMarker.X + m_rectMarker.Width <= m_rectShade.X + m_rectShade.Width)
                {
                    if (HitPause.Equals(new TimeSpan(0, 0, 0, 1)))
                    {
                        score = score + level * 2;
                    }

                    if (level < levelCount)
                    {
                        HitPause -= gameTime.ElapsedGameTime;
                        canPress = false;
                        if (HitPause <= TimeSpan.Zero)
                        {
                            level += 1;
                            ShrinkShade();
                            HitPause = new TimeSpan(0, 0, 0, 1);
                        }
                        
                    }
                    else
                    {
                        HitPause -= gameTime.ElapsedGameTime;
                        canPress = false;
                        if (HitPause <= TimeSpan.Zero)
                        {
                            stopMarker = false;
                            HitPause = new TimeSpan(0, 0, 0, 1);
                        }
                    }

                    
                    particleOn = true;
                    CreateParticles(m_rectMarker);
                   

                }
                else
                {
                    gameOver = true;
                }
            }

            if (gameOver)
            {
                if (!refreshed)
                {
                    RefreshHighScores(score);
                    refreshed = true;
                }

                toMainTimer -= gameTime.ElapsedGameTime;
                if (toMainTimer < TimeSpan.Zero)
                {
                    returnMain = true;
                    toMainTimer = new TimeSpan(0, 0, 0, 0, 700);
                }
            }

            if (particleOn)
            {
                foreach (var emitter in emitters)
                {
                    emitter.update(gameTime);
                }
                /*m_emitter1.update(gameTime);
                m_emitter2.update(gameTime);
                m_emitter3.update(gameTime);
                m_emitter4.update(gameTime);*/
                particleTimer -= gameTime.ElapsedGameTime;
                if (particleTimer < TimeSpan.Zero)
                {
                    particleOn = false;
                    particleTimer = new TimeSpan(0, 0, 0, 1);
                    emitters.Clear();
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            Vector2 stringSize = m_font.MeasureString(score.ToString());
            m_spriteBatch.DrawString(m_font, score.ToString(), new Vector2(WINDOW_WIDTH / 2 - stringSize.X / 2, WINDOW_HEIGHT / 20), Color.Red);

            m_spriteBatch.Draw(m_textBorder, m_rectMeter, Color.White);
            Rectangle inner = new Rectangle(m_rectMeter.X + 2, m_rectMeter.Y + 2, m_rectMeter.Width - 4, m_rectMeter.Height - 4);
            m_spriteBatch.Draw(m_textMeter, inner, Color.White);

            m_spriteBatch.Draw(m_textBorder, m_rectShade, Color.White);
            inner = new Rectangle(m_rectShade.X + 2, m_rectShade.Y + 2, m_rectShade.Width - 4, m_rectShade.Height - 4);
            m_spriteBatch.Draw(m_textShade, inner, Color.White);

            m_spriteBatch.Draw(m_textBorder, m_rectMarker, Color.White);
            inner = new Rectangle(m_rectMarker.X + 2, m_rectMarker.Y + 2, m_rectMarker.Width - 4, m_rectMarker.Height - 4);
            m_spriteBatch.Draw(m_textMarker, inner, Color.White);

            if (particleOn)
            {
                foreach( var emitter in emitters)
                {
                    emitter.draw(m_spriteBatch);
                }
                /*m_emitter1.draw(m_spriteBatch);
                m_emitter2.draw(m_spriteBatch);
                m_emitter3.draw(m_spriteBatch);
                m_emitter4.draw(m_spriteBatch);*/
            }
            
            if (gameOver)
            {
                m_spriteBatch.Draw(m_textGameOver, m_rectGameOver, Color.White);
                Vector2 stringSizeGameOver = m_font.MeasureString(stringGameOver);
                Vector2 stringSizeURSCORE = m_font.MeasureString(UR_SCORE_MSG);
                Vector2 stringSizeScore = m_font.MeasureString(score.ToString());
                m_spriteBatch.DrawString(m_font, stringGameOver,
                    new Vector2(WINDOW_WIDTH /2 - stringSizeGameOver.X / 2, WINDOW_HEIGHT / 2 - stringSizeGameOver.Y / 2 - WINDOW_HEIGHT / 8),
                    Color.Red);
                m_spriteBatch.DrawString(m_font, UR_SCORE_MSG,
                    new Vector2(WINDOW_WIDTH / 2 - stringSizeURSCORE.X / 2, WINDOW_HEIGHT / 2 - stringSizeURSCORE.Y / 2 - WINDOW_HEIGHT / 8 + WINDOW_HEIGHT / 12),
                    Color.White);
                m_spriteBatch.DrawString(m_font, score.ToString(),
                    new Vector2(WINDOW_WIDTH / 2 - stringSizeScore.X / 2, WINDOW_HEIGHT / 2 - stringSizeScore.Y / 2 - WINDOW_HEIGHT / 8 + WINDOW_HEIGHT / 6),
                    Color.White);

                
            }


            m_spriteBatch.End();

        }


        private void ShrinkShade()
        {
            stopMarker = false;
            m_rectMarker.X = 0;
            m_rectShade.Width = m_rectShade.Width - m_rectShade.Width / 3;
            m_rectShade.X = WINDOW_WIDTH / 2 - m_rectShade.Width / 2;
            canPress = true;
        }

        private void CreateParticles(Rectangle marker)
        {
            // top
            for (int i = 0; i < 5; i++)
            {
                m_emitter1 = new ParticleEmitter(
                ctMng,
                new TimeSpan(0, 0, 0, 0, 200),
                (int)m_rand.nextRange(marker.X, marker.X + marker.Width), marker.Y,
                5,
                1,
                new TimeSpan(0, 0, 0, 0, 100));

                emitters.Add(m_emitter1);
            }
            // left
            for (int i = 0; i < 5; i++)
            {
                m_emitter2 = new ParticleEmitter(
                ctMng,
                new TimeSpan(0, 0, 0, 0, 100),
                marker.X, (int)m_rand.nextRange(marker.Y, marker.Y + marker.Height),
                5,
                1,
                new TimeSpan(0, 0, 0, 0, 200));
                emitters.Add(m_emitter2);
            }

            // right
            for (int i = 0; i < 5; i++)
            {
                m_emitter1 = new ParticleEmitter(
                ctMng,
                new TimeSpan(0, 0, 0, 0, 200),
                (marker.X + marker.Width), (int)m_rand.nextRange(marker.Y, marker.Y + marker.Height),
                5,
                1,
                new TimeSpan(0, 0, 0, 0, 100));

                emitters.Add(m_emitter1);
            }
            // bottom
            for (int i = 0; i < 5; i++)
            {
                m_emitter2 = new ParticleEmitter(
                ctMng,
                new TimeSpan(0, 0, 0, 0, 100),
                (int)m_rand.nextRange(marker.X, marker.X + marker.Width), (marker.Y + marker.Height),
                5,
                1,
                new TimeSpan(0, 0, 0, 0, 200));
                emitters.Add(m_emitter2);
            }

        }

        public void MoveMarker(GameTime gameTime)
        {
            if (!stopMarker)
            {
                // go right
                if (m_rectMarker.X <= 0)
                {
                    markerSpeed = WINDOW_WIDTH / 1000.0f;
                }
                //go left
                else if (m_rectMarker.X >= WINDOW_WIDTH - m_rectMarker.Width)
                {
                    markerSpeed = -(WINDOW_WIDTH / 1000.0f);
                }
                int moveDistance = (int)(gameTime.ElapsedGameTime.TotalMilliseconds * markerSpeed);
                m_rectMarker.X += moveDistance;
            }
            
        }

        public void PressSpace()
        {
            kbd = Keyboard.GetState();
            if (kbd.IsKeyDown(Keys.Space) && prevKbd.IsKeyUp(Keys.Space))
            {
                stopMarker = true;
            }

            prevKbd = kbd;
        }

        private void RefreshHighScores(int currentScore)
        {
            int[] source = (int[])highScores.Clone();

            for (int i = 0; i < highScores.Length; i++)
            {
                if (currentScore > highScores[i])
                {
                    highScores[i] = currentScore;

                    Array.Copy(source, i, highScores, i + 1, highScores.Length - (i + 1));
                    SaveHighScores();
                    return;
                }
            }

        }

        private void LoadHighScores()
        {
            lock (this)
            {
                if (!this.loading)
                {
                    this.loading = true;
#pragma warning disable CS4014
                    FinalizeLoadAsync();
#pragma warning disable CS4014
                }
            }
        }

        private async Task FinalizeLoadAsync()
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
                                    m_loadedHighScores = (int[])mySerializer.Deserialize(fs);
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Debug.WriteLine("Load content failed");
                    }
                }
                this.loading = false;
            });
        }

        private void SaveHighScores()
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;

                    FinalizeSaveAsync(highScores);
                }
            }
        }

        private async void FinalizeSaveAsync(int[] scores)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("HighScores.xml", FileMode.OpenOrCreate))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(int[]));
                                mySerializer.Serialize(fs, scores);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Debug.WriteLine("Save content failed");
                    }
                }
                this.saving = false;
            });
        }

        private void LoadControls()
        {
            lock (this)
            {
                if (!this.loadingControl)
                {
                    this.loadingControl = true;
#pragma warning disable CS4014
                    FinalizeLoadControlAsync();
#pragma warning disable CS4014
                }
            }
        }

        private async Task FinalizeLoadControlAsync()
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists("Controls.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("Controls.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(int[]));
                                    m_loadedControls = (int[])mySerializer.Deserialize(fs);
                                    Debug.WriteLine("controls should be loaded");
                                    /*foreach (int i in m_loadedControls)
                                    {
                                        Debug.WriteLine(i);
                                    }*/
                                }
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        Debug.WriteLine("Load content failed");
                    }
                }
                this.loadingControl = false;
            });
        }
    }
}
