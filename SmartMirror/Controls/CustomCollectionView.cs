using System;
using System.Runtime.CompilerServices;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace SmartMirror.Controls;

public class CustomCollectionView : CollectionView
{
    public CustomCollectionView()
    {
        AppendToMapping();
    }

    #region -- Public properties --

    public static readonly BindableProperty PaddingProperty = BindableProperty.Create(
       propertyName: nameof(Padding),
       returnType: typeof(Thickness),
       declaringType: typeof(CustomCollectionView));

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == PaddingProperty.PropertyName || propertyName == ItemsLayoutProperty.PropertyName)
        {
            if (Handler?.PlatformView is RecyclerView recyclerView)
            {
                UpdatePadding(recyclerView);
            }
        }
    }

    #endregion

    #region -- Private helpers --

    private void AppendToMapping()
    {
        Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping(nameof(CustomCollectionView), (handler, view) =>
        {
            if (view is CustomCollectionView)
            {
                UpdatePadding(handler.PlatformView);
            }
        });
    }

    private void UpdatePadding(RecyclerView recyclerView)
    {
        var density = Platform.AppContext.Resources.DisplayMetrics.Density;

        var leftPadding = (int)(Padding.Left * density);
        var topPadding = (int)(Padding.Top * density);
        var rightPadding = (int)(Padding.Right * density);
        var bottomPadding = (int)(Padding.Bottom * density);

        if (recyclerView.GetItemDecorationAt(0) is SpacingItemDecoration spacingItemDecoration)
        {
            leftPadding -= spacingItemDecoration.HorizontalOffset;
            rightPadding -= spacingItemDecoration.HorizontalOffset;
            topPadding -= spacingItemDecoration.VerticalOffset;
            bottomPadding -= spacingItemDecoration.VerticalOffset;
        }

        recyclerView.SetPadding(leftPadding, topPadding, rightPadding, bottomPadding);
        recyclerView.SetClipToPadding(false);
    }

    #endregion
}
