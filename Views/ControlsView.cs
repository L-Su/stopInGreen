using stopInGreen.Input;
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
    public class ControlsView : GameStateView
    {
        private SpriteFont m_font;
        private SpriteFont m_fontMenuSelect;
        private SpriteFont m_fontNote;

        private const string CONTROLS = "CONTROLS";
        private const string MOVELEFT = "MOVE LEFT";
        private const string MOVERIGHT = "MOVE RIGHT";
        private const string MOVEUP = "MOVE UP";
        private const string MOVEDOWN = "MOVE DOWN";
        private const string FIRE = "FIRE";

        private string leftKey = "Left";
        private string rightKey = "Right";
        private string upKey = "Up";
        private string downKey = "Down";
        private string fireKey = "Space";

        private bool m_waitForKeyRelease = false;
        private bool enterRelease = false;
        private bool set = false;
        private bool refresh = false;
        private bool loading = false;
        private bool saving = false;
        private bool canLoad = true;

        private int[] m_loadedControls = null;
        private int[] m_controls = new int[5];

        public static KeyboardInput m_inputKeyboard;

        private KeyboardState kbd;
        private KeyboardState prevKbd;

        private Texture2D m_textPopup;
        private Rectangle m_rectPopup;



        private enum ControlsState
        {
            MoveLeft,
            MoveRight,
            MoveUp,
            MoveDown,
            Fire
        }

        private ControlsState m_currentSelection;
        private ControlsState m_changeSelection;

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/text");
            m_fontMenuSelect = contentManager.Load<SpriteFont>("Fonts/title");
            m_fontNote = contentManager.Load<SpriteFont>("Fonts/note");

            m_textPopup = new Texture2D(m_graphics.GraphicsDevice, 1, 1);
            m_textPopup.SetData(new[] { Color.Gray });

            // [width / 4 * 3, height / 6]
            // x starts at w / 8 to be centered, y centered
            m_rectPopup = new Rectangle(m_graphics.PreferredBackBufferWidth / 8, m_graphics.PreferredBackBufferHeight / 2 - m_graphics.PreferredBackBufferHeight / 6 / 2, m_graphics.PreferredBackBufferWidth / 4 * 3, m_graphics.PreferredBackBufferHeight / 6);

            LoadControls();

            m_currentSelection = ControlsState.MoveLeft;
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            // todo: RESET ALL, RESET OR EMPTY IS DUPLICATE

            // Stay clean after reenter this view
            if (refresh)
            {
                m_currentSelection = ControlsState.MoveLeft;
                m_waitForKeyRelease = false;
                enterRelease = false;
                set = false;
                refresh = false;
            }

            kbd = Keyboard.GetState();
            // back to main menu
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !set && prevKbd.IsKeyUp(Keys.Escape))
            {
                refresh = true;
                return GameStateEnum.MainMenu;
            }

            // browse choice
            if (!m_waitForKeyRelease && !set && prevKbd.IsKeyUp(Keys.Up) && prevKbd.IsKeyUp(Keys.Down))
            {
                // Arrow keys to navigate the menu
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && m_currentSelection != ControlsState.Fire)
                {
                    m_currentSelection = m_currentSelection + 1;
                    m_waitForKeyRelease = true;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Up) && m_currentSelection != ControlsState.MoveLeft)
                {
                    m_currentSelection = m_currentSelection - 1;
                    m_waitForKeyRelease = true;
                }
            }

            // not pressing up or down key
            else if (Keyboard.GetState().IsKeyUp(Keys.Down) && Keyboard.GetState().IsKeyUp(Keys.Up))
            {
                m_waitForKeyRelease = false;
            }

            // modified method to pop up window when customizing
            if (enterRelease && !set && prevKbd.IsKeyUp(Keys.Enter))
            {
                // If enter is pressed, select a new key
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    m_changeSelection = m_currentSelection;
                    set = true;
                    enterRelease = false;
                }
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Enter))
            {
                enterRelease = true;
            }

            prevKbd = kbd;

            return GameStateEnum.Controls;
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            // Title
            Vector2 controlsStringSize = m_font.MeasureString(CONTROLS);
            m_spriteBatch.DrawString(m_font, CONTROLS,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - controlsStringSize.X / 2, m_graphics.PreferredBackBufferHeight / 6 - controlsStringSize.Y), Color.Purple);
            
            // columns : 1/3, 2/3
            // rows: starts at 1/3, add 1/9 each row
            Vector2 stringSize = m_font.MeasureString(MOVELEFT);
            float column1X = 1;
            float column2X = 2;


            drawMenuItem(
            m_currentSelection == ControlsState.MoveLeft ? m_fontMenuSelect : m_font,
            MOVELEFT, column1X,
            (int)ControlsState.MoveLeft,
            m_currentSelection == ControlsState.MoveLeft ? Color.MediumPurple : Color.White);
            drawMenuItem(m_currentSelection == ControlsState.MoveLeft ? m_fontMenuSelect : m_font, leftKey, column2X, (int)ControlsState.MoveLeft, m_currentSelection == ControlsState.MoveLeft ? Color.MediumPurple : Color.White);

            drawMenuItem(m_currentSelection == ControlsState.MoveRight ? m_fontMenuSelect : m_font, MOVERIGHT, column1X, (int)ControlsState.MoveRight, m_currentSelection == ControlsState.MoveRight ? Color.MediumPurple : Color.White);
            drawMenuItem(m_currentSelection == ControlsState.MoveRight ? m_fontMenuSelect : m_font, rightKey, column2X, (int)ControlsState.MoveRight, m_currentSelection == ControlsState.MoveRight ? Color.MediumPurple : Color.White);

            drawMenuItem(m_currentSelection == ControlsState.MoveUp ? m_fontMenuSelect : m_font, MOVEUP, column1X, (int)ControlsState.MoveUp, m_currentSelection == ControlsState.MoveUp ? Color.MediumPurple : Color.White);
            drawMenuItem(m_currentSelection == ControlsState.MoveUp ? m_fontMenuSelect : m_font, upKey, column2X, (int)ControlsState.MoveUp, m_currentSelection == ControlsState.MoveUp ? Color.MediumPurple : Color.White);

            drawMenuItem(m_currentSelection == ControlsState.MoveDown ? m_fontMenuSelect : m_font, MOVEDOWN, column1X, (int)ControlsState.MoveDown, m_currentSelection == ControlsState.MoveDown ? Color.MediumPurple : Color.White);
            drawMenuItem(m_currentSelection == ControlsState.MoveDown ? m_fontMenuSelect : m_font, downKey, column2X, (int)ControlsState.MoveDown, m_currentSelection == ControlsState.MoveDown ? Color.MediumPurple : Color.White);

            drawMenuItem(m_currentSelection == ControlsState.Fire ? m_fontMenuSelect : m_font, FIRE, column1X, (int)ControlsState.Fire, m_currentSelection == ControlsState.Fire ? Color.MediumPurple : Color.White);
            drawMenuItem(m_currentSelection == ControlsState.Fire ? m_fontMenuSelect : m_font, fireKey, column2X, (int)ControlsState.Fire, m_currentSelection == ControlsState.Fire ? Color.MediumPurple : Color.White);

            if (set)
            {
                m_spriteBatch.Draw(m_textPopup, m_rectPopup, Color.White);

                // pop up window when customizing new key
                String message1 = "Bind " + Enum.GetName(typeof(ControlsState), m_changeSelection);
                String message2 = "Press any key now or ESC to cancel";
                Vector2 message1StringSize = m_font.MeasureString(message1);
                Vector2 message2StringSize = m_fontNote.MeasureString(message2);
                m_spriteBatch.DrawString(m_font, message1,
               new Vector2(m_graphics.PreferredBackBufferWidth / 2 - message1StringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - message1StringSize.Y / 2 - m_graphics.PreferredBackBufferHeight / 24), Color.White);
                m_spriteBatch.DrawString(m_fontNote, message2,
               new Vector2(m_graphics.PreferredBackBufferWidth / 2 - message2StringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - message2StringSize.Y / 2 + m_graphics.PreferredBackBufferHeight / 24), Color.White);

            }

            m_spriteBatch.End();
        }

        private float drawMenuItem(SpriteFont font, string text, float x, float y, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            m_spriteBatch.DrawString(
                font,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 3  * x - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 3 - stringSize.Y + m_graphics.PreferredBackBufferHeight / 9 * y),
                color);

            return y + 1;
        }

        public override void update(GameTime gameTime)
        {
            //test
            /*Debug.WriteLine(Keys.Up);
            Debug.WriteLine(Keys.Up.GetHashCode());
            Debug.WriteLine(Enum.ToObject(typeof(Keys), Keys.Up.GetHashCode()));
            Debug.WriteLine(Enum.ToObject(typeof(Keys), Keys.Up.GetHashCode()).ToString());*/

            // left, right, up, down, fire
            if (canLoad)
            {
                /*LoadControls();*/
                if (m_loadedControls != null)
                {
                    
                    m_controls = m_loadedControls;
                    leftKey = Enum.ToObject(typeof(Keys), m_controls[0]).ToString();
                    rightKey = Enum.ToObject(typeof(Keys), m_controls[1]).ToString();
                    upKey = Enum.ToObject(typeof(Keys), m_controls[2]).ToString();
                    downKey = Enum.ToObject(typeof(Keys), m_controls[3]).ToString();
                    fireKey = Enum.ToObject(typeof(Keys), m_controls[4]).ToString();

                }
                else
                {
                    Debug.WriteLine("nothing in load in update");
                    m_controls[0] = Keys.Left.GetHashCode();
                    m_controls[1] = Keys.Right.GetHashCode();
                    m_controls[2] = Keys.Up.GetHashCode();
                    m_controls[3] = Keys.Down.GetHashCode();
                    m_controls[4] = Keys.Space.GetHashCode();
                    leftKey = Enum.ToObject(typeof(Keys), m_controls[0]).ToString();
                    rightKey = Enum.ToObject(typeof(Keys), m_controls[1]).ToString();
                    upKey = Enum.ToObject(typeof(Keys), m_controls[2]).ToString();
                    downKey = Enum.ToObject(typeof(Keys), m_controls[3]).ToString();
                    fireKey = Enum.ToObject(typeof(Keys), m_controls[4]).ToString();
                    SaveControls();
                }
                canLoad = false;
            }
            

            

            if (set && enterRelease)
            {
                Keys[] keys = Keyboard.GetState().GetPressedKeys();
                if (keys.Length!=0)
                {
                    Keys k = keys[keys.Length - 1];

                    if (k.Equals(Keys.Escape))
                    {
                        set = false;
                    }
                    else
                    {
                        if (m_changeSelection == ControlsState.MoveLeft)
                        {
                            //leftKey = k.ToString();
                            //m_inputKeyboard.registerCommand(k, false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveLeft(gameTime); }), "left");
                            m_controls[0] = k.GetHashCode();
                            Debug.WriteLine(m_controls[0]);

                            //m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_controls[0]), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveLeft(gameTime); }), "left");
                            leftKey = Enum.ToObject(typeof(Keys), m_controls[0]).ToString();
                            SaveControls();
                        }
                        else if (m_changeSelection == ControlsState.MoveRight)
                        {
                            //rightKey = k.ToString();
                            //m_inputKeyboard.registerCommand(k, false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveRight(gameTime); }), "right");
                            m_controls[1] = k.GetHashCode();
                            Debug.WriteLine(m_controls[1]);

                            //m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_controls[1]), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveRight(gameTime); }), "right");
                            rightKey = Enum.ToObject(typeof(Keys), m_controls[1]).ToString();
                            SaveControls();
                        }
                        else if (m_changeSelection == ControlsState.MoveUp)
                        {
                            //upKey = k.ToString();
                            //m_inputKeyboard.registerCommand(k, false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveUp(gameTime); }), "up");
                            m_controls[2] = k.GetHashCode();
                            Debug.WriteLine(m_controls[2]);

                           // m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_controls[2]), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveUp(gameTime); }), "up");
                            upKey = Enum.ToObject(typeof(Keys), m_controls[2]).ToString();
                            SaveControls();
                        }
                        else if (m_changeSelection == ControlsState.MoveDown)
                        {
                            //downKey = k.ToString();
                            //m_inputKeyboard.registerCommand(k, false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveDown(gameTime); }), "down");
                            m_controls[3] = k.GetHashCode();
                            Debug.WriteLine(m_controls[3]);
                            //m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_controls[3]), false, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onMoveDown(gameTime); }), "down");
                            downKey = Enum.ToObject(typeof(Keys), m_controls[3]).ToString();
                            SaveControls();
                        }
                        else if (m_changeSelection == ControlsState.Fire)
                        {
                            //fireKey = k.ToString();
                            //m_inputKeyboard.registerCommand(k, true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onFire(); }), "fire");
                            m_controls[4] = k.GetHashCode();
                            Debug.WriteLine(m_controls[4]);

                            //m_inputKeyboard.registerCommand((Keys)Enum.ToObject(typeof(Keys), m_controls[4]), true, new InputDeviceHelper.CommandDelegate((gameTime, value) => { GameStateDemo.m_gv.onFire(); }), "fire");
                            fireKey = Enum.ToObject(typeof(Keys), m_controls[4]).ToString();
                            SaveControls();
                        }
                        set = false;
                    }
                }
                
                
                
                
                

                

                //canLoad = true;

            }
        }

        private void LoadControls()
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
                        if (storage.FileExists("Controls.xml"))
                        {
                            using (IsolatedStorageFileStream fs = storage.OpenFile("Controls.xml", FileMode.Open))
                            {
                                if (fs != null)
                                {
                                    XmlSerializer mySerializer = new XmlSerializer(typeof(int[]));
                                    m_loadedControls = (int[])mySerializer.Deserialize(fs);
                                    Debug.WriteLine("should be loaded");
                                    foreach( int i in m_loadedControls)
                                    {
                                        Debug.WriteLine(i);
                                    }
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

        private void SaveControls()
        {
            lock (this)
            {
                if (!this.saving)
                {
                    this.saving = true;

                    FinalizeSaveAsync(m_controls);
                    Debug.WriteLine("should be saved");
                    foreach( int i in m_controls)
                    {
                        Debug.WriteLine(i);
                    }
                }
            }
        }

        private async void FinalizeSaveAsync(int[] controls)
        {
            await Task.Run(() =>
            {
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile("Controls.xml", FileMode.OpenOrCreate))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(int[]));
                                mySerializer.Serialize(fs, controls);
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
    }
}
