using System;
using System.Collections.Generic;
using GamePlay.Action;
using GamePlay.Capabilities;
using UnityEngine;

namespace GamePlay.Objects.Actors
{
    public class Brain : MonoBehaviour
    {
        #region UnityBehaviour

        private void Awake()
        {
            var capabilities = GetComponentsInChildren<CapabilityBase>();
            foreach (var capability in capabilities)
            {
                _CapabilityMap[capability.GetType()] = capability;
            }

            _ResetController();
        }

        private void Update()
        {
            _ExecuteCommands();
        }

        private void OnDestroy()
        {
            _ActorController.Dispose();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                _ResetController();
            }
        }

        #endregion UnityBehaviour

        #region PublicMethods

        public T GetCapability<T>() where T : CapabilityBase
        {
            if (_CapabilityMap.TryGetValue(typeof(T), out var capability))
            {
                return (T)capability;
            }

            return null;
        }

        public void AddCapability(CapabilityBase capability)
        {
            _CapabilityMap[capability.GetType()] = capability;
        }

        public void RemoveCapability(CapabilityBase capability)
        {
            _CapabilityMap.Remove(capability.GetType());
        }

        public void AddCommand(CommandBase command)
        {
            _CommandStream.Enqueue(command);
        }

        public void ExecuteCommand(CommandBase command)
        {
            command.Execute(this);
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _ExecuteCommands()
        {
            while (_CommandStream.Count > 0)
            {
                ExecuteCommand(_CommandStream.Dequeue());
            }
        }

        private void _ResetController()
        {
            if (_ActorController != null)
            {
                _ActorController.Dispose();
                _ActorController = null;
            }

            switch (_ControlBy)
            {
                case ControlType.Player:
                    _ActorController = new PlayerController(this);
                    break;
                case ControlType.AI:
                    _ActorController = new AIController(this);
                    break;
                case ControlType.None:
                default:
                    break;
            }
        }

        #endregion PrivateMethods

        #region Porperties

        public ControlType ControlBy
        {
            get => _ControlBy;
            set
            {
                if (_ControlBy == value) return;

                _ControlBy = value;
                _ResetController();
            }
        }

        #endregion Porperties

        #region Fields

        public Animator            ActorAnimator;
        public CharacterController ActorCharacterController;
        public Rigidbody           Rigidbody;

        [SerializeField] private ControlType _ControlBy = ControlType.None;
        private ActorControllerBase _ActorController;
        private Dictionary<Type, CapabilityBase> _CapabilityMap = new Dictionary<Type, CapabilityBase>();
        private Queue<CommandBase> _CommandStream = new Queue<CommandBase>();

        #endregion Fields
    }

    public enum ControlType
    {
        None,
        Player,
        AI
    }
}