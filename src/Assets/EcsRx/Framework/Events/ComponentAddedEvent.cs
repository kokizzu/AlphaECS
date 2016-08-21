namespace EcsRx
{
    public class ComponentAddedEvent
    {
        public IEntity Entity { get; private set; }
        public IComponent Component { get; private set; }

        public ComponentAddedEvent(IEntity entity, IComponent component)
        {
            Entity = entity;
            Component = component;
        }
    }
}