using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace stopInGreen.Input
{
    /// <summary>
    /// Derived input device for the PC Keyboard
    /// </summary>
    public class KeyboardInput : IInputDevice
    {
        /// <summary>
        /// Registers a callback-based command
        /// </summary>
        public void registerCommand(Keys key, bool keyPressOnly, InputDeviceHelper.CommandDelegate callback, string movement)
        {
            //
            // If already registered, remove it!
            if (m_commandEntries.ContainsKey(key))
            {
                m_commandEntries.Remove(key);
            }
            foreach (var cb in m_commandEntries)
            {
                if (cb.Value.movement.Equals(movement))
                {
                    m_commandEntries.Remove(cb.Key);
                }
            }
            m_commandEntries.Add(key, new CommandEntry(key, keyPressOnly, callback, movement));
        }

        /// <summary>
        /// Track all registered commands in this dictionary
        /// </summary>
        private Dictionary<Keys, CommandEntry> m_commandEntries = new Dictionary<Keys, CommandEntry>();

        /// <summary>
        /// Used to keep track of the details associated with a command
        /// </summary>
        private struct CommandEntry
        {
            public CommandEntry(Keys key, bool keyPressOnly, InputDeviceHelper.CommandDelegate callback, string movement)
            {
                this.key = key;
                this.keyPressOnly = keyPressOnly;
                this.callback = callback;
                this.movement = movement;
            }

            public Keys key;
            public bool keyPressOnly;
            public InputDeviceHelper.CommandDelegate callback;
            public string movement;
        }

        /// <summary>
        /// Goes through all the registered commands and invokes the callbacks if they
        /// are active.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            foreach (CommandEntry entry in this.m_commandEntries.Values)
            {
                if (entry.keyPressOnly && keyPressed(entry.key))
                {
                    entry.callback(gameTime, 1.0f);
                }
                else if (!entry.keyPressOnly && state.IsKeyDown(entry.key))
                {
                    entry.callback(gameTime, 1.0f);
                }
            }

            //
            // Move the current state to the previous state for the next time around
            m_statePrevious = state;
        }

        private KeyboardState m_statePrevious;

        /// <summary>
        /// Checks to see if a key was newly pressed
        /// </summary>
        private bool keyPressed(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && !m_statePrevious.IsKeyDown(key));
        }
    }
}
