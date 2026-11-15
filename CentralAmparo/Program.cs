namespace CentralAmparo;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Form { Text = "Central Amparo", Width = 800, Height = 500 });
    }
}
