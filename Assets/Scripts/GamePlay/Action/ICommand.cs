using GamePlay.Objects.Actors;

namespace GamePlay.Action
{
    public interface ICommand
    {
        public void Execute(Brain executor);
    }
}