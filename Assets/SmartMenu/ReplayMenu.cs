using System;
using System.Collections.Generic;
using Assets.GridLayout;
using Assets.VirtualProfiler;

namespace Assets.SmartMenu
{
    public class ReplayMenu : ISmartMenu
    {
        private Grid _grid;
        private readonly List<IMenuTextField> _boundTextFields;
        private string _replayFolder = "";
        private ISmartMenu _nextMenu;

        public ReplayMenu()
        {
            Global.ReplayManager.EnableReplayView();
        }

        public ISmartMenu CreateMenu()
        {
            _nextMenu = this;
            if (_grid == null)
            {
                _grid = new Grid(5, 5, 150, 155);
                _grid.Add(new[]
                    {
                        new GridColumn(new List<IGridRenderable>
                            {
                                new GridLabel(() => "Movement log file:"),
                                new GridTextField((x) => _replayFolder = x, "0123-0123-0123-0123-0123"),
                                new GridColumn(new List<IGridRenderable>
                                    {
                                        new GridButton("Exit to Main Menu", () =>
                                            {
                                                Global.ReplayManager.StopReplay();
                                                _nextMenu = new MainMenuView();
                                            }),
                                        new GridButton("Instant Render", () =>
                                            {
                                                _nextMenu = LaunchReplay(false);
                                            }),
                                        new GridButton("Real-time Render", () =>
                                            {
                                                _nextMenu = LaunchReplay(true);
                                            }),
                                    }, new Padding(0, 2)),
                            }, new Padding(2, 5))
                    });
            }
            _grid.Render();

            return _nextMenu;
        }

        public ISmartMenu LaunchReplay(bool isRealTime)
        {
            try
            {
                if (isRealTime) Global.ReplayManager.RealTimeReplay(_replayFolder);
                else Global.ReplayManager.InstantReplay(_replayFolder);
            }
            catch (Exception e)
            {
                Logger.Warning("Problem during replay.", e);
                return new ConfirmationDialogMenu("Profiler", string.Format("There was a problem during replay: {0}", e.Message), this);
            }

            return new ReplayRunningMenu();
        }

    }
}
