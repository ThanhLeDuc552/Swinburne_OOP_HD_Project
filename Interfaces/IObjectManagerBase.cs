namespace Swinburne_OOP_HD
{
    public interface IObjectManagerBase
    {
        int Count { get; }
        void DrawAll();
        void UpdateAll();
    }
}