using GamePlay.Action;
using UnityEngine;
using Utils;

namespace GamePlay.Actors
{
    public class TestAIController : ActorControllerBase
    {
        #region PublicMethods

        public TestAIController(Brain actorBrain) : base(actorBrain)
        {
            if (!_ActorBrain.IsOwner) return;
            G.UpdateRunner.Subscribe(_Tick, 0);
            Debug.Log("AI Controller initialized.");
        }

        public override void Dispose()
        {
            G.UpdateRunner.Unsubscribe(_Tick);
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _Tick(float deltaTime)
        {
            _HandleMove();
        }

        private void _HandleMove()
        {
            var randomPoint   = Random.insideUnitCircle.normalized;
            var moveDirection = new Vector3(randomPoint.x, 0, randomPoint.y);

            _ActorBrain.ExecuteCommand(new MoveCommand(moveDirection, 1f));
        }

        #endregion PrivateMethods
    }
}