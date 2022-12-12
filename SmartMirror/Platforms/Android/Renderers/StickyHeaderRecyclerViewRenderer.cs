using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using SmartMirror.ViewModels;
using SmartMirror.ViewModels.Tabs.Pages;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using View = Android.Views.View;

namespace SmartMirror.Platforms.Android.Renderers
{
    //Based on: https://medium.com/@saber.solooki/sticky-header-for-recyclerview-c0eb551c3f68

    //Forms structure: GroupableItemsViewRenderer : SelectableItemsViewRenderer : StructuredItemsViewRenderer : ItemsViewRenderer : RecyclerView
    public class StickyHeaderRecyclerViewRenderer<TItemsView, TAdapter, TItemsViewSource> : GroupableItemsViewRenderer<TItemsView, TAdapter, TItemsViewSource>, IStickyHeaderRecyclerView
        where TItemsView : GroupableItemsView
        where TAdapter : GroupableItemsViewAdapter<TItemsView, TItemsViewSource>
        where TItemsViewSource : IGroupableItemsViewSource
    {
        private GroupableItemsViewAdapter<TItemsView, TItemsViewSource> StickyHeaderAdapter => this.GetAdapter() as GroupableItemsViewAdapter<TItemsView, TItemsViewSource>;

        private Dictionary<int, ImageView> _stickyHeaderCache = new Dictionary<int, ImageView>();

        NotificationsPageViewModel VM => Element?.BindingContext as NotificationsPageViewModel;

        public StickyHeaderRecyclerViewRenderer(Context context) : base(context)
        {
            this.AddItemDecoration(new StickyHeaderRecyclerViewItemDecoration(this));
        }


        public View GetHeaderLayout(int itemPosition)
        {
            var headerPosition = 0;

            for (int i = 0; i < VM.Notifications.Count; i++)
            {
                itemPosition -= VM.Notifications[i].Count;
                itemPosition--;
                if (itemPosition >= 0)
                {
                    headerPosition++;
                }
                else
                {
                    break;
                }
            }

            if (_stickyHeaderCache.ContainsKey(headerPosition))
            {
                return _stickyHeaderCache[headerPosition];
            }

            var recyclerView = (View as RecyclerView);
            var topItemInRecyclerViewIndex = 0;
            var headerView = recyclerView.GetChildAt(topItemInRecyclerViewIndex);
            if (IsHeader(itemPosition))
            {
                var bitmap = Bitmap.CreateBitmap(headerView.Width, headerView.Height, Bitmap.Config.Argb8888);
                var canvas = new Canvas(bitmap);
                headerView.Draw(canvas);

                var imageView = new ImageView(Context);
                imageView.SetImageBitmap(bitmap);
                var layoutParams = new LayoutParams(headerView.Width, headerView.Height);
                imageView.LayoutParameters = layoutParams;
                _stickyHeaderCache.Add(headerPosition, imageView);
                return imageView;
            }
            return default;
        }

        public bool IsHeader(int itemPosition) => StickyHeaderAdapter.GetItemViewType(itemPosition) == Microsoft.Maui.Controls.Compatibility.Platform.Android.ItemViewType.GroupHeader ? true : false;
    }

    public class StickyHeaderRecyclerViewItemDecoration : ItemDecoration
    {
        private IStickyHeaderRecyclerView IStickyHeaderRecyclerView;
        private int _stickyHeaderHeight;

        public StickyHeaderRecyclerViewItemDecoration(IStickyHeaderRecyclerView stickyHeaderRecyclerView) : base()
        {
            IStickyHeaderRecyclerView = stickyHeaderRecyclerView;
        }

        public override void OnDrawOver(Canvas canvas, RecyclerView parent, RecyclerView.State state)
        {
            base.OnDrawOver(canvas, parent, state);
            var topChild = parent.GetChildAt(0);
            if (topChild == null)
            {
                return;
            }

            int topChildPosition = parent.GetChildAdapterPosition(topChild);
            if (topChildPosition == RecyclerView.NoPosition)
            {
                return;
            }

            View currentHeader = GetHeaderViewForItem(topChildPosition, parent);
            if (currentHeader == null)
            {
                return;
            }

            FixLayoutSize(parent, currentHeader);

            int contactPoint = currentHeader.Bottom;
            View childInContact = GetChildInContact(parent, contactPoint, topChildPosition);

            if (childInContact != null && IStickyHeaderRecyclerView.IsHeader(parent.GetChildAdapterPosition(childInContact)))
            {
                MoveHeader(canvas, currentHeader, childInContact);
                return;
            }

            DrawHeader(canvas, currentHeader);
        }

        private View GetChildInContact(RecyclerView parent, int contactPoint, int currentHeaderPosition)
        {
            View childInContact = null;
            for (int i = 0; i < parent.ChildCount; i++)
            {
                var heightTolerance = 0;
                var child = parent.GetChildAt(i);

                if (currentHeaderPosition != i)
                {
                    var isChildHeader = IStickyHeaderRecyclerView.IsHeader(parent.GetChildAdapterPosition(child));
                    if (isChildHeader)
                    {
                        heightTolerance = _stickyHeaderHeight - child.Height;
                    }
                }

                int childBottomPosition;
                if (child.Top > 0)
                {
                    childBottomPosition = child.Bottom + heightTolerance;
                }
                else
                {
                    childBottomPosition = child.Bottom;
                }

                if (childBottomPosition > contactPoint)
                {
                    if (child.Top <= contactPoint)
                    {
                        childInContact = child;
                        break;
                    }
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

        private View GetHeaderViewForItem(int headerPosition, RecyclerView parent)
        {
            if (headerPosition < 0)
            {
                return default;
            }
            var header = IStickyHeaderRecyclerView.GetHeaderLayout(headerPosition);
            return header;
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
    }

    public interface IStickyHeaderRecyclerView
    {
        View GetHeaderLayout(int headerPosition);

        bool IsHeader(int itemPosition);
    }
}
