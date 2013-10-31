using System;
using System.Collections.Generic;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class ConfigurationMenu : ISmartMenu
    {
        private readonly List<IMenuTextField> _boundTextFields;

        public ConfigurationMenu()
        {
            _boundTextFields = new List<IMenuTextField>
                {
                    new MenuTextFieldBinder<string>(Global.Config.SerialPortMovementInput)
                        {
                            Name = "Movement COM port",
                            Description = "The COM port for reading movement data (e.g. 'COM1', 'COM2', etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 50, 120, 20), "Movement COM Port: ");
                                    return GUI.TextField(new Rect(130, 50, 100, 20), x, 4);
                                },
                            Validator = x =>
                                {
                                    if (string.IsNullOrEmpty(x))
                                        throw new ArgumentException("The movement COM port must be specified!");
                                    Global.Config.SerialPortMovementInput = x;
                                },
                        },
                    new MenuTextFieldBinder<string>(Global.Config.SerialPortBaud.ToString())
                        {
                            Name = "Baud rate",
                            Description = "The baud rate for the movement COM",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 75, 120, 20), "Movement COM Baud: ");
                                    return GUI.TextField(new Rect(130, 75, 100, 20), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.SerialPortBaud = int.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.MovementLogDirectory)
                        {
                            Name = "Reporting folder",
                            Description = "A path to a folder where the individual profiler runs are saved.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 105, 120, 20), "Reporting Folder: ");
                                    return GUI.TextField(new Rect(130, 105, 100, 20), x, 500);
                                },
                            Validator = x =>
                                {
                                    if (string.IsNullOrEmpty(x))
                                        throw new ArgumentException("The reporting folder cannot be empty!");
                                    Global.Config.MovementLogDirectory = x;
                                },
                        },
                    new MenuTextFieldBinder<string>(Global.Config.ScaleX.ToString())
                        {
                            Name = "X Scaling",
                            Description = "The 'X' scaling value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 130, 120, 20), "X Scaling: ");
                                    return GUI.TextField(new Rect(130, 130, 100, 20), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.ScaleX = float.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.ScaleY.ToString())
                        {
                            Name = "Y Scaling",
                            Description = "The 'Y' scaling value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 155, 120, 20), "Y Scaling: ");
                                    return GUI.TextField(new Rect(130, 155, 100, 20), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.ScaleY = float.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.ScaleZ.ToString())
                        {
                            Name = "Z Scaling",
                            Description = "The 'Z' scaling value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 180, 120, 20), "Z Scaling: ");
                                    return GUI.TextField(new Rect(130, 180, 100, 20), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.ScaleZ = float.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.Smoothing.ToString())
                        {
                            Name = "Smoothing",
                            Description = "The smoothing value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 205, 120, 20), "Smoothing: ");
                                    return GUI.TextField(new Rect(130, 205, 100, 20), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.Smoothing = float.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.RuntimeCameraTag)
                        {
                            Name = "Runtime camera tag",
                            Description = "The tag name assigned to the runtime camera.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 230, 120, 20), "Runtime camera tag: ");
                                    return GUI.TextField(new Rect(130, 230, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.RuntimeCameraTag = x,
                        },
                    new MenuTextFieldBinder<string>(Global.Config.ReplayCameraTag)
                        {
                            Name = "Replay camera tag",
                            Description = "The tag name assigned to the replay camera.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 255, 120, 20), "Replay camera tag: ");
                                    return GUI.TextField(new Rect(130, 255, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.ReplayCameraTag = x,
                        },
                    new MenuTextFieldBinder<string>(Global.Config.SurfaceLayerTag)
                        {
                            Name = "Surface layer tag",
                            Description = "The tag name assigned to the surface layer.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 280, 120, 20), "Surface layer tag: ");
                                    return GUI.TextField(new Rect(130, 280, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.SurfaceLayerTag = x,
                        },
                    new MenuTextFieldBinder<string>(Global.Config.LineRendererTag)
                        {
                            Name = "Line renderer tag",
                            Description = "The tag name assigned to the line renderer game object.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 305, 120, 20), "Line renderer tag: ");
                                    return GUI.TextField(new Rect(130, 305, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.LineRendererTag = x,
                        },
 					 new MenuTextFieldBinder<string>(Global.Config.HistogramMinY.ToString())
                        {
                            Name = "Histogram Min Y",
                            Description = "Min Y Value of Histogram.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 330, 120, 20), "Min Y Histogram: ");
                                    return GUI.TextField(new Rect(130, 330, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.HistogramMinY = float.Parse(x),
                        },
									
					 new MenuTextFieldBinder<string>(Global.Config.HistogramMaxY.ToString())
                        {
                            Name = "Histogram Max Y",
                            Description = "Max Y Value Histogram.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 355, 120, 20), "Max Y Histogram: ");
                                    return GUI.TextField(new Rect(130, 355, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.HistogramMaxY = float.Parse(x),
                        },
									
					 new MenuTextFieldBinder<string>(Global.Config.HistogramGranularity.ToString())
                        {
                            Name = "Historam Granuality",
                            Description = "Historam Granuality.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 380, 120, 20), "Historam Granuality: ");
                                    return GUI.TextField(new Rect(130, 380, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.HistogramGranularity = int.Parse(x),
                        },
									
					 new MenuTextFieldBinder<string>(Global.Config.HistogramSampleRate.ToString())
                        {
                            Name = "Histogram Sample Rate",
                            Description = "Histogram Sample Rate.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 405, 120, 20), "Histogram Sample Rate: ");
                                    return GUI.TextField(new Rect(130, 405, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.HistogramSampleRate = float.Parse(x),
                        },
				 	new MenuTextFieldBinder<string>(Global.Config.RenderHistogram.ToString())
                        {
                            Name = "Render Histogram",
                            Description = "Render Histogram",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 430, 120, 20), "Render Histogram: ");
                                    return GUI.TextField(new Rect(130, 430, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.RenderHistogram = bool.Parse(x)
                        },
				 	new MenuTextFieldBinder<string>(Global.Config.HistogramHeight.ToString())
                        {
                            Name = "Histogram Height",
                            Description = "Histogram Height",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 455, 120, 20), "Histogram Height: ");
                                    return GUI.TextField(new Rect(130, 455, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.HistogramHeight = int.Parse(x)
                        },
				 	new MenuTextFieldBinder<string>(Global.Config.HistogramWidth.ToString())
                        {
                            Name = "Histogram Width",
                            Description = "Histogram Width",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 480, 120, 20), "Histogram Width: ");
                                    return GUI.TextField(new Rect(130, 480, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.HistogramWidth = int.Parse(x)
                        },				
					 new MenuTextFieldBinder<string>(Global.Config.MinMotionFilter.ToString())
                        {
                            Name = "Min Motion Filter",
                            Description = "Min Motion Filter.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 505, 120, 40), "Min Motion Filter (higher number reduces jitter): ");
                                    return GUI.TextField(new Rect(130, 505, 100, 20), x.ToString(), 200);
                                },
                            Validator = x => Global.Config.MinMotionFilter = int.Parse(x),
                        },				
               };
        }

        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(5, 5, 235, 700), GUI.skin.box);


            if (GUI.Button(new Rect(5, 550, 50, 20), "OK"))
            {
                foreach (var boundField in _boundTextFields)
                {
                    if (!boundField.UpdateAndValidate())
                    {
                        return new ConfirmationDialogMenu(boundField.Name, boundField.Description, this);
                    }
                }
                return new MainMenuView();
            }
            if (GUI.Button(new Rect(130, 550, 100, 20), "Save as default"))
            {
                foreach (var boundField in _boundTextFields)
                {
                    if (!boundField.UpdateAndValidate())
                    {
                        return new ConfirmationDialogMenu(boundField.Name, boundField.Description, this);
                    }
                }
                Global.Launcher.SaveGlobalConfiguration();

                return new MainMenuView();
            }

            foreach (var boundField in _boundTextFields)
                boundField.Update();

            GUI.EndGroup();

            return this;
        }

    }
}