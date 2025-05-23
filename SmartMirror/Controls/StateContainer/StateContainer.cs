﻿using System;
using System.Runtime.CompilerServices;

namespace SmartMirror.Controls.StateContainer
{
    [ContentProperty(nameof(Conditions))]
    public class StateContainer : ContentView
    {
        private readonly Grid _currentContentGrid = new();

        #region -- Public properties --

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

        public List<StateCondition> Conditions { get; set; } = new();

        #endregion

        #region -- Private helpers --

        private static void OnStatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is StateContainer view)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    view?.ChooseState(oldValue, newValue);
                });
            }
        }

        private void ChooseState(object oldValue, object newValue)
        {
            if (Content is null)
            {
                this.Content = _currentContentGrid;
            }

            if (Conditions is not null)
            {
                var newView = Conditions?.FirstOrDefault(condition => condition?.State?.ToString() == newValue?.ToString())?.Content;

                if (newView is not null)
                {
                    _currentContentGrid.Children.Clear();
                    (newView.Parent as Layout)?.Remove(newView);

                    _currentContentGrid.Add(newView);
                }
            }
        }

        #endregion
    }
}

