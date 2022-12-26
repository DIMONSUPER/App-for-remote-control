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
using System.Linq;

namespace SmartMirror.Controls;

public partial class ExpandedMultipleChipSelector : Grid
{
    private const float EMPTY_CHIP_WIDTH = 88;
    private const float EMPTY_CHIP_WIDTH_WHEN_SELECTED = 108;
    private const float CHIP_WIDTH_DIFFERENT = EMPTY_CHIP_WIDTH_WHEN_SELECTED - EMPTY_CHIP_WIDTH;
    private const float HORIZONTAL_CHIPS_SPACING = 18;
    private const float EXPAND_BUTTON_WIDTH = 52;

    private List<float> _chipsWidths = new();
    private int _chipsCountInFirstRow = 0;

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

    public static readonly BindableProperty ItemTemplateSelectorProperty = BindableProperty.Create(
        propertyName: nameof(ItemTemplateSelector),
        returnType: typeof(DataTemplateSelector),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWay);

    public DataTemplateSelector ItemTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
        set => SetValue(ItemTemplateSelectorProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(ItemsSource),
        returnType: typeof(ObservableCollection<ISelectableTextModel>),
        defaultValue: new ObservableCollection<ISelectableTextModel>(),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWay);

    public ObservableCollection<ISelectableTextModel> ItemsSource
    {
        get => (ObservableCollection<ISelectableTextModel>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty DisplayedItemsSourceProperty = BindableProperty.Create(
        propertyName: nameof(DisplayedItemsSource),
        returnType: typeof(ObservableCollection<IBaseSelectableModel>),
        defaultValue: new ObservableCollection<IBaseSelectableModel>(),
        declaringType: typeof(ExpandedMultipleChipSelector),
        defaultBindingMode: BindingMode.OneWayToSource);

    public ObservableCollection<IBaseSelectableModel> DisplayedItemsSource
    {
        get => (ObservableCollection<IBaseSelectableModel>)GetValue(DisplayedItemsSourceProperty);
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
    public ICommand ExpandCommand => _expandCommand ??= SingleExecutionCommand.FromFunc(OnExpandCommandAsync, delayMillisec: 0);

    private ICommand _selectItemCommand;
    public ICommand SelectItemCommand => _selectItemCommand ??= SingleExecutionCommand.FromFunc<ISelectableTextModel>(OnSelectItemCommandAsync);

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
            CalculateChipsCountInFirstRow();
            SetDisplayedItemsSource();
        }
        else if (propertyName is nameof(Width) && _chipsCountInFirstRow == 0)
        {
            CalculateChipsCountInFirstRow();
            SetDisplayedItemsSource();
        }
    }

    #endregion

    #region -- Private helpers --

    private Task OnSelectItemCommandAsync(ISelectableTextModel selectedModel)
    {
        if (selectedModel is not null)
        {
            selectedModel.IsSelected = !selectedModel.IsSelected;

            if (ItemSelectedCommand is not null && ItemSelectedCommand.CanExecute(selectedModel))
            {
                ItemSelectedCommand.Execute(selectedModel);
            }

            RecalculateChipWidthByModel(selectedModel);
            CalculateChipsCountInFirstRow();
            SetDisplayedItemsSource();
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
        if (_chipsCountInFirstRow > 0)
        {
            if (_chipsCountInFirstRow == ItemsSource.Count)
            {
                DisplayedItemsSource = new(ItemsSource);
            }
            else
            {
                DisplayedItemsSource = IsExpanded
                    ? new(ItemsSource)
                    : new(ItemsSource.Take(_chipsCountInFirstRow));

                DisplayedItemsSource.Add(new SelectedBindableModel
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
            var chipWidth = StringWidthHelper.CalculateStringWidth(item.Text, item.FontSize, item.FontFamily) + HORIZONTAL_CHIPS_SPACING;

            chipWidth += item.IsSelected
                ? EMPTY_CHIP_WIDTH_WHEN_SELECTED
                : EMPTY_CHIP_WIDTH;

            _chipsWidths.Add(chipWidth);
        }
    }

    private void RecalculateChipWidthByModel(ISelectableTextModel model)
    {
        var modelIndex = ItemsSource.IndexOf(model);

        if (modelIndex > -1)
        {
            _chipsWidths[modelIndex] -= model.IsSelected
                ? -CHIP_WIDTH_DIFFERENT
                : CHIP_WIDTH_DIFFERENT;
        }
    }

    private void CalculateChipsCountInFirstRow()
    {
        _chipsCountInFirstRow = 0;

        if (Width > 0 && _chipsWidths.Any())
        {
            var accumulatedWidthForChips = 0f;
            var isChipsFitInFirstRow = true;

            for (int i = 0; isChipsFitInFirstRow && i < _chipsWidths.Count; i++)
            {
                var widthForChips = accumulatedWidthForChips + _chipsWidths[_chipsCountInFirstRow];

                isChipsFitInFirstRow = widthForChips < Width;

                if (isChipsFitInFirstRow)
                {
                    ++_chipsCountInFirstRow;

                    accumulatedWidthForChips = widthForChips;
                }
            }

            if (_chipsCountInFirstRow < ItemsSource.Count && accumulatedWidthForChips + EXPAND_BUTTON_WIDTH > Width)
            {
                --_chipsCountInFirstRow;
            }
        }
    }

    #endregion
}