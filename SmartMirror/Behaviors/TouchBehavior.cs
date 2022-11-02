using CommunityToolkit.Maui.Extensions;
using System.Windows.Input;

namespace SmartMirror.Behaviors
{
    public class TouchBehavior : Behavior<VisualElement>
    {
        #region -- Public properties --

        public static BindableProperty CommandProperty = BindableProperty.CreateAttached(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(ICommand),
            propertyChanged: OnPropertyChanged);

        public static BindableProperty CommandParameterProperty = BindableProperty.CreateAttached(
            propertyName: "CommandParameter",
            returnType: typeof(object),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(object));

        public static BindableProperty IsAnimationProperty = BindableProperty.CreateAttached(
            propertyName: "IsAnimation",
            returnType: typeof(bool),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(bool),
            propertyChanged: OnPropertyChanged);

        public static BindableProperty NormalBackgroundColorProperty = BindableProperty.CreateAttached(
            propertyName: "NormalBackgroundColor",
            returnType: typeof(Color),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(Color));

        public static BindableProperty PressedBackgroundColorProperty = BindableProperty.CreateAttached(
            propertyName: "PressedBackgroundColor",
            returnType: typeof(Color),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(Color));

        #endregion

        #region -- Public helpers --

        public static ICommand GetCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(CommandProperty);
        }

        public static object GetCommandParameter(BindableObject view)
        {
            return view.GetValue(CommandParameterProperty);
        }

        public static bool GetIsAnimation(BindableObject view)
        {
            return (bool)view.GetValue(IsAnimationProperty);
        }

        public static Color GetNormalBackgroundColor(BindableObject view)
        {
            return (Color)view.GetValue(NormalBackgroundColorProperty);
        }

        public static Color GetPressedBackgroundColor(BindableObject view)
        {
            return (Color)view.GetValue(PressedBackgroundColorProperty);
        }

        public static void SetAnimation(BindableObject view, bool value)
        {
            view.SetValue(IsAnimationProperty, value);
        }

        public static void SetNormalBackgroundColor(BindableObject view, Color color)
        {
            view.SetValue(NormalBackgroundColorProperty, color);
        }
        
        public static void SetPressedBackgroundColor(BindableObject view, Color color)
        {
            view.SetValue(PressedBackgroundColorProperty, color);
        }

        public static void SetCommand(BindableObject view, ICommand command)
        {
            view.SetValue(CommandProperty, command);
        }

        #endregion

        #region -- Private helpers --

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view && view.GestureRecognizers.FirstOrDefault(gesture => gesture is TapGestureRecognizer) is null)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnTappedEventHandler;

                view.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        private static async void OnTappedEventHandler(object sender, EventArgs e)
        {
            var bindable = sender as BindableObject;

            var isAnimation = GetIsAnimation(bindable);

            if (isAnimation && sender is View view)
            {
                var normalBackgroundColor = GetNormalBackgroundColor(bindable);
                var pressedBackgroundColor = GetPressedBackgroundColor(bindable);

                if (normalBackgroundColor is not null && pressedBackgroundColor is not null)
                {
                    await bindable.Dispatcher.DispatchAsync(async () =>
                    {
                        await view.BackgroundColorTo(pressedBackgroundColor, 16, 60, Easing.SpringOut)
                            .ContinueWith(x => view.BackgroundColorTo(normalBackgroundColor, 16, 60, Easing.SpringIn));
                    });
                }
            }

            var command = GetCommand(bindable);
            var commandParameter = GetCommandParameter(bindable);

            if (command is not null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        #endregion
    }
}
