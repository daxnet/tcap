using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NStack;
using SharpPcap;
using Terminal.Gui;

namespace Tcap.App
{
    internal class MainAppView : Toplevel
    {
        private readonly CaptureDeviceList _captureDevices;
        private MenuBar? _menuBar;
        private StatusBar? _statusBar;
        private ListView? _listView;
        private StatusItem? _statusSelectedDevice;

        public MainAppView()
        {
            Initialize();
            _captureDevices = CaptureDeviceList.Instance;
        }

        private void Initialize()
        {
            ColorScheme = Colors.Base;
            _menuBar = new()
            {
                ColorScheme = Colors.Menu,
                Menus =
                [
                    new
                    (
                        "_Session",
                        [
                            new MenuItem(
                                "_New...",
                                "",
                                CreateNewSession,
                                shortcut: Key.CtrlMask | Key.N
                            ),
                            null,
                            new MenuItem(
                                "_Start",
                                "",
                                () => { },
                                shortcut: Key.CtrlMask | Key.AltMask | Key.S
                            ),
                            new MenuItem(
                                "S_top",
                                "",
                                () => { },
                                shortcut: Key.CtrlMask | Key.AltMask | Key.T
                            ),
                            null,
                            new MenuItem(
                                "_Exit",
                                "",
                                () => Application.RequestStop(this),
                                shortcut: Key.CtrlMask | Key.Q)
                        ]
                    ),
                    new
                    (
                        "_Help",
                        [
                            new MenuItem(
                                "_About...",
                                "",
                                () => MessageBox.Query(
                                    title: "About Tcap",
                                    message: "Terminal Packet Capture v1.0",
                                    buttons: "_OK"
                                ),
                                null,
                                null,
                                Key.CtrlMask | Key.A
                            )
                        ]
                    )
                ]
            };

            _statusSelectedDevice = new StatusItem(Key.CharMask, "Device: (none)", null);

            _statusBar = new()
            {
                Visible = true,
                Items =
                [
                    new StatusItem(Key.CtrlMask | Key.Q, "^Q: Quit", () => Application.RequestStop(this)),
                    _statusSelectedDevice
                ]
            };

            

            Add(_menuBar, _statusBar);
        }

        private void CreateNewSession()
        {
            var device = SelectLiveDevice();
            if (device is null || _statusSelectedDevice is null)
            {
                return;
            }

            _statusSelectedDevice.Title = $"Device: {device.Description}";
            _statusBar?.SetNeedsDisplay();
        }

        private ILiveDevice? SelectLiveDevice()
        {
            ILiveDevice? selectedDevice = null;
            var radioButtons = new List<ustring>();
            foreach (var device in _captureDevices)
            {
                radioButtons.Add(device.Description);
            }

            var rgDeviceList = new RadioGroup(radioButtons.ToArray());

            var dialogOkButton = new Button("_OK", true);
            var dialogCancelButton = new Button("_Cancel");

            dialogOkButton.Clicked += () =>
            {
                selectedDevice = _captureDevices[rgDeviceList.SelectedItem];
                Application.RequestStop();
            };

            dialogCancelButton.Clicked += () => Application.RequestStop();

            var dialog = new Dialog("Select Device", [dialogOkButton, dialogCancelButton]);
            var label = new Label("Select the device for capturing the packets:");

            dialog.Add(label, rgDeviceList);

            Application.Run(dialog);
            return selectedDevice;
        }
    }
}
