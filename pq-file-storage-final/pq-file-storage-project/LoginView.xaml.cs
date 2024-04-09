namespace pq_file_storage_project
{
    public partial class LoginView : ContentPage
    {

        public LoginView()
        {
            InitializeComponent();
        }

        // Event handler for email entry completion
        private void OnEmailCompleted(object sender, EventArgs e)
        { 
            SemanticScreenReader.Announce("Email field completed");
        }

        // Event handler for email entry focus
        private void OnEmailFocused(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Email field focused");
        }

        // Event handler for email entry unfocus
        private void OnEmailUnfocused(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Email field unfocused");
        }

        // Event handler for login button click
        private void OnLoginClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button clicked");
        }

        // Event handler for login button focus
        private void OnLoginFocused(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button focused");
        }

        // Event handler for login button unfocus
        private void OnLoginUnfocused(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button unfocused");
        }

        // Event handler for login button press
        private void OnLoginPressed(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pressed");
        }

        // Event handler for login button release
        private void OnLoginReleased(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button released");
        }

        // Event handler for login button pointer entered
        private void OnLoginPointerEntered(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pointer entered");
        }

        // Event handler for login button pointer moved
        private void OnLoginPointerMoved(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pointer moved");
        }

        // Event handler for login button pointer exit
        private void OnLoginPointerExit(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pointer exit");
        }

        // Event handler for login button pointer pressed
        private void OnLoginPointerPressed(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pointer pressed");
        }

        // Event handler for login button pointer released
        private void OnLoginPointerReleased(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pointer released");
        }

        // Event handler for login button pointer canceled
        private void OnLoginPointerCanceled(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pointer canceled");
        }

        // Event handler for login button pointer capture lost
        private void OnLoginPointerCaptureLost(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Login button pointer capture lost");
        }
        
        // Event handler for Register link click
        private void OnRegisterClicked(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link clicked");
        }

        // Event handler for Register link focus
        private void OnRegisterFocused(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link focused");
        }

        // Event handler for Register link unfocus
        private void OnRegisterUnfocused(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link unfocused");
        }

        // Event handler for Register link press
        private void OnRegisterPressed(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link pressed");
        }

        // Event handler for Register link release
        private void OnRegisterReleased(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link released");
        }

        // Event handler for Register link pointer entered
        private void OnRegisterPointerEntered(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link pointer entered");
        }

        // Event handler for Register link pointer moved
        private void OnRegisterPointerMoved(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link pointer moved");
        }

        // Event handler for Register link pointer exit
        private void OnRegisterPointerExit(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link pointer exit");
        }

        // Event handler for Register link pointer pressed
        private void OnRegisterPointerPressed(object sender, EventArgs e)
        {
            SemanticScreenReader.Announce("Register link pointer pressed");
        }
    }

}
