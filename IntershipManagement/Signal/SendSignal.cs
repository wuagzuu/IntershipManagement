using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IntershipManagement.Models;

namespace IntershipManagement.Signal
{
    public class SendSignal
    {
        IntershipManagementEntities db = new IntershipManagementEntities();
        public void Save(Notification notif)
        {
            NotificationHub objNotifHub = new NotificationHub();
            db.Configuration.ProxyCreationEnabled = false;
            db.Notifications.Add(notif);
            db.SaveChanges();

            objNotifHub.SendNotification(notif.SentTo);
        }
    }
}