using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Resend;
using UrlSamurai.Data;

namespace UrlSamurai.Components.Account;

internal sealed class IdentityResendClient(IResend resend) : IEmailSender<ApplicationUser>
{
    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        var message = new EmailMessage
        {
            From = "no-reply@denysov.me"
        };
        message.To.Add( email );
        message.Subject = "Confirm your email";
        message.HtmlBody = $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.";
        
        await resend.EmailSendAsync(message);
    }

    public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        var message = new EmailMessage
        {
            From = "no-reply@denysov.dev"
        };
        message.To.Add( email );
        message.Subject = "Reset your password";
        message.HtmlBody = $"Please reset your password by <a href='{resetLink}'>clicking here</a>.";
        
        await resend.EmailSendAsync(message);
    }

    public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        var message = new EmailMessage
        {
            From = "no-reply@denysov.dev"
        };
        message.To.Add( email );
        message.Subject = "Reset your password";
        message.HtmlBody = $"Please reset your password using the following code: {resetCode}";
        
        await resend.EmailSendAsync(message);
    }
}