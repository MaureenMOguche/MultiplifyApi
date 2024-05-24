namespace Multiplify.Application.Models;
public static class EmailTemplates
{
    public static string Prefix(string username, string title, string message, string subtitle="")
    {
        return $@"<!DOCTYPE html>
            <html lang=""en"">

            <head>
                <meta charset=""UTF-8"">
                <link rel=""icon"" type=""image/x-icon""
                    href=""https://res.cloudinary.com/dpikjq31o/image/upload/v1713279393/ya4bt1offxqkdt0bezlj.png"" />
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css""
                    integrity=""sha512-iecdLmaskl7CVkqkXNQ/ZH/XLlvWZOJyj7Yy7tcenmpD1ypASozpmT/E0iPtmFIB46ZmdtAc9eNBvH0H/ZpiBw==""
                    crossorigin=""anonymous"" referrerpolicy=""no-referrer"" />
                <link
                    href=""https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap""
                    rel=""stylesheet"">

                <title>Multiplify</title>
                <style>
                    /* Define styles for non-inline support */
                </style>
            </head>

            <body style=""font-family: 'Poppins', sans-serif; padding: 0; margin: 0; box-sizing: border-box;"">
                <div style=""padding: 30px;justify-content: space-between; align-items: center; background-color: #f5f5f5;"">
                    <div style=""width: 100%;"">
                        <div style=""text-align: center; margin-bottom: 20px;"">
                            <img src=""https://res.cloudinary.com/dpikjq31o/image/upload/v1713279393/pwbjehd6jjpu7r9wmqgy.png""
                                alt=""logo"">
                        </div>
                        <div style=""width: 80%; margin: auto; background-color: #008080; height: 2px;""></div>
                    </div>
                    <div>
                        <h2 style=""text-align: center;padding:0;margin:0"">{title}</h2>
                        <small style=""margin-left: 25px;padding:0"">{subtitle}</small>
                    </div>
                    <br>
                    <div>
                        <p>Dear <strong>{username},</strong></p>
                        <div style=""display:block;"">
                            {message}
                        </div>

                        <br>
                        <p>Best regards,</p>
                        <p>Multiplify Team</p>
                    </div>
                </div>
                <div class=""footer""
                    style=""background-color: #008080; padding: 30px; text-align: center; color: white;"">
                    <p>Copyright &copy; {DateTime.UtcNow.Year} Multiplify</p>
                </div>
            </body>

            </html>
            ";
    }

    public static string WaitlistEmail(string username)
    {
        var message = $@"<p>
                    Welcome to Multiplify waitlist! We are building a platform to help women entreprenuers like you access funding, 
                    resources, and support to grow your business.
                </p>
                <p>
                    As a waitlist member, you will get:
                </p>
                
                <ul>
                    <li>
                        Early access to our platform.
                    </li>
                    <li>
                        Exclusive updates and opportunities.
                    </li>
                    <li>
                        A chance to move up the list by inviting friends.
                    </li>
                </ul>
                <p>
                    Get ready to multiply your success with us!
                </p>
                <br>
                <p>
                    Be the first to experience Multiplify. Tell a friend and move up the list together!
                </p>";
                
        //return message;
        return Prefix(username, "Welcome to Multiplify", message, "...enabling women, enabling the world");
    }

    public static string WelcomeEmail(string username)
    {
        var message = $@"<h1>Welcome to Multiplify!</h1>
                <p>Dear {username},</p>
                <p>We're thrilled to have you join Multiplify, your ultimate platform for gaining access to finance to grow your business!</p>
                <p>With Multiplify, you can:</p>
                <ul>
                  <li>Gain Access to finance to fulfil your business needs</li>
                  <li>Showcase your skills in the market place and get hired!</li>
                  <li>Provide funds for entreprenuers in business areas of interest to you</li>
                  <li>And much more!</li>
                </ul>
                <p>To get started, simply click the button below to access your account:</p>
                <div style=""text-align: center;"">
                  <a href=""https://www.mymultiplify.com/login"" class=""button"">Access My Account</a>
                </div>


                <p>If you have any questions or need assistance, feel free to reach out to our support team at support@multiplify.com.</p>
                <br/>            
                <p>Happy multiplying!</p>
                <p>Sincerely,<br>The Multiplify Team</p>";

        return Prefix(username, "Welcome to Multiplify", message);
    }

    public static string ResetPasswordEmail(string username, string token)
    {
        var message = $@"<h1>Password Reset Request</h1>
            <p>Dear {username},</p>
            <p>We received a request to reset your password for your account with Multiplify.</p>
            <p>If you made this request, please click the button below to reset your password:</p>
            <div style=""text-align: center;"">
              <a href=""{token}"" class=""button"">Reset Password</a>
            </div>
            <p>If you didn't make this request, you can safely ignore this email. Your password will remain unchanged.</p>
            <p>If you need further assistance, please contact our support team at support@multiplify.com.</p>
            <p>Best regards,<br>The Multiplify Team</p>";

        return Prefix(username, "Password Reset Request", message);
    }
}
