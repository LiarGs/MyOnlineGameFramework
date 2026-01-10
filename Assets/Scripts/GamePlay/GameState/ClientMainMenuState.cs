using System;
using UnityEngine;

namespace GamePlay.GameState
{
    /// <summary>
    /// Game Logic that runs when sitting at the MainMenu. This is likely to be "nothing", as no game has been started. But it is
    /// nonetheless important to have a game state, as the GameStateBehaviour system requires that all scenes have states.
    /// </summary>
    /// <remarks> OnNetworkSpawn() won't ever run, because there is no network connection at the main menu screen.
    /// Fortunately we know you are a client, because all players are clients when sitting at the main menu screen.
    /// </remarks>
    public class ClientMainMenuState : GameStateBehaviour
    {
        #region UnityBehavior

        protected void Awake()
        {
            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                _OnSignInFailed();
                return;
            }

            // _TrySignIn();
        }

        #endregion UnityBehavior

        #region PrivateMethods

        private void _TrySignIn()
        {
            try
            {
                _OnAuthSignIn();
            }
            catch (Exception)
            {
                _OnSignInFailed();
            }
        }

        private void _OnAuthSignIn()
        {
        }

        private void _OnSignInFailed()
        {
            Debug.Log("SignIn failed");
        }

        #endregion PrivateMethods

        #region Porperties

        public override GameState ActiveState => GameState.MainMenu;

        #endregion Porperties
    }
}