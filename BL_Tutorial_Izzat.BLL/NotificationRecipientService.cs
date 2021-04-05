using BL_Tutorial_Izzat.DAL.Models;
using BL_Tutorial_Izzat.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL_Tutorial_Izzat.BLL
{
    public class NotificationRecipientService
    {
        public async void CreateNewNotification(string messageBody)
        {
            var activity = new NotificationRecipient()
            {
                Id = Guid.NewGuid().ToString(),
                MessageBody = messageBody,
                DateSaved = DateTime.UtcNow
            };

            using (var reps = new AccessCosmos.NotificationRepository())
            {
                await reps.CreateAsync(activity);
            }
        }
    }
}
