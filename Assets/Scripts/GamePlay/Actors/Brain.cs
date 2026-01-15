using System;
using System.Collections.Generic;
using GameData.Actors;
using GamePlay.Action;
using GamePlay.Capabilities;
using GamePlay.Managers;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Actors
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
            _CurrentActorController?.Dispose();
            base.OnDestroy();
        }

//         private void OnValidate()
//         {
//             if (!Application.isPlaying) return;
//
// #if UNITY_EDITOR
//             EditorApplication.delayCall += () =>
//             {
//                 // 再次检查空引用，防止在这个等待期间物体被删了
//                 if (this == null) return;
//
//                 _ResetController();
//             };
// #endif
//         }

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

        [ContextMenu("Reset Controller")]
        private void _ResetController()
        {
            _CurrentActorController?.Dispose();

            _CurrentActorController = ActorControllerConfig?.CreateActorController(this);
            _CurrentActorController?.Init();
        }

        #endregion PrivateMethods

        #region Fields

        public ActorControllerConfigBase ActorControllerConfig;
        public Transform                 LookAtPos;
        public CharacterController       ActorCharacterController;

        private          ActorControllerBase              _CurrentActorController;
        private readonly Dictionary<Type, CapabilityBase> _CapabilityMap = new Dictionary<Type, CapabilityBase>();
        private readonly Queue<ICommand>                  _CommandStream = new Queue<ICommand>();

        #endregion Fields
    }
}