using UnityEngine;
using UnityEngine.InputSystem;

namespace GamePlay.UserInput
{
    public class UserInput : MonoBehaviour
    {
        #region UnityBehavior

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance.gameObject);
            }

            Instance         = this;
            _InputController = new UserInputController();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _InputController.Enable();
            _InputController.GamePlay.Move.performed     += _OnMovePerformed;
            _InputController.GamePlay.Move.canceled      += _OnMoveCanceled;
            _InputController.GamePlay.Look.performed     += _OnLookPerformed;
            _InputController.GamePlay.Interact.performed += _OnInteractPerformed;
            _InputController.UI.Cancel.performed         += _OnCancelPerformed;
        }

        private void OnDisable()
        {
            _InputController.GamePlay.Move.performed     -= _OnMovePerformed;
            _InputController.GamePlay.Move.canceled      -= _OnMoveCanceled;
            _InputController.GamePlay.Look.performed     -= _OnLookPerformed;
            _InputController.GamePlay.Interact.performed -= _OnInteractPerformed;
            _InputController.UI.Cancel.performed         -= _OnCancelPerformed;
            _InputController.Disable();
            _MovementInput = Vector2.zero;
            MoveAmount     = 0;
        }

        #endregion UnityBehavior

        #region PrivateMethods

        #region HandleLocomotion

        private void _OnMovePerformed(InputAction.CallbackContext context)
        {
            _MovementInput = context.ReadValue<Vector2>();
            MoveAmount     = Mathf.Clamp01(Mathf.Abs(VerticalInput) + Mathf.Abs(HorizontalInput));
            MoveAmount = MoveAmount switch
            {
                > 0 and <= 0.5f => 0.5f,
                > 0.5f and < 1  => 1f,
                _               => MoveAmount
            };

            _CheckInputDeviceChange(context);
        }

        private void _OnMoveCanceled(InputAction.CallbackContext context)
        {
            _MovementInput = Vector2.zero;
            MoveAmount     = 0;

            _CheckInputDeviceChange(context);
        }

        #endregion HandleLocomotion

        private void _OnLookPerformed(InputAction.CallbackContext context)
        {
            _CameraInput = context.ReadValue<Vector2>();
            _CheckInputDeviceChange(context);
        }

        private void _OnInteractPerformed(InputAction.CallbackContext context)
        {
            // TODO: 互动逻辑
            _CheckInputDeviceChange(context);
        }

        #region HanldeUI

        private void _OnPausePerformed(InputAction.CallbackContext context)
        {
            // TODO: 暂停逻辑
            _CheckInputDeviceChange(context);
        }

        private void _OnCancelPerformed(InputAction.CallbackContext context)
        {
            // TODO: 暂停逻辑
            _CheckInputDeviceChange(context);
        }

        #endregion HanldeUI

        private void _CheckInputDeviceChange(InputAction.CallbackContext context)
        {
            if (_CurrentControllerDevice == null || _CurrentControllerDevice != context.control.device)
            {
                _CurrentControllerDevice = context.control.device;
            }
        }

        #endregion PrivateMethods

        #region Porperties

        public float MoveAmount { get; private set; }

        public float VerticalInput   => _MovementInput.y;
        public float HorizontalInput => _MovementInput.x;

        public float CameraVerticalInput   => _CameraInput.y;
        public float CameraHorizontalInput => _CameraInput.x;

        #endregion Porperties

        #region Fields

        internal static UserInput Instance;

        private UserInputController _InputController;
        private InputDevice         _CurrentControllerDevice;

        private Vector2 _MovementInput;
        private Vector2 _CameraInput;

        #endregion Fields
    }
}