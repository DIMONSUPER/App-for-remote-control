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

    private int _visibleChipsCountInFirstRow = 0;
    private List<float> _chipsWidths = new();

    public ExpandedMultipleChipSelector()
	{
		InitializeComponent();
	}

    #region -- Public properties --

    public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
        propertyName: nameof(IsExpanded),
        returnType: typeof(bool),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWayToSource);

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

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(ItemsSource),
        returnType: typeof(ObservableCollection<IChipModel>),
        defaultValue: new ObservableCollection<IChipModel>(),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWay);

    public ObservableCollection<IChipModel> ItemsSource
    {
        get => (ObservableCollection<IChipModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
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

    public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(
        propertyName: nameof(ItemSelectedCommand),
        returnType: typeof(ICommand),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWay);

    public ICommand ItemSelectedCommand
    {
        get => (ICommand)GetValue(ItemSelectedCommandProperty);
        set => SetValue(ItemSelectedCommandProperty, value);
    }

    private ICommand _expandCommand;
    public ICommand ExpandCommand => _expandCommand ??= SingleExecutionCommand.FromFunc(OnExpandCommandAsync);

    private ICommand _selectItemCommand;
    public ICommand SelectItemCommand => _selectItemCommand ??= SingleExecutionCommand.FromFunc<object>(OnSelectItemCommandAsync);

    #endregion

    #region -- Overrides --

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(ItemsSource))
        {
            foreach (var item in ItemsSource)
            {
                item.TapCommand = SelectItemCommand;
            }

            CalculateChipsWidths();
            CalculateVisibleChipsCountInFirstRow();
            SetDisplayedItemsSource();
        }
        else if (propertyName is nameof(Width))
        {
            CalculateVisibleChipsCountInFirstRow();
            SetDisplayedItemsSource();
        }
    }

    #endregion

    #region -- Private helpers --

    private Task OnSelectItemCommandAsync(object parameter)
    {
        if (parameter is IChipModel model)
        {
            model.IsSelected = !model.IsSelected;
        }

        if (ItemSelectedCommand is not null && ItemSelectedCommand.CanExecute(parameter))
        {
            ItemSelectedCommand.Execute(parameter);
        }

        return Task.CompletedTask;
    }

    private Task OnExpandCommandAsync()
    {
        IsExpanded = !IsExpanded;

        SetDisplayedItemsSource();

        return Task.CompletedTask;
    }

    private void SetDisplayedItemsSource()
    {
        if (_visibleChipsCountInFirstRow > 0)
        {
            if (_visibleChipsCountInFirstRow == ItemsSource.Count)
            {
                DisplayedItemsSource = new(ItemsSource);
            }
            else
            {
                DisplayedItemsSource = IsExpanded
                    ? new(ItemsSource)
                    : new(ItemsSource.Take(_visibleChipsCountInFirstRow));

                DisplayedItemsSource.Add(new CheckBindableModel
                {
                    IsSelected = IsExpanded,
                    TapCommand = ExpandCommand,
                });
            }
        }
    }

    private void CalculateChipsWidths()
    {
        _chipsWidths = new();

        foreach (var item in ItemsSource)
        {
            var chipWidht = StringWidthHelper.CalculateStringWidth(item.Text, item.FontSize, item.FontFamily) + EMPTY_CHIP_WIDTH;

            _chipsWidths.Add(chipWidht);
        }
    }

    private void CalculateVisibleChipsCountInFirstRow()
    {
        _visibleChipsCountInFirstRow = 0;

        if (Width > 0 && _chipsWidths.Any())
        {
            var availableWidthForItems = Width - WRAP_BUTTON_WIDTH;
            var accumulatedItemsWidth = 0f;

            for (int i = 0; i < ItemsSource.Count && (accumulatedItemsWidth + _chipsWidths[_visibleChipsCountInFirstRow]) < availableWidthForItems; i++)
            {
                accumulatedItemsWidth += _chipsWidths[_visibleChipsCountInFirstRow++];
            }
        }
    }

    #endregion
}