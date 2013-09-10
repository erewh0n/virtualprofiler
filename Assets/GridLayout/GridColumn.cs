using System.Collections.Generic;
using System.Linq;
using Assets.SmartMenu;

namespace Assets.GridLayout
{
    public class GridColumn : IGridRenderable
    {
        protected readonly List<IGridRenderable> Renderables;
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Padding Pad { get; set; }

        public GridColumn(IEnumerable<IGridRenderable> renderables, Padding pad = null)
        {
            Renderables = renderables.ToList();
            Pad = pad ?? new Padding(0, 0);
            Width = Renderables.OrderByDescending(x => x.Width).First().Width;
            Height = Renderables.Aggregate(0, (x, y) => y.Height);
        }

        public void Render(int x, int y, int w, int h)
        {
            x += Pad.Left;
            foreach (var renderable in Renderables)
            {
                y += Pad.Top;
                renderable.Render(x, y, Width, renderable.Height);
                y += renderable.Height;
            }
        }

    }
}