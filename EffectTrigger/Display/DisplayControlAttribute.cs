using System.Windows;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Views.Converters;


namespace EffectTrigger.Display
{
    internal class DisplayControlAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new DisplayControl();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            var editor = (DisplayControl)control;
            editor.SetBinding(DisplayControl.ValueProperty, ItemPropertiesBinding.Create2(itemProperties));
        }

        public override void ClearBindings(FrameworkElement control)
        {
            BindingOperations.ClearBinding(control, DisplayControl.ValueProperty);
        }
    }
}
