using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Android.Widget;
using Android.Graphics;
using SmartMirror.Interfaces;
using SmartMirror.Models.BindableModels;
using Android.Content;
using Android.Util;
using SmartMirror.Helpers;
using Paint = Android.Graphics.Paint;
using Rect = Android.Graphics.Rect;
using System.ComponentModel;

namespace SmartMirror.Controls;

public partial class ExpandedMultipleChipSelector : ContentView
{
    private const float EMPTY_CHIP_WIDTH = 107;
    private const float WRAP_BUTTON_WIDTH = 61;

    private int _visibleChipsInFirstRow = 0;
    private List<float> _chipWidths = new();

    public ExpandedMultipleChipSelector()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
        propertyName: nameof(IsExpanded),
        returnType: typeof(bool),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWay);

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly BindableProperty ChipDataTemplateSelectorProperty = BindableProperty.Create(
        propertyName: nameof(ChipDataTemplateSelector),
        returnType: typeof(DataTemplateSelector),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWay);

    public DataTemplateSelector ChipDataTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(ChipDataTemplateSelectorProperty);
        set => SetValue(ChipDataTemplateSelectorProperty, value);
    }

    public static readonly BindableProperty AllItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(AllItemsSource),
        returnType: typeof(ObservableCollection<IChipModel>),
        defaultValue: new ObservableCollection<IChipModel>(),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWay);

    public ObservableCollection<IChipModel> AllItemsSource
    {
        get => (ObservableCollection<IChipModel>)GetValue(AllItemsSourceProperty);
        set => SetValue(AllItemsSourceProperty, value);
    }

    public static readonly BindableProperty DisplayedItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(DisplayedItemsSource),
        returnType: typeof(ObservableCollection<IBaseChipModel>),
        defaultValue: new ObservableCollection<IBaseChipModel>(),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWayToSource);

    public ObservableCollection<IBaseChipModel> DisplayedItemsSource
    {
        get => (ObservableCollection<IBaseChipModel>)GetValue(DisplayedItemsSourceProperty);
        set => SetValue(DisplayedItemsSourceProperty, value);
    }

    private ICommand _expandCommand;
    public ICommand ExpandCommand => _expandCommand ??= SingleExecutionCommand.FromFunc(OnExpandCommandAsync);

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(AllItemsSource))
        {
            _chipWidths = GetChipWidths();
            _visibleChipsInFirstRow = CalculateDislplayedItemsCountInFirstRow();
            SetDisplayedItemsSource();
        }
        else if (propertyName is nameof(Width))
        {
            _visibleChipsInFirstRow = CalculateDislplayedItemsCountInFirstRow();
            SetDisplayedItemsSource();
        }
    }

    #endregion

    #region -- Private helpers --

    private Task OnExpandCommandAsync()
    {
        IsExpanded = !IsExpanded;

        SetDisplayedItemsSource();

        return Task.CompletedTask;
    }

    private void SetDisplayedItemsSource()
    {
        if (_visibleChipsInFirstRow > 0)
        {
            DisplayedItemsSource = IsExpanded
                ? new(AllItemsSource)
                : new(AllItemsSource.Take(_visibleChipsInFirstRow));

            DisplayedItemsSource.Add(new CheckBindableModel
            {
                IsSelected = IsExpanded,
                TapCommand = ExpandCommand,
            });
        }
    }

    private List<float> GetChipWidths()
    {
        List<float> chipWidths = new();

        foreach (var item in AllItemsSource)
        {
            var chipWidht = StringWidthHelper.CalculateStringWidth(item.Text, item.FontSize, item.FontFamily) + EMPTY_CHIP_WIDTH;

            chipWidths.Add(chipWidht);
        }

        return chipWidths;
    }

    private int CalculateDislplayedItemsCountInFirstRow()
    {
        var visibleChipsInFirstRow = 0;

        if (Width > 0 && _chipWidths.Any())
        {
            var availableWidth = Width - WRAP_BUTTON_WIDTH;
            var accumulateWidth = 0f;

            while ((accumulateWidth + _chipWidths[visibleChipsInFirstRow]) < availableWidth)
            {
                accumulateWidth += _chipWidths[visibleChipsInFirstRow++];
            }
        }

        return visibleChipsInFirstRow;
    }

    #endregion
}