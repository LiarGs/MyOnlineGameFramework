using System;
using System.Collections;
using ApplicationLifecycle.Messages;
using Infrastructure;
using Infrastructure.PubSub;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace ApplicationLifecycle
{
    public class ApplicationController:MonoBehaviour
    {
        #region UnityBehavior
        
        private void Start()
        {
            Application.wantsToQuit += _OnWantToQuit;
            _Subscription = G.MessageChannels.QuitMessageChannel.Subscribe(_QuitGame);
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_UpdateRunner.gameObject);
            Application.targetFrameRate = 120;
            SceneManager.LoadScene(MainSceneName);
        }
        
        protected void OnDestroy()
        {
            _Subscription?.Dispose();
        }
        
        #endregion UnityBehavior

        #region PrivateMethods

        /// <summary>
        ///     In builds, if we are in a lobby and try to send a Leave request on application quit, it won't go through if we're quitting on the same frame.
        ///     So, we need to delay just briefly to let the request happen (though we don't need to wait for the result).
        /// </summary>
        private IEnumerator _LeaveBeforeQuit()
        {
            // We want to quit anyway, so if anything happens while trying to leave the Lobby, log the exception then carry on
            try
            {
                // _LobbyServiceFacade.EndTracking();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            yield return null;
            Application.Quit();
        }

        private bool _OnWantToQuit()
        {
            Application.wantsToQuit -= _OnWantToQuit;
            
            var canQuit = true;
            // var canQuit = _LocalLobby != null && string.IsNullOrEmpty(_LocalLobby.LobbyID);
            if (!canQuit)
            {
                StartCoroutine(_LeaveBeforeQuit());
            }
            
            return canQuit;
        }

        private static void _QuitGame(QuitApplicationMessage msg)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion PrivateMethods
       
        #region Fields
        
        public  string             MainSceneName =  "MainMenu";
        [SerializeField] private UpdateRunner       _UpdateRunner;
        private                  IDisposable        _Subscription;
        
        #endregion Fields
    }
}
