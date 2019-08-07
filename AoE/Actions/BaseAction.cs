namespace AoE.Actions
{
    abstract class BaseAction
    {
        public abstract void Do(float dt);
        public abstract bool Completed();
    }
}