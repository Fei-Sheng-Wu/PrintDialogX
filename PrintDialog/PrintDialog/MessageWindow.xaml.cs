using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PrintDialogX.PrintControl
{
    partial class MessageWindow : Window
    {
        public enum ResultButton
        {
            Button1,
            Button2,
            Null
        }

        public enum MessageIcon
        {
            Info,
            Error,
            Question,
            Warning,
            Null
        }

        public ResultButton WindowResultButton { get; protected set; }

        public MessageWindow(string Message, string Title = "Message", string Btn1 = "OK", string Btn2 = null, MessageIcon Icon = MessageIcon.Info)
        {
            InitializeComponent();

            this.Title = Title;

            WindowResultButton = ResultButton.Null;

            if (Btn1 != null)
            {
                Button1.Content = Btn1;
            }
            else
            {
                Button1.Visibility = Visibility.Collapsed;
            }

            if (Btn2 != null)
            {
                Button2.Content = Btn2;
            }
            else
            {
                Button2.Visibility = Visibility.Collapsed;
            }

            txtMessage.Text = Message;

            if (Icon == MessageIcon.Null)
            {
                imgIconPath.Data = null;
                txtMessage.SetValue(Grid.ColumnProperty, 0);
                txtMessage.SetValue(Grid.ColumnSpanProperty, 2);
            }
            else if (Icon == MessageIcon.Info)
            {
                imgIconPath.Data = Geometry.Parse("M512 0C299.936 0 128 171.936 128 384S299.936 768 512 768s384-171.936 384-384-171.936-384-384-384m0 832C264.96 832 64 631.04 64 384s200.96-448 448-448 448 200.96 448 448S759.04 832 512 832M480 128h64v288h-64zM512 624a48 48 0 1 1 0-96 48 48 0 0 1 0 96");
                imgIcon.Foreground = new SolidColorBrush(Color.FromRgb(75, 139, 244));
            }
            else if (Icon == MessageIcon.Error)
            {
                imgIconPath.Data = Geometry.Parse("M512 0C299.936 0 128 171.936 128 384S299.936 768 512 768s384-171.936 384-384-171.936-384-384-384m0 832C264.96 832 64 631.04 64 384s200.96-448 448-448 448 200.96 448 448S759.04 832 512 832M665.376 582.624L512 429.248l-153.376 153.376-45.248-45.248L466.752 384l-153.376-153.376 45.248-45.248L512 338.752l153.376-153.376 45.248 45.248L557.248 384l153.376 153.376z");
                imgIcon.Foreground = new SolidColorBrush(Color.FromRgb(195, 75, 66));
            }
            else if (Icon == MessageIcon.Warning)
            {
                imgIconPath.Data = Geometry.Parse("M480 256h64v256h-64zM512 96a48 48 0 1 1 0 96 48 48 0 0 1 0-96M512 700.8L874.272 32H149.728L512 700.8z m443.744-685.76l0.384 0.192-416 768-0.384-0.224A31.584 31.584 0 0 1 512 800a31.584 31.584 0 0 1-27.744-16.96l-0.384 0.192-416-768 0.384-0.224A31.2 31.2 0 0 1 64 0a32 32 0 0 1 32-32h832a32 32 0 0 1 32 32 31.2 31.2 0 0 1-4.256 15.04z");
                imgIcon.Foreground = new SolidColorBrush(Color.FromRgb(255, 205, 67));
            }
            else if (Icon == MessageIcon.Question)
            {
                imgIconPath.Data = Geometry.Parse("M512 0C299.936 0 128 171.936 128 384S299.936 768 512 768s384-171.936 384-384-171.936-384-384-384m0 832C264.96 832 64 631.04 64 384s200.96-448 448-448 448 200.96 448 448S759.04 832 512 832M511.68 192a48 48 0 1 1 0-96 48 48 0 0 1 0 96M512 640l-0.288-0.032A160.16 160.16 0 0 1 352 480h64a96.096 96.096 0 0 0 95.712 95.968L512 576c52.928 0 96-43.072 96-96s-43.072-96-96-96l-0.288 0.032V384H480v-128h64v67.264A160.224 160.224 0 0 1 672 480c0 88.224-71.776 160-160 160");
                imgIcon.Foreground = new SolidColorBrush(Color.FromRgb(23, 160, 93));
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            WindowResultButton = ResultButton.Button1;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            WindowResultButton = ResultButton.Button2;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
