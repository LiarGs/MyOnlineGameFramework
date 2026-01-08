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
            _HostButton.onClick.AddListener(OnHostButtonClicked);
            _ClientButton.onClick.AddListener(OnClientButtonClicked);
            _QuitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnDisable()
        {
            _HostButton.onClick.RemoveListener(OnHostButtonClicked);
            _ClientButton.onClick.RemoveListener(OnClientButtonClicked);
            _QuitButton.onClick.RemoveListener(OnQuitButtonClicked);
        }

        #endregion UnityBehavior

        #region PrivateMethods

        private void OnHostButtonClicked()
        {
            G.NetworkManager.StartHost();
            gameObject.SetActive(false);
        }
        
        private void OnClientButtonClicked()
        {
            G.NetworkManager.StartClient();
            gameObject.SetActive(false);
        }

        private static void OnQuitButtonClicked()
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
