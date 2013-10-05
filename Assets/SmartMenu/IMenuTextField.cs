namespace Assets.SmartMenu
{
    public interface IMenuTextField
    {
        bool UpdateAndValidate();
        void Update();
        string Name { get; }
        string Description { get; }
    }
}