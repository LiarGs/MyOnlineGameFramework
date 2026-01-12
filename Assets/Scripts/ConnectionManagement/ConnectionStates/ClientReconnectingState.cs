using System.Collections;
using Infrastructure.PubSub;
using UnityEngine;
using Utils;

namespace ConnectionManagement.ConnectionStates
{
    /// <summary>
    /// Connection state corresponding to a client attempting to reconnect to a server. It will try to reconnect a
    /// number of times defined by the ConnectionManager's NbReconnectAttempts property. If it succeeds, it will
    /// transition to the ClientConnected state. If not, it will transition to the Offline state. If given a disconnect
    /// reason first, depending on the reason given, may not try to reconnect again and transition directly to the
    /// Offline state.
    /// </summary>
    class ClientReconnectingState : ClientConnectingState
    {
        #region PublicMethods

        public override void Enter()
        {
            _NbAttempts                 = 0;
            _ReconnectCoroutineInstance = _ConnectionManager.StartCoroutine(_ReconnectCoroutine());
        }

        public override void Exit()
        {
            if (_ReconnectCoroutineInstance != null)
            {
                _ConnectionManager.StopCoroutine(_ReconnectCoroutineInstance);
                _ReconnectCoroutineInstance = null;
            }
            // _ReconnectMessagePublisher.Publish(new ReconnectMessage(_ConnectionManager.NbReconnectAttempts, _ConnectionManager.NbReconnectAttempts));
        }

        public override void OnClientConnected(ulong _)
        {
            _ConnectionManager.ChangeState(_ConnectionManager.ClientConnected);
        }

        public override void OnClientDisconnect(ulong _)
        {
            var disconnectReason = G.NetworkManager.DisconnectReason;
            if (_NbAttempts < _ConnectionManager.NbReconnectAttempts)
            {
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    _ReconnectCoroutineInstance = _ConnectionManager.StartCoroutine(_ReconnectCoroutine());
                }
                else
                {
                    var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    _ConnectStatusPublisher.Publish(connectStatus);
                    switch (connectStatus)
                    {
                        case ConnectStatus.UserRequestedDisconnect:
                        case ConnectStatus.HostEndedSession:
                        case ConnectStatus.ServerFull:
                        case ConnectStatus.IncompatibleBuildType:
                            _ConnectionManager.ChangeState(_ConnectionManager.Offline);
                            break;
                        default:
                            _ReconnectCoroutineInstance = _ConnectionManager.StartCoroutine(_ReconnectCoroutine());
                            break;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    _ConnectStatusPublisher.Publish(ConnectStatus.GenericDisconnect);
                }
                else
                {
                    var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    _ConnectStatusPublisher.Publish(connectStatus);
                }

                _ConnectionManager.ChangeState(_ConnectionManager.Offline);
            }
        }

        #endregion PublicMethods

        #region PrivateMethods

        private IEnumerator _ReconnectCoroutine()
        {
            // If not on first attempt, wait some time before trying again, so that if the issue causing the disconnect
            // is temporary, it has time to fix itself before we try again. Here we are using a simple fixed cooldown
            // but we could want to use exponential backoff instead, to wait a longer time between each failed attempt.
            // See https://en.wikipedia.org/wiki/Exponential_backoff
            if (_NbAttempts > 0)
            {
                yield return new WaitForSeconds(_TimeBetweenAttempts);
            }

            Debug.Log("Lost connection to host, trying to reconnect...");

            G.NetworkManager.Shutdown();

            yield return
                new WaitWhile(() =>
                    G.NetworkManager.ShutdownInProgress); // wait until NetworkManager completes shutting down
            Debug.Log($"Reconnecting attempt {_NbAttempts + 1}/{_ConnectionManager.NbReconnectAttempts}...");
            // _ReconnectMessagePublisher.Publish(new ReconnectMessage(_NbAttempts, _ConnectionManager.NbReconnectAttempts));

            // If first attempt, wait some time before attempting to reconnect to give time to services to update
            // (i.e. if in a Lobby and the host shuts down unexpectedly, this will give enough time for the lobby to be
            // properly deleted so that we don't reconnect to an empty lobby
            if (_NbAttempts == 0)
            {
                yield return new WaitForSeconds(_TimeBeforeFirstAttempt);
            }

            _NbAttempts++;
            var reconnectingSetupTask = _ConnectionMethod.SetupClientReconnectionAsync();
            yield return new WaitUntil(() => reconnectingSetupTask.IsCompleted);

            if (!reconnectingSetupTask.IsFaulted && reconnectingSetupTask.Result.success)
            {
                // If this fails, the OnClientDisconnect callback will be invoked by Netcode
                var connectingTask = ConnectClientAsync();
                yield return new WaitUntil(() => connectingTask.IsCompleted);
            }
            else
            {
                if (!reconnectingSetupTask.Result.shouldTryAgain)
                {
                    // setting number of attempts to max so no new attempts are made
                    _NbAttempts = _ConnectionManager.NbReconnectAttempts;
                }

                // Calling OnClientDisconnect to mark this attempt as failed and either start a new one or give up
                // and return to the Offline state
                OnClientDisconnect(0);
            }
        }

        #endregion PrivateMethods

        #region Fields

        // [Inject] private IPublisher<ReconnectMessage> _ReconnectMessagePublisher;
        private       Coroutine _ReconnectCoroutineInstance;
        private       int       _NbAttempts;
        private const float     _TimeBeforeFirstAttempt = 1;
        private const float     _TimeBetweenAttempts    = 5;

        #endregion Fields
    }
}