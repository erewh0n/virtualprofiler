using System.Collections.Generic;
using UnityEngine;

namespace Assets.GridLayout
{
    public class Grid
    {
        private int _xOffset;
        private int _yOffset;
        private int _width;
        private int _height;
        private List<IGridRenderable> _renderables;

        public Grid(int xOffset, int yOffset, int width, int height)
        {
            _xOffset = xOffset;
            _yOffset = yOffset;
            _width = width;
            _height = height;
            _renderables = new List<IGridRenderable>();
        }

        public Grid Add(IEnumerable<IGridRenderable> renderables)
        {
            _renderables.AddRange(renderables);

            return this;
        }

        public void Render()
        {
            GUI.BeginGroup(new Rect(_xOffset, _yOffset, _width, _height));
            GUI.Box(new Rect(0, 0, _width, _height), "");

            foreach (var renderable in _renderables)
            {
                renderable.Render(0, 0, 0, 0);
            }

            GUI.EndGroup();
        }
    }
}