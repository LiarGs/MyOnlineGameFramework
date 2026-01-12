using System;
using System.Collections.Generic;
using GamePlay.Action;
using GamePlay.Capabilities;
using Unity.Netcode;
using UnityEngine;

namespace GamePlay.Objects.Actors
{
    public class Brain : NetworkBehaviour
    {
        #region UnityBehaviour

        private void Awake()
        {
            var capabilities = GetComponentsInChildren<CapabilityBase>();
            foreach (var capability in capabilities)
            {
                _CapabilityMap[capability.GetType()] = capability;
            }

            AnimatorManager = new ActorAnimatorManager(ActorAnimator);
        }

        public override void OnNetworkSpawn()
        {
            _ResetController();
        }

        private void Update()
        {
            _ExecuteCommands();
        }

        public override void OnDestroy()
        {
            _ActorController.Dispose();
            base.OnDestroy();
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

            throw new Exception($"Capability not found in {gameObject.name}.");
        }

        public void AddCapability(CapabilityBase capability)
        {
            _CapabilityMap[capability.GetType()] = capability;
        }

        public void RemoveCapability(CapabilityBase capability)
        {
            _CapabilityMap.Remove(capability.GetType());
        }

        public void AddCommand(ICommand command)
        {
            _CommandStream.Enqueue(command);
        }

        public void ExecuteCommand(ICommand command)
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

        public Transform            LookAtPos;
        public Animator             ActorAnimator;
        public CharacterController  ActorCharacterController;
        public Rigidbody            Rigidbody;
        public ActorAnimatorManager AnimatorManager;

        [SerializeField] private ControlType         _ControlBy = ControlType.None;
        private                  ActorControllerBase _ActorController;

        private readonly Dictionary<Type, CapabilityBase> _CapabilityMap = new Dictionary<Type, CapabilityBase>();
        private readonly Queue<ICommand>                  _CommandStream = new Queue<ICommand>();

        #endregion Fields
    }

    public enum ControlType
    {
        None,
        Player,
        AI
    }
}