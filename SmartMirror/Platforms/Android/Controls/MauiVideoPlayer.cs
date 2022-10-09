using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using SmartMirror.Controls;
using SmartMirror.Enums;
using SmartMirror.Interfaces;
using Color = Android.Graphics.Color;
using Uri = Android.Net.Uri;

namespace SmartMirror.Platforms.Android.Controls
{
    public class MauiVideoPlayer : CoordinatorLayout
    {
        private readonly Context _context;
        private VideoView _videoView;
        private Video _video;
        private IVideoController _videoController;

        public MauiVideoPlayer(Context context, Video video) : base(context)
        {
            _context = context;
            _video = video;
            _videoController = video;
        }

        #region -- Overrides --

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _videoView.Prepared -= OnVideoViewPrepared;
                _videoView.Dispose();
                _videoView = null;
                _video = null;
            }

            base.Dispose(disposing);
        } 

        #endregion

        #region -- Public helpers --

        public void TryUpdateSource()
        {
            bool hasSetSource = false;
            
            RefreshView();

            try
            {
                if (!string.IsNullOrWhiteSpace(_video.Source))
                {

                    _videoView.SetVideoURI(Uri.Parse(_video.Source));

                    hasSetSource = true;
                }
            }
            catch
            {
            }

            if (hasSetSource)
            {
                if (_video.Action == EVideoAction.Play)
                {
                    _videoController.LoadingState = EVideoLoadingState.Preparing;
                    
                    _videoView.Start();
                }
            }
            else
            {
                _videoController.LoadingState = EVideoLoadingState.Unprepared;

                _videoView.StopPlayback();
                _videoView.Resume();
            }
        }

        public void PlayRequested()
        {
            _videoView.Start();
        }

        public void PauseRequested()
        {
            _videoView.Pause();
        }

        public void StopRequested()
        {
            _videoView.StopPlayback();

            _videoView.Resume();
        }

        #endregion

        #region -- Private helpres --

        private void RefreshView()
        {
            RemoveViews(0, ChildCount);

            if (_videoView != null)
            {
                _videoView.Prepared -= OnVideoViewPrepared;
                _videoView.Dispose();
            }

            _videoView = new VideoView(_context)
            {
                LayoutParameters = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent),
            };

            _videoView.Prepared += OnVideoViewPrepared;

            var relativeLayout = new RelativeLayout(_context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                {
                    Gravity = (int)GravityFlags.Center
                }
            };

            SetBackgroundColor(Color.Black);

            relativeLayout.AddView(_videoView);

            AddView(relativeLayout);
        }

        private void OnVideoViewPrepared(object sender, EventArgs args) => _videoController.LoadingState = EVideoLoadingState.Prepared; 

        #endregion
    }
}
