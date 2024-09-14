
using System.Text;
using Tcap.App;
using Terminal.Gui;
using Unix.Terminal;

Console.OutputEncoding = Encoding.Default;

Application.Init();
// Colors.Menu = GetMenuColorScheme();
Application.Run<MainAppView>();
Application.Shutdown();

static ColorScheme GetMenuColorScheme()
{
    var colorScheme = new ColorScheme();
    colorScheme.Normal = Curses.A_REVERSE;
    return colorScheme;
}