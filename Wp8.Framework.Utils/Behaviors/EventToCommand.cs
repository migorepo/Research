using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Wp8.Framework.Utils.Behaviors
{
    public class EventToCommand : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventToCommand), new PropertyMetadata(null, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var eventToCommand = s as EventToCommand;
            if (eventToCommand == null)
            {
                return;
            }
            if (eventToCommand.AssociatedObject == null)
            {
                return;
            }
            eventToCommand.EnableDisableElement();
        }));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommand),
            new PropertyMetadata(null, (s, e) => OnCommandChanged(s as EventToCommand, e)));
        public static readonly DependencyProperty MustToggleIsEnabledProperty = DependencyProperty.Register("MustToggleIsEnabled", typeof(bool), typeof(EventToCommand), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var eventToCommand = s as EventToCommand;
            if (eventToCommand == null)
            {
                return;
            }
            if (eventToCommand.AssociatedObject == null)
            {
                return;
            }
            eventToCommand.EnableDisableElement();
        }));
        private object _commandParameterValue;
        private bool? _mustToggleValue;
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        public object CommandParameterValue
        {
            get
            {
                return _commandParameterValue ?? CommandParameter;
            }
            set
            {
                _commandParameterValue = value;
                EnableDisableElement();
            }
        }
        public bool MustToggleIsEnabled
        {
            get
            {
                return (bool)GetValue(MustToggleIsEnabledProperty);
            }
            set
            {
                SetValue(MustToggleIsEnabledProperty, value);
            }
        }
        public bool MustToggleIsEnabledValue
        {
            get
            {
                return _mustToggleValue.HasValue ? _mustToggleValue.Value : MustToggleIsEnabled;
            }
            set
            {
                _mustToggleValue = value;
                EnableDisableElement();
            }
        }
        public bool PassEventArgsToCommand
        {
            get;
            set;
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }
        private Control GetAssociatedObject()
        {
            return AssociatedObject as Control;
        }
        private ICommand GetCommand()
        {
            return Command;
        }
        public void Invoke()
        {
            Invoke(null);
        }
        protected override void Invoke(object parameter)
        {
            if (AssociatedElementIsDisabled())
            {
                return;
            }
            var command = GetCommand();
            var obj = CommandParameterValue;
            if (obj == null && PassEventArgsToCommand)
            {
                obj = parameter;
            }
            if (command != null && command.CanExecute(obj))
            {
                command.Execute(obj);
            }
        }
        private static void OnCommandChanged(EventToCommand element, DependencyPropertyChangedEventArgs e)
        {
            if (element == null)
            {
                return;
            }
            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= element.OnCommandCanExecuteChanged;
            }
            var command = (ICommand)e.NewValue;
            if (command != null)
            {
                command.CanExecuteChanged += element.OnCommandCanExecuteChanged;
            }
            element.EnableDisableElement();
        }
        private bool AssociatedElementIsDisabled()
        {
            var associatedObject = GetAssociatedObject();
            return AssociatedObject == null || (associatedObject != null && !associatedObject.IsEnabled);
        }
        private void EnableDisableElement()
        {
            var associatedObject = GetAssociatedObject();
            if (associatedObject == null)
            {
                return;
            }
            var command = GetCommand();
            if (MustToggleIsEnabledValue && command != null)
            {
                associatedObject.IsEnabled = (command.CanExecute(CommandParameterValue));
            }
        }
        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }
    }
}
