using GamePlay.Actors;

namespace GamePlay.Action
{
    public interface ICommand
    {
        public void Execute(Brain executor);
    }
}