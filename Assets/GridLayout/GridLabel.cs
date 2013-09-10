using System;
using UnityEngine;

namespace Assets.GridLayout
{
    public class GridDraw : IGridRenderable
    {
        private readonly Action<Rect> _vectorsFn;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public GridDraw(int x, int y, Action<Rect> vectorsFn)
        {
            _vectorsFn = vectorsFn;
            Width = x;
            Width = y;
        }

        public void Render(int x, int y, int w, int h)
        {
            _vectorsFn.Invoke(new Rect(x, y, w, h));
        }

    }

    public class GridLabel : IGridRenderable
    {
        private readonly Func<string> _getText;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public GridLabel(Func<string> getText, string minStrut = null)
        {
            if (getText == null) throw new ArgumentNullException("getText");
            _getText = getText;

            var vector = GUI.skin.label.CalcSize(new GUIContent(minStrut ?? getText.Invoke()));
            Width = (int)vector.x;
            Height = (int)vector.y;
        }

        public void Render(int x, int y, int w, int h)
        {
            GUI.Label(new Rect(x, y, Width, Height), _getText.Invoke());
        }

    }
}