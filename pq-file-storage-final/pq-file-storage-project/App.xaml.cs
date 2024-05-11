namespace pq_file_storage_project
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            // Handle when the app starts
            await Shell.Current.GoToAsync("//login");

            base.OnStart();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            if (window != null)
            {
                window.Title = "Lade";
                const int newWidth = 1000;
                const int newHeight = 800;

                window.X = 100;
                window.Y = 200;

                window.Width = newWidth;
                window.Height = newHeight;

                window.MinimumHeight = newHeight;
                window.MinimumWidth = newWidth;
            }

            return window;
        }
    }
}
