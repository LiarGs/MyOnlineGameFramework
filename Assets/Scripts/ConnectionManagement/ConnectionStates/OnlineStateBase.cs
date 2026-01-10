namespace ConnectionManagement.ConnectionStates
{
    /// <summary>
    /// Base class representing an online connection state.
    /// </summary>
    abstract class OnlineStateBase : ConnectionStateBase
    {
        #region PublicMethods

        public override void OnUserRequestedShutdown()
        {
            // This behaviour will be the same for every online state
            _ConnectStatusPublisher.Publish(ConnectStatus.UserRequestedDisconnect);
            _ConnectionManager.ChangeState(_ConnectionManager.Offline);
        }

        public override void OnTransportFailure()
        {
            // This behaviour will be the same for every online state
            _ConnectionManager.ChangeState(_ConnectionManager.Offline);
        }

        #endregion PublicMethods
    }
}