﻿using Microsoft.Office.Interop.Outlook;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class MailService
    {
        private readonly ILogger<MailService> _logger;

        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sends an email using Microsoft Outlook.
        /// </summary>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body content of the email.</param>
        /// <param name="recipient">Email address of the recipient.</param>
        public void SendEmailUsingOutlook(string recipient, string subject, string body, string attachmentPath = null)
        {
            try
            {
                if (string.IsNullOrEmpty(recipient))
                {
                    throw new ArgumentException("Recipient email address is required.");
                }

                Application outlookApp = new Application(); // Create new Outlook application.
                MailItem mailItem = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem); // Create a new mail item.

                // Set the properties of the mail item.
                mailItem.Subject = subject;
                mailItem.Body = body;
                mailItem.To = recipient; // Specify the recipient.

                // Check if there is an attachment path provided and add it to the email
                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    if (System.IO.File.Exists(attachmentPath))
                    {
                        mailItem.Attachments.Add(attachmentPath, OlAttachmentType.olByValue, 1, attachmentPath);
                    }
                    else
                    {
                        _logger.LogWarning("The specified attachment path does not exist: {0}", attachmentPath);
                    }
                }

                _logger.LogInformation("Attempting to send mail: Subject: {0}, Body: {1}, To: {2}, Attachment: {3}", subject, body, recipient, attachmentPath);
                mailItem.Send(); // Send the email.
                _logger.LogInformation($"Email sent successfully to: {recipient}");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Failed to send email. Error: {ex.Message}");
                throw; // Optionally rethrow the exception to be handled by the caller
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to send email. Error: {ex.Message}");
            }
        }

    }
}
