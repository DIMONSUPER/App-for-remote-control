using System;
using System.Runtime.CompilerServices;

namespace SmartMirror.Controls.StateContainer
{
    [ContentProperty("Conditions")]
    public class StateContainer : ContentView
    {
        public static readonly BindableProperty StateProperty = BindableProperty.Create(
            propertyName: nameof(State),
            returnType: typeof(object),
            declaringType: typeof(StateContainer),
            propertyChanged: OnStatePropertyChanged);

        public object State
        {
            get => GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        private readonly Grid _currentContentGrid = new();

        #region -- Public properties --

        public List<StateCondition> Conditions { get; set; } = new();

        #endregion

        #region -- Private helpers --

        private static void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is StateContainer view)
            {
                view.ChooseStateAsync(oldValue, newValue);
            }
        }

        private Task ChooseStateAsync(object oldValue, object newValue)
        {
            if (Content is null)
            {
                this.Content = _currentContentGrid;
            }

            if (Conditions is not null)
            {
                var tasks = new List<Task>();

                var newView = Conditions?.FirstOrDefault(condition => condition?.State?.ToString() == newValue?.ToString())?.Content;

                if (newView is not null)
                {
                    _currentContentGrid.Children.Clear();
                    (newView.Parent as Layout)?.Remove(newView);

                    _currentContentGrid.Add(newView);
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}

