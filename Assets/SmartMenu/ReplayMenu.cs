using System;
using System.Collections.Generic;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class ReplayMenu : ISmartMenu
    {
        private readonly List<IMenuTextField> _boundTextFields;
        private string _replayFolder = "";

        public ReplayMenu()
        {
            _boundTextFields = new List<IMenuTextField>
                {
                    new MenuTextFieldBinder<string>(_replayFolder)
                        {
                            Name = "Folder name",
                            Description = "Name for this run",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 50, 150, 25), "Name for this run: ");
                                    return GUI.TextField(new Rect(160, 50, 220, 25), x, 512);
                                },
                            Validator = x =>
                                {
                                    if (string.IsNullOrEmpty(x))
                                        throw new ArgumentException("A valid folder name must be specified.");
                                    _replayFolder = x;
                                },
                        },
                };
        }

        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 160, 400, 600));

            GUI.Box(new Rect(0, 0, 400, 300), "");

            if (GUI.Button(new Rect(60, 270, 110, 25), "Replay!"))
            {
                foreach (var boundField in _boundTextFields)
                {
                    if (!boundField.UpdateAndValidate())
                    {
                        return new ConfirmationDialogMenu(boundField.Name, boundField.Description, this);
                    }
                }
                try
                {
                    Global.Launcher.StartReplay(_replayFolder);
                    return new RunningMenu();
                }
                catch (Exception e)
                {
                    return new ConfirmationDialogMenu("Profiler", string.Format("There was a problem while loading replay information: {0}", e.Message), this);
                }
            }

            if (GUI.Button(new Rect(230, 270, 110, 25), "Cancel"))
            {
                Global.Launcher.DisableStreamAdapter();
                return new MainMenuView();
            }

            foreach (var boundField in _boundTextFields)
                boundField.Update();

            GUI.EndGroup();

            return this;
        }
    }
}