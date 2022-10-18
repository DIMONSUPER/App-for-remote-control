using CommunityToolkit.Maui.Extensions;
using System.Windows.Input;

namespace SmartMirror.Behaviors
{
    public class TouchBehavior : Behavior<VisualElement>
    {
        #region -- Public Properties --

        public static BindableProperty CommandProperty = BindableProperty.CreateAttached(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(ICommand),
            propertyChanged: OnCommandPropertyChanged);

        public static BindableProperty CommandParameterProperty = BindableProperty.CreateAttached(
            propertyName: "CommandParameter",
            returnType: typeof(object),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(object));

        public static BindableProperty IsAnimationProperty = BindableProperty.CreateAttached(
            propertyName: "IsAnimation",
            returnType: typeof(bool),
            declaringType: typeof(TouchBehavior),
            defaultValue: default(bool));

        #endregion

        #region -- Public Helpers --

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

        #endregion

        #region -- Private Helpers --

        private static void OnCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view && view.GestureRecognizers.FirstOrDefault(gesture => gesture is TapGestureRecognizer) is null)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnTappedEventHandler;

                view.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        private static void OnTappedEventHandler(object sender, EventArgs e)
        {
            var command = GetCommand((BindableObject)sender);
            var isAnimation = GetIsAnimation((BindableObject)sender);
            var commandParameter = GetCommandParameter((BindableObject)sender);

            if (isAnimation
                && sender is View view
                && view.BackgroundColor is not null)
            {
                var backgroundColor = view.BackgroundColor;
                var backgroundColorTo = new Color(backgroundColor.Red, backgroundColor.Green, backgroundColor.Blue, 0.5f);

                view.BackgroundColorTo(backgroundColorTo, 16, 150, Easing.SpringOut).ContinueWith((x) =>
                {
                    view.BackgroundColorTo(backgroundColor, 16, 150, Easing.SpringIn);
                });
            }

            if (command is not null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }

        #endregion
    }
}
