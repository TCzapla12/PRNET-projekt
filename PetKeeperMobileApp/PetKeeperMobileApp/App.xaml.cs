namespace PetKeeperMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //wymuszenie jasnego motywu
            Application.Current!.UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();
        }
    }
}
