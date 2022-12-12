using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using SmartMirror.ViewModels.Tabs.Pages;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using View = Android.Views.View;

namespace SmartMirror.Platforms.Android.Renderers
{
    [Obsolete]
    public class StickyHeaderRecyclerViewRenderer<TItemsView, TAdapter, TItemsViewSource> : GroupableItemsViewRenderer<TItemsView, TAdapter, TItemsViewSource>, IStickyHeaderRecyclerView
        where TItemsView : GroupableItemsView
        where TAdapter : GroupableItemsViewAdapter<TItemsView, TItemsViewSource>
        where TItemsViewSource : IGroupableItemsViewSource
    {
        private GroupableItemsViewAdapter<TItemsView, TItemsViewSource> _stickyHeaderAdapter;
        private GroupableItemsViewAdapter<TItemsView, TItemsViewSource> StickyHeaderAdapter => _stickyHeaderAdapter ??= GetAdapter() as GroupableItemsViewAdapter<TItemsView, TItemsViewSource>;
        
        private NotificationsPageViewModel _notificationsPageViewModel;
        private NotificationsPageViewModel NotificationsPageViewModel => _notificationsPageViewModel ??= Element?.BindingContext as NotificationsPageViewModel;
        
        private readonly Dictionary<int, ImageView> _stickyHeaderCache = new();

        public StickyHeaderRecyclerViewRenderer(Context context) : base(context)
        {
            AddItemDecoration(new StickyHeaderRecyclerViewItemDecoration(this));
        }

        #region -- IStickyHeaderRecyclerView implementation --

        public View GetHeaderLayout(int itemPosition)
        {
            View result = default;
            
            if (itemPosition >= 0)
            {
                var headerPosition = 0;

                for (int i = 0; itemPosition >= 0 && i < NotificationsPageViewModel.Notifications.Count; i++)
                {
                    itemPosition -= NotificationsPageViewModel.Notifications[i].Count;
                    itemPosition--;
                    headerPosition++;
                }

                if (_stickyHeaderCache.TryGetValue(headerPosition, out ImageView value))
                {
                    result = value;
                }
                else
                {
                    var headerView = (View as RecyclerView).GetChildAt(0);

                    if (IsHeader(itemPosition))
                    {
                        var bitmap = Bitmap.CreateBitmap(headerView.Width, headerView.Height, Bitmap.Config.Argb8888);
                        var canvas = new Canvas(bitmap);

                        headerView.Draw(canvas);

                        var imageView = new ImageView(Context);
                        imageView.SetImageBitmap(bitmap);

                        imageView.LayoutParameters = new LayoutParams(headerView.Width, headerView.Height);

                        _stickyHeaderCache.Add(headerPosition, imageView);

                        result = imageView;
                    }
                } 
            }

            return result;
        }

        public bool IsHeader(int itemPosition) => StickyHeaderAdapter.GetItemViewType(itemPosition) == Microsoft.Maui.Controls.Compatibility.Platform.Android.ItemViewType.GroupHeader; 
        
        #endregion
    }

    public class StickyHeaderRecyclerViewItemDecoration : ItemDecoration
    {
        private readonly IStickyHeaderRecyclerView _stickyHeaderRecyclerView;
        private int _stickyHeaderHeight;

        public StickyHeaderRecyclerViewItemDecoration(IStickyHeaderRecyclerView stickyHeaderRecyclerView) : base()
        {
            _stickyHeaderRecyclerView = stickyHeaderRecyclerView;
        }

        #region -- Overrides --

        public override void OnDrawOver(Canvas canvas, RecyclerView parent, RecyclerView.State state)
        {
            base.OnDrawOver(canvas, parent, state);

            if (parent.GetChildAt(0) is View topChild)
            {
                int topChildPosition = parent.GetChildAdapterPosition(topChild);

                Console.WriteLine(topChildPosition);
                
                if (topChildPosition != RecyclerView.NoPosition && _stickyHeaderRecyclerView.GetHeaderLayout(topChildPosition) is View currentHeader)
                {
                    FixLayoutSize(parent, currentHeader);

                    int contactPoint = currentHeader.Bottom;
                    View childInContact = GetChildInContact(parent, contactPoint, topChildPosition);

                    if (childInContact != null && _stickyHeaderRecyclerView.IsHeader(parent.GetChildAdapterPosition(childInContact)))
                    {
                        MoveHeader(canvas, currentHeader, childInContact);
                    }
                    else
                    {
                        DrawHeader(canvas, currentHeader);
                    }
                }
            }
        } 

        #endregion

        #region -- Private helpers --

        private View GetChildInContact(RecyclerView parent, int contactPoint, int currentHeaderPosition)
        {
            View childInContact = null;

            for (int i = 0; i < parent.ChildCount; i++)
            {
                var heightTolerance = 0;
                var child = parent.GetChildAt(i);

                if (currentHeaderPosition != i)
                {
                    var isChildHeader = _stickyHeaderRecyclerView.IsHeader(parent.GetChildAdapterPosition(child));

                    if (isChildHeader)
                    {
                        heightTolerance = _stickyHeaderHeight - child.Height;
                    }
                }

                int childBottomPosition = child.Bottom;

                if (child.Top > 0)
                {
                    childBottomPosition += heightTolerance;
                }

                if (childBottomPosition > contactPoint && child.Top <= contactPoint)
                {
                    childInContact = child;
                    break;
                }
            }

            return childInContact;
        }

        private void FixLayoutSize(ViewGroup parent, View view)
        {
            if (view != null)
            {
                //specs for parent (recycler view)
                var widthSpec = View.MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly);
                var heightSpec = View.MeasureSpec.MakeMeasureSpec(parent.Height, MeasureSpecMode.Unspecified);

                //specs for children (headers)
                var childWidthSpec = ViewGroup.GetChildMeasureSpec(widthSpec, parent.PaddingLeft + parent.PaddingRight, view.LayoutParameters?.Width ?? 0);
                var childHeightSpec = ViewGroup.GetChildMeasureSpec(heightSpec, parent.PaddingTop + parent.PaddingBottom, view.LayoutParameters?.Height ?? 0);

                view.Measure(childWidthSpec, childHeightSpec);

                _stickyHeaderHeight = view.MeasuredHeight;

                view.Layout(0, 0, view.MeasuredWidth, _stickyHeaderHeight);
            }
        }

        private void MoveHeader(Canvas canvas, View currentHeader, View nextHeader)
        {
            canvas.Save();
            canvas.Translate(0, nextHeader.Top - currentHeader.Height);
            currentHeader.Draw(canvas);
            canvas.Restore();
        }

        private void DrawHeader(Canvas canvas, View header)
        {
            canvas.Save();
            canvas.Translate(0, 0);
            header.Draw(canvas);
            canvas.Restore();
        } 

        #endregion
    }

    public interface IStickyHeaderRecyclerView
    {
        View GetHeaderLayout(int headerPosition);

        bool IsHeader(int itemPosition);
    }
}