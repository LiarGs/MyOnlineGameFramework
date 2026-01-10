using ApplicationLifecycle.Messages;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class MainMenuController : MonoBehaviour
    {
        #region UnityBehavior

        private void OnEnable()
        {
            _HostButton.onClick.AddListener(_OnHostButtonClicked);
            _ClientButton.onClick.AddListener(_OnClientButtonClicked);
            _QuitButton.onClick.AddListener(_OnQuitButtonClicked);
        }

        private void OnDisable()
        {
            _HostButton.onClick.RemoveListener(_OnHostButtonClicked);
            _ClientButton.onClick.RemoveListener(_OnClientButtonClicked);
            _QuitButton.onClick.RemoveListener(_OnQuitButtonClicked);
        }

        #endregion UnityBehavior

        #region PrivateMethods

        private void _OnHostButtonClicked()
        {
            G.ConnectionManager.StartHostIp("server", "127.0.0.1", 9998);
        }

        private void _OnClientButtonClicked()
        {
            G.ConnectionManager.StartClientIp("client", "127.0.0.1", 9998);
        }

        private static void _OnQuitButtonClicked()
        {
            G.MessageChannels.QuitMessageChannel.Publish(new QuitApplicationMessage());
        }

        #endregion PrivateMethods

        #region Fields

        [SerializeField] private Button _HostButton;
        [SerializeField] private Button _ClientButton;
        [SerializeField] private Button _QuitButton;

        #endregion Fields
    }
}