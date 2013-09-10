namespace Assets.GridLayout
{
    // TODO KPH: This interface isn't quite right:
    // * Render should probably just generically handle layout
    //   by building a Rect() and passing that to a function.
    public interface IGridRenderable
    {
        void Render(int x, int y, int w, int h);
        int Width { get; }
        int Height { get; }
    }
}