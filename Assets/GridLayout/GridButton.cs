using System;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.GridLayout
{
    public class GridButton : IGridRenderable
    {
        private readonly Action _buttonAction;
        private readonly string _text;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public GridButton(string text, Action buttonAction)
        {
            _text = text;
            _buttonAction = buttonAction;
            var vector = GUI.skin.button.CalcSize(new GUIContent(text));
            Width = (int) vector.x;
            Height = (int) vector.y;
        }

        public GridButton(string text, int width, int height, Action buttonAction)
            : this(text, buttonAction)
        {
            Width = width;
            Height = height;
        }

        public void Render(int x, int y, int w, int h)
        {
            if (GUI.Button(new Rect(x, y, Width, Height), _text))
            {
                _buttonAction.Invoke();
            }
        }

    }
}