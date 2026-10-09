using FluentAssertions;
using Microsoft.Playwright;

namespace PostsTesting.UI_Models.Pages
{
    public class RegisterPage : Page
    {
        private static string url => $"{baseUrl}/register";
        public RegisterPage(IPage page) : base(page) { }


        public ILocator firstName => page.GetByLabel("First Name", new PageGetByLabelOptions { Exact = true });
        public ILocator lastName => page.GetByLabel("Last Name", new PageGetByLabelOptions { Exact = true });
        public ILocator email => page.GetByLabel("Email", new PageGetByLabelOptions { Exact = true });
        public ILocator password => page.GetByLabel("Password", new PageGetByLabelOptions { Exact = true });
        public ILocator confirmPassword => page.GetByLabel("Confirm Password", new PageGetByLabelOptions { Exact = true });
        public ILocator confirmationalForm => page.Locator(".form.confirmational");
        public ILocator formTitle => page.Locator(".register .form .form-title");
        public ILocator registerButton => button;
        public ILocator loginLink => link;


        public async Task Visit()
        {
            await page.GotoAsync(url);
        }

        public async Task ClickRegisterButton()
        {
            await registerButton.ClickAsync();
        }

        public async Task ClickLoginLink()
        {
            await loginLink.ClickAsync();
        }

        public async Task FillInFirstNameInput(string firstNameToBeEntered)
        {
            await firstName.FillAsync(firstNameToBeEntered);
        }
        public async Task FillInLastNameInput(string lastNameToBeEntered)
        {
            await lastName.FillAsync(lastNameToBeEntered);
        }

        public async Task FillInEmailInput(string emailToBeEntered)
        {
            await email.FillAsync(emailToBeEntered);
        }

        public async Task FillInPasswordInput(string passwordToBeEntered)
        {
            await password.FillAsync(passwordToBeEntered);
        }

        public async Task FillInConfirmPasswordInput(string passwordToBeEntered)
        {
            await confirmPassword.FillAsync(passwordToBeEntered);
        }

        public async Task Register(string firstName, string lastName, string email, string password)
        {
            await FillInFirstNameInput(firstName);
            await FillInLastNameInput(lastName);
            await FillInEmailInput(email);
            await FillInPasswordInput(password);
            await FillInConfirmPasswordInput(password);
            await ClickRegisterButton();
        }
    }
}
