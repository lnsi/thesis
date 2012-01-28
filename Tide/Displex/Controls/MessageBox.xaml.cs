using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media.Animation;

namespace Displex.Controls
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : SurfaceUserControl
    {
        public enum TYPE : byte { OK, ERROR, INFO };

        public static TYPE OK
        {
            get { return TYPE.OK; }
        }

        public static TYPE ERROR
        {
            get { return TYPE.ERROR; }
        }

        public static TYPE INFO
        {
            get { return TYPE.INFO; }
        }

        public MessageBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(TYPE), typeof(MessageBox));

        public TYPE Icon
        {
            get { return (TYPE)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MessageBox));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(MessageBox));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public void Show(TYPE type, string header, string text)
        {
            Icon = type;
            Header = header;
            Text = text;

            switch (Icon)
            {
                case TYPE.OK: iconType.Source = Resources["okIcon"] as BitmapImage; break;
                case TYPE.ERROR: iconType.Source = Resources["errorIcon"] as BitmapImage; break;
                case TYPE.INFO: iconType.Source = Resources["infoIcon"] as BitmapImage; break;
            }

            Storyboard story = FindResource("showMessageBox") as Storyboard;
            story.Begin(this);
        }
    }
}
