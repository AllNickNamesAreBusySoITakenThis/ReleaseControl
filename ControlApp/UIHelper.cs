using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using ReleaseControlLib;
using System.Windows;


namespace ControlApp
{
    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogCloser),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var window = d as Window;
            if (window != null)
            {
                window.DialogResult = e.NewValue as bool?;
                if (window.DialogResult != null)
                    window.Close();
            }
        }

        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }
    }

    public class StorageTypeToVisibleConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is StorageTypes))
            {
                return null;
            }
            else
            {
                switch (parameter)
                {
                    case "XML":
                        return (StorageTypes)value == StorageTypes.XML ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    default:
                        return (StorageTypes)value == StorageTypes.Database ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConnectionTypeToVisibleConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is ConnectionTypes))
            {
                return null;
            }
            else
            {
                switch (parameter)
                {
                    case "SQL":
                        return (ConnectionTypes)value == ConnectionTypes.Sql ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    case "MySql":
                        return (ConnectionTypes)value == ConnectionTypes.OracleMySql ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                    default:
                        return (ConnectionTypes)value == ConnectionTypes.OleDb ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConnectionTypeToBoolConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is ConnectionTypes))
            {
                return null;
            }
            else
            {
                switch (parameter)
                {
                    case "SQL":
                        return (ConnectionTypes)value == ConnectionTypes.Sql;
                    case "MySql":
                        return (ConnectionTypes)value == ConnectionTypes.OracleMySql;
                    default:
                        return (ConnectionTypes)value == ConnectionTypes.OleDb;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
