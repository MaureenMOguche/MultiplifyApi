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
                    </div>
                </div>
                <div class=""footer""
                    style=""background-color: #008080; padding: 30px; text-align: center; color: white;"">
                    <p>Copyright &copy; {DateTime.Now.Year} Multiplify</p>
                </div>
            </body>

            </html>
            ";
    }

    public static string WaitlistEmail(string username)
    {
        var message = $@"<p>
                    Welcome to Multiplify, where we believe in the transformative power of female entrepreneurship! We are
                    thrilled to have you join our waitlist as we prepare to launch our platform dedicated to connecting
                    women to essential financial tools and opportunities.
                </p>
                <p>
                    At Multiplify, our mission is clear: <strong>to empower women entrepreneurs like you to unleash your
                        full
                        potential and drive positive change in communities worldwide.</strong> With our platform, you'll
                    gain access
                    to a
                    dynamic network of resources, mentorship, and support tailored to fuel your entrepreneurial journey.
                </p>
                <p>
                    Here's a glimpse of what you can expect from Multiplify:
                </p>
                <ul>
                    <li>
                        Financial Tools: Gain access to a suite of financial resources designed to help you manage and grow
                        your
                        business effectively.
                    </li>
                    <li>
                        Opportunity Connections: Connect with like-minded women entrepreneurs, mentors, and investors to
                        expand
                        your network and unlock new opportunities.
                    </li>
                    <li>
                        Educational Resources: Access curated content, workshops, and webinars to enhance your skills,
                        knowledge, and confidence as a business leader.
                    </li>
                    <li>
                        Community Support: Join a vibrant community of women who share your passion for entrepreneurship,
                        where
                        you can exchange ideas, seek advice, and celebrate successes together.
                    </li>
                </ul>
                <p>
                    By signing up for our waitlist, you're not just expressing interest in Multiplify – you're joining a
                    movement dedicated to leveling the playing field for women in business.
                </p>
                <br>
                <p>
                    Stay tuned for updates on our official launch date, exclusive sneak peeks, and early access
                    opportunities. In the meantime, we invite you to spread the word about Multiplify and help us amplify
                    the voices and contributions of women entrepreneurs worldwide.
                </p>
                <p>
                    Thank you for embarking on this exciting journey with us. Together, let's multiply the impact of women
                    entrepreneurs and empower the world!
                </p>";

        //return message;
        return Prefix(username, "Welcome to Multiplify", message, "...enabling women, enabling the world");
    }
}
