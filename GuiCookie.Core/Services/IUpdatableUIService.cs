namespace GuiCookie.Core.Services
{
    public interface IUpdatableUIService : IUIService
    {
        int Order { get; }

        void PreUpdate(TimeSpan elapsedTime, TimeSpan totalTime);
        void Update(TimeSpan elapsedTime, TimeSpan totalTime);
        void PostUpdate(TimeSpan elapsedTime, TimeSpan totalTime);
    }
}
