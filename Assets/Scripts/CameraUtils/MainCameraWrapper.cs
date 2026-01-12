using Cinemachine;
using UnityEngine;

namespace CameraUtils
{
    public class MainCameraWrapper : MonoBehaviour
    {
        #region UnityBehavior

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance.gameObject);
            }

            Instance = this;
        }

        private void Start()
        {
            MainCamera = Camera.main;
        }

        #endregion UnityBehavior

        #region Porperties

        public Camera MainCamera { get; set; }

        #endregion Porperties

        #region Fields

        public static MainCameraWrapper Instance;

        public CinemachineVirtualCamera CineMachineVirtualCamera;

        #endregion Fields
    }
}