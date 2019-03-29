using learning_gui.Views;
using Terminal.Gui;

namespace learning_gui
{
    internal static class Program
    {
        public static void Main()
        {
            Application.Init();
            var top = Application.Top;

            Welcome.CreateWelcomeUI(top);

            Application.Run();
        }
    }
}