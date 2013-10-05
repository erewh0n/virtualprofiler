using System.Collections.Generic;
using System.Linq;
using Assets.SmartMenu;

namespace Assets.GridLayout
{
    public class GridRow : IGridRenderable
    {
        protected readonly List<IGridRenderable> Renderables;
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Padding Pad { get; set; }

        public GridRow(IEnumerable<IGridRenderable> renderables, Padding pad = null)
        {
            Renderables = renderables.ToList();
            Pad = pad ?? new Padding(0, 0);
            Width = Renderables.Aggregate(0, (x, y) => y.Width);
            Height = Renderables.OrderBy(x => x.Height).First().Height;
        }

        public void Render(int x, int y, int w, int h)
        {
            y += Pad.Top;
            foreach (var renderable in Renderables)
            {
                x += Pad.Left;
                renderable.Render(x, y, w == 0 ? renderable.Width : w, h == 0 ? renderable.Height : h);
                x += renderable.Width;
            }
        }
    }
}