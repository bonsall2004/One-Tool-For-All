using AdonisUI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTFA
{
    public class Errors
    {
        public static void ShowException(Exception ex)
        {
            var messageBox = new MessageBoxModel
            {
                Text = "An Error has Occurred",
                Caption = ex.Message,
                Icon = MessageBoxImage.Error,
                Buttons = new[] {
                            MessageBoxButtons.Ok(),
                        }
            };
            MessageBox.Show(messageBox);
            return;
        }

        public static void ShowOther(string title, string caption)
        {
            var messageBox = new MessageBoxModel
            {
                Text = title,
                Caption = caption,
                Icon = MessageBoxImage.Error,
                Buttons = new[] {
                    MessageBoxButtons.Ok(),
                }
            };
            MessageBox.Show(messageBox);
        }
    }

    public class Info
    {
        public static void Show(string title, string caption)
        {
            var messageBox = new MessageBoxModel
            {
                Text = title,
                Caption = caption,
                Icon = MessageBoxImage.Information,
                Buttons = new[] {
                    MessageBoxButtons.Ok(),
                }
            };
            MessageBox.Show(messageBox);
        }
    }
}
