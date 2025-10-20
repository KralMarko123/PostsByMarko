using Microsoft.Playwright;
using PostsTesting.UI_Models.Components;

namespace PostsByMarko.FrontendTests.UI_Models.Components
{
    public class Message : Component
    {
        public ILocator message;

        public Message(IPage page, ILocator message) : base(page)
        {
            this.message = message;
        }

        public ILocator date => message.Locator(".message-date");
        public ILocator handle => message.Locator(".message-handle");
        public ILocator content => message.Locator(".message-content");

    }
}
