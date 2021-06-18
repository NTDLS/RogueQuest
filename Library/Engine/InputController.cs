using Library.Types;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Library.Engine
{
    public class InputController
    {
        public EngineCoreBase Core { get; set; }
        private Dictionary<PlayerKey, KeyPressState> _keyStates = new Dictionary<PlayerKey, KeyPressState>();

        public InputController(EngineCoreBase core)
        {
            Core = core;
        }

        public bool IsKeyPressed(PlayerKey key)
        {
            if (_keyStates.ContainsKey(key))
            {
                return (_keyStates[key] == KeyPressState.Down);
            }

            return false;
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChanged(PlayerKey key, KeyPressState state)
        {
            if (_keyStates.ContainsKey(key))
            {
                _keyStates[key] = state;
            }
            else
            {
                _keyStates.Add(key, state);
            }
        }

        public void HandleSingleKeyPress(Keys key)
        {
            Core.HandleSingleKeyPress(key);
            #region Debug.
            if (key == Keys.D0)
            {
            }
            else if (key == Keys.D1)
            {
            }
            else if (key == Keys.D2)
            {
            }
            else if (key == Keys.D3)
            {
            }
            else if (key == Keys.D4)
            {
            }
            else if (key == Keys.D5)
            {
            }
            else if (key == Keys.D6)
            {
            }
            else if (key == Keys.D7)
            {
            }
            else if (key == Keys.D8)
            {
            }
            else if (key == Keys.D9)
            {
            }
            else if (key == Keys.F1)
            {
            }
            else if (key == Keys.F12)
            {
            }
            else if (key == Keys.Escape)
            {
            }
            #endregion
        }
    }
}
