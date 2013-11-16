using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.GamerServices;

namespace Wp8.Framework.Utils.Common
{
    public enum MessageDialogResult
    {
        Yes,
        No,
        None
    }

    namespace PopNotes.Common
    {
        public static class MessageDialog
        {
            public static async Task<MessageDialogResult> Show(string messageBoxText)
            {
                return await Show(messageBoxText, " ");
            }

            public static async Task<MessageDialogResult> Show(string messageBoxText, string caption)
            {
                return await Show(messageBoxText, caption, "");
            }

            public static async Task<MessageDialogResult> Show(string messageBoxText, string caption, string button1)
            {
                return await Show(messageBoxText, caption, button1, null);
            }


            public static async Task<MessageDialogResult> Show(string messageBoxText, string caption, string button1,
                                                               string button2)
            {
                try
                {
                    if (Guide.IsVisible)
                        return MessageDialogResult.None;

                    var returned =
                        await Task.Factory.FromAsync<int?>((callback, state) =>
                            Guide.BeginShowMessageBox(caption, messageBoxText,
                                                      (button2 == null) ? new[] { button1 } : new[] { button1, button2 }, 0,
                                                      MessageBoxIcon.None, callback, state),
                            Guide.EndShowMessageBox,
                            null);

                    if (!returned.HasValue)
                        return MessageDialogResult.None;
                    switch (returned)
                    {
                        case 0:
                            return MessageDialogResult.Yes;
                        case 1:
                            return MessageDialogResult.No;
                        default:
                            return MessageDialogResult.None;
                    }
                }
                catch (GuideAlreadyVisibleException)
                {
                    return MessageDialogResult.None;
                }
                catch (ArgumentException)
                {
                    return MessageDialogResult.None;
                }
                catch (Exception)
                {
                    return MessageDialogResult.None;
                }
            }
        }
    }
}
