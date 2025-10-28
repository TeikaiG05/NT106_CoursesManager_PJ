using System.Windows.Forms;

namespace NT106_BT2.Notifications
{
    public static class NotificationManager
    {
        public static void Show(Form parent, string message, ToastNotification.ToastType type)
        {
            if (parent == null || parent.IsDisposed) return;

            var toast = new ToastNotification(message, type);
            toast.ShowNotification(parent);
        }
    }
}
