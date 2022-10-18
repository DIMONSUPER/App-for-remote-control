namespace SmartMirror.Effects.Touch
{
    sealed class EffectIds
    {
        /// <summary>
        /// The Base Resolution Group Name For Effects
        /// </summary>
        static string effectResolutionGroupName = $"{nameof(Xamarin)}.{nameof(CommunityToolkit)}.{nameof(Effects)}";

        /// <summary>
        /// Effect Id for <see cref="TouchEffect"/>
        /// </summary>
        public static string TouchEffect => $"{effectResolutionGroupName}.{nameof(TouchEffect)}";

        /// <summary>
        /// Effect Id for <see cref="ShadowEffect"/>
        /// </summary>
        public static string ShadowEffect => $"{effectResolutionGroupName}.{nameof(ShadowEffect)}";
    }
}
