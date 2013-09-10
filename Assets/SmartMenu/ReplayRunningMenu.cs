using System;
using System.Collections.Generic;
using Assets.GridLayout;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class ColorPoint
    {
        public ColorPoint(float x, float y, float r, float g, float b)
        {
            Position = new Vector2(x, y);
            Color = new Color(r, g, b);
        }

        public ColorPoint(Vector2 position, Color color)
        {
            Position = position;
            Color = color;
        }

        public Vector2 Position { get; set; }
        public Color Color { get; set; }
    }

    public class StatsView
    {
        private readonly ReplayStats _replayStats;
        private readonly Vector2 _dimensions;
        private readonly Dictionary<string, List<ColorPoint>> _graphLayers;

        public StatsView(ReplayStats replayStats, Rect rect)
        {
            _replayStats = replayStats;
            _dimensions = new Vector2(
                rect.width  < 50 ? 50 : rect.width,
                rect.height < 50 ? 50 : rect.height);
            _graphLayers = new Dictionary<string, List<ColorPoint>>
                {
                    { "base",         TextureBase() },
                    { "grid",         TextureGrid() },
                    { "fullVelocity", TextureFullVelocityLine() },
                };
        }

        private List<ColorPoint> TextureBase()
        {
            var points = new List<ColorPoint>();
            for (var i = 0; i < _dimensions.y; i++)
                for (var j = 0; j < _dimensions.x; j++)
                    points.Add(new ColorPoint(j, i, 0, 0, 0));

            return points;
        }

        public List<ColorPoint> TextureGrid()
        {
            var points = new List<ColorPoint>();

            for (var x = 10; x < (_dimensions.x - 10); x++)
                points.Add(new ColorPoint(x, _dimensions.y - 10, 1, 1, 1));

            for (var y = 10; y < (_dimensions.y - 10); y++)
                points.Add(new ColorPoint(10, y, 1, 1, 1));

            return points;
        }

        public List<ColorPoint> TextureFullVelocityLine()
        {
            var points = new List<ColorPoint>();

            var x0 = 10;
            var y0 = 10;
            var x1 = _dimensions.x - 10;
            var y1 = _dimensions.y - 10;
            var velocities = _replayStats.Velocities;
            var xScale = velocities.Count / (x1 - x0);
            for (var x = 0; x < (x1 - x0); x++)
            {
                var index = xScale * x;
                var nV = velocities[(int)index] / _replayStats.MaxVelocity;
                points.Add(new ColorPoint(x, (int)(y0 + (y1 - y0) * nV), 0, 0.2f, 1.0f));
            }

            return points;
        }
        
        public void DrawGraph(Rect rect)
        {
            
        }

    }

    public class GridItem : IGridRenderable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private readonly Action<Rect> _renderable;

        public GridItem(Action<Rect> renderable, int width, int height)
        {
            _renderable = renderable;
            Width = width;
            Height = height;
        }

        public void Render(int x, int y, int w, int h)
        {
            _renderable.Invoke(new Rect(x, y, w, h));
        }
    }

    public class ReplayRunningMenu : ISmartMenu
    {
        private Grid _grid;
        private Grid _statsView;
        private ISmartMenu _nextMenu;

        public ReplayRunningMenu()
        {
            var statusTexture = new Texture2D(1, 1);
            statusTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f));
            statusTexture.Apply();
            var style = new GUIStyle(GUI.skin.textArea) { normal = { background = statusTexture }, alignment = TextAnchor.MiddleCenter };
        }

        private void BuildStatsView()
        {
            if (_statsView != null) return;

            var statsEngine = Global.ReplayManager.GetStatsEngine();

            _statsView = new Grid(130, 5, 400, 100);
            _statsView.Add(new[]
                {
                    new GridColumn(new List<IGridRenderable>
                        {
                            new GridRow(new List<IGridRenderable>
                                {
                                            new GridLabel(() => string.Format("% Complete:     {0:f2}", 100*statsEngine.Elapsed.TotalMilliseconds/ statsEngine.TotalTime.TotalMilliseconds)),
                                }),
                            new GridRow(new List<IGridRenderable>
                                {
                                    new GridColumn(new List<IGridRenderable>
                                        {
                                            new GridRow(new List<IGridRenderable>
                                                {
                                                    new GridLabel(() => "Elapsed:", "0123-0123-01"),
                                                    new GridLabel(() => string.Format("{0:f2}", statsEngine.Elapsed)),
                                                }),
                                            new GridRow(new List<IGridRenderable>
                                                {
                                                    new GridLabel(() => "Start time:", "0123-0123-01"),
                                                    new GridLabel(() => string.Format("{0}", statsEngine.StartTime)),
                                                }),
                                            new GridRow(new List<IGridRenderable>
                                                {
                                                    new GridLabel(() => "End time:", "0123-0123-01"),
                                                    new GridLabel(() => string.Format("{0}", statsEngine.EndTime)),
                                                }),
                                            new GridRow(new List<IGridRenderable>
                                                {
                                                    new GridLabel(() => "Total Time:", "0123-0123-01"),
                                                    new GridLabel(() => string.Format("{0:f2}", statsEngine.TotalTime)),
                                                }),
                                        }),
                                    new GridColumn(new List<IGridRenderable>
                                        {
                                            new GridRow(new List<IGridRenderable>
                                                {
                                                    new GridLabel(() => "Current velocity (u/s):", "0123-0123-0123-0123-0"),
                                                    new GridLabel(() => string.Format("{0:f4}", statsEngine.CurrentVelocity)),
                                                }),
                                            new GridRow(new List<IGridRenderable>
                                                {
                                                    new GridLabel(() => "Minimum velocity (u/s):", "0123-0123-0123-0123-0"),
                                                    new GridLabel(() => string.Format("{0:f4}", statsEngine.MinVelocity)),
                                                }),
                                            new GridRow(new List<IGridRenderable>
                                                {
                                                    new GridLabel(() => "Maximum velocity (u/s):", "0123-0123-0123-0123-0"),
                                                    new GridLabel(() => string.Format("{0:f4}", statsEngine.MaxVelocity)),
                                                }),
                                        }, new Padding(100, 0)),
                                }),
                        }, new Padding(5, 5)),
                });
        }

        public ISmartMenu CreateMenu()
        {
            _nextMenu = this;

            var statsEngine = Global.ReplayManager.GetStatsEngine();
            BuildStatsView();

            if (_grid == null)
            {
                _grid = new Grid(5, 5, 120, 100);
                _grid.Add(new[]
                    {
                        new GridColumn(new List<IGridRenderable>
                            {
                                new GridColumn(new List<IGridRenderable>
                                    {
                                        new GridRow(new List<IGridRenderable>
                                            {
                                                new GridButton("<<", () => { }),
                                                new GridButton("< ",  () => { }),
                                                new GridButton("PP",  () => Global.ReplayManager.PlayPause()),
                                                new GridButton(" >",  () => { }),
                                                new GridButton(">>", () => { }),
                                            }),
                                        new GridRow(new List<IGridRenderable>
                                            {
                                                new GridItem(rect =>
                                                        GUI.HorizontalSlider(rect, statsEngine.CurrentSegment, 0,
                                                                             statsEngine.TotalSegments), 100, 15),
                                            })
                                    }),
                                new GridColumn(new List<IGridRenderable>
                                    {
                                        new GridButton("Exit Replay", () =>
                                            {
                                                Global.ReplayManager.StopReplay();
                                                _nextMenu = new ReplayMenu();
                                            }),
                                    }, new Padding(0, 15)),
                            }, new Padding(5, 5))
                    });
            }
            _grid.Render();
            _statsView.Render();

            return _nextMenu;
        }
    }
}