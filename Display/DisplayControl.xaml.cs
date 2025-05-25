using System.Windows;
using System.Windows.Controls;
using YukkuriMovieMaker.Commons;

namespace Trigger.Display
{
    public partial class DisplayControl : UserControl,IPropertyEditorControl
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(DisplayControl), new PropertyMetadata(string.Empty));

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public DisplayControl()
        {
            InitializeComponent();
        }
    }
}
