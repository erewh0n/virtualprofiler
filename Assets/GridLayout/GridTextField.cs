using System;
using UnityEngine;

namespace Assets.GridLayout
{
    public class GridTextField : IGridRenderable
    {
        private string _currentText = "";
        private readonly Func<string, string> _getText;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public GridTextField(Func<string, string> getText, string spacing)
        {
            if (getText == null) throw new ArgumentNullException("getText");
            _getText = getText;

            var vector = GUI.skin.label.CalcSize(new GUIContent(spacing));
            Width = (int)vector.x;
            Height = (int)vector.y;
        }

        public void Render(int x, int y, int w, int h)
        {
            _currentText = _getText.Invoke(_currentText);
            _currentText = GUI.TextField(new Rect(x, y, w, h), _currentText);
        }
    }
}