using Lab06_ThreeLayerMemory.GUI;
namespace Lab06_ThreeLayerMemory;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new frmMain());
    }
}