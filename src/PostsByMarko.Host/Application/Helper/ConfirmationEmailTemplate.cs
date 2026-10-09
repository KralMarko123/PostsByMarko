using System.Text.Encodings.Web;

namespace PostsByMarko.Host.Application.Helper;

public static class ConfirmationEmailTemplate
{
    public static EmailContent Create(string firstName, string confirmationLink)
    {
        var name = HtmlEncoder.Default.Encode(firstName);
        var link = HtmlEncoder.Default.Encode(confirmationLink);
        var text = $"""
            Hi {firstName},

            Welcome to PostsByMarko! Confirm your email to finish creating your account and start sharing posts and chatting.

            Confirm your email: {confirmationLink}

            If you didn't create this account, you can ignore this email.
            """;
        // Tables and inline styles keep the layout usable in email clients without external assets.
        var html = $$"""
            <!doctype html>
            <html lang="en">
              <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>Confirm your email · PostsByMarko</title>
              </head>
              <body style="margin:0;padding:0;background-color:#f1f5f9;color:#0f172a;font-family:Arial,Helvetica,sans-serif;">
                <div style="display:none;max-height:0;overflow:hidden;mso-hide:all;">One more step to join PostsByMarko. Confirm your email to get started.</div>
                <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="background-color:#f1f5f9;">
                  <tr>
                    <td align="center" style="padding:32px 16px;">
                      <table role="presentation" width="100%" cellpadding="0" cellspacing="0" style="max-width:560px;background-color:#ffffff;border:1px solid #e2e8f0;border-radius:16px;">
                        <tr>
                          <td style="padding:28px 28px 24px;border-bottom:1px solid #e2e8f0;">
                            <p style="margin:0;color:#4f46e5;font-size:20px;font-weight:700;">PostsByMarko</p>
                            <p style="margin:8px 0 0;color:#64748b;font-size:14px;">Share your thoughts. Start a conversation.</p>
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:28px;">
                            <h1 style="margin:0 0 20px;font-size:28px;line-height:1.25;">You're almost there.</h1>
                            <p style="margin:0 0 12px;font-size:16px;line-height:1.6;">Hi {{name}},</p>
                            <p style="margin:0 0 24px;color:#475569;font-size:16px;line-height:1.6;">Welcome to PostsByMarko! Confirm your email to finish creating your account and start sharing posts and chatting.</p>
                            <table role="presentation" cellpadding="0" cellspacing="0">
                              <tr>
                                <td align="center" bgcolor="#4f46e5" style="border-radius:8px;mso-padding-alt:16px 24px;">
                                  <a href="{{link}}" style="display:inline-block;padding:16px 24px;border:1px solid #4f46e5;border-radius:8px;color:#ffffff;font-size:16px;font-weight:700;line-height:1.2;text-decoration:none;">Confirm your email</a>
                                </td>
                              </tr>
                            </table>
                            <p style="margin:24px 0 8px;color:#64748b;font-size:13px;line-height:1.6;">If the button doesn't work, copy this link into your browser:</p>
                            <p style="margin:0;font-size:13px;line-height:1.6;word-break:break-all;overflow-wrap:anywhere;"><a href="{{link}}" style="color:#4f46e5;text-decoration:underline;">{{link}}</a></p>
                          </td>
                        </tr>
                        <tr>
                          <td style="padding:20px 28px;border-top:1px solid #e2e8f0;">
                            <p style="margin:0;color:#64748b;font-size:13px;line-height:1.6;">If you didn't create this account, you can ignore this email.</p>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </body>
            </html>
            """;
        return new EmailContent(text, html);
    }
}
