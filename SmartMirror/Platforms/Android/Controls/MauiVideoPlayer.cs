using Android.Content;
using Android.Media;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using SmartMirror.Controls;
using SmartMirror.Enums;
using SmartMirror.Interfaces;
using Uri = Android.Net.Uri;

namespace SmartMirror.Platforms.Android.Controls
{
    public class MauiVideoPlayer : CoordinatorLayout
    {
        private readonly IVideoController _videoController;
        private readonly Context _context;
        private VideoView _videoView;
        private Video _video;
        private RelativeLayout _relativeLayout;

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
                if (_videoView is not null)
                {
                    _videoView.Prepared -= OnVideoViewPrepared;
                    _videoView.Error -= OnVideoViewError;
                    _videoView.Dispose();
                }
                
                _video = null;
                _videoView = null; 
                _relativeLayout = null;
            }

            base.Dispose(disposing);
        } 

        #endregion

        #region -- Public helpers --

        public void TryUpdateSource()
        {
            if (ChildCount == 0)
            {
                InitVideoPlayer();
            }
            else
            {
                ResetVideoPlayer();
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(_video?.Source))
                {
                    _videoView?.SetVideoURI(Uri.Parse(_video?.Source));
                    
                    _videoController.LoadingState = EVideoLoadingState.Preparing;
                }
            }
            catch
            {
            }
        }

        public void PlayRequested()
        {
            _videoView?.Start();
        }

        public void PauseRequested()
        {
            _videoView?.Pause();
        }

        public void StopRequested()
        {
            _videoView?.StopPlayback();
        }

        #endregion

        #region -- Private helpres --

        private void OnVideoViewPrepared(object sender, EventArgs args)
        {
            _videoController.LoadingState = EVideoLoadingState.Prepared;

            if (_video.Action == EVideoAction.Play)
            {
                _videoView?.Start();
            }
        }

        private void OnVideoViewError(object sender, MediaPlayer.ErrorEventArgs e)
        {
            ResetVideoPlayer();

            _video.VideoPlaybackErrorCommand?.Execute(null);
        }

        private void InitVideoPlayer()
        {
            _videoView = new VideoView(_context)
            {
                LayoutParameters = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent),
            };

            _videoView.Prepared += OnVideoViewPrepared;
            _videoView.Error += OnVideoViewError;

            _relativeLayout = new RelativeLayout(_context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
                {
                    Gravity = (int)GravityFlags.Center,
                },
            };

            _relativeLayout.AddView(_videoView);

            AddView(_relativeLayout);
        }

        private void ResetVideoPlayer()
        {
            _videoController.LoadingState = EVideoLoadingState.Unprepared;

            _videoView?.StopPlayback();
            _videoView?.SetVideoURI(null);

            _relativeLayout?.RemoveView(_videoView);
            _relativeLayout?.AddView(_videoView);

            _videoView?.SetZOrderOnTop(true);
        }

        #endregion
    }
}
