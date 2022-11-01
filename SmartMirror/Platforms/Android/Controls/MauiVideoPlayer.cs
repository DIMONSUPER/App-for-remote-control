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
        private readonly Context _context;
        private VideoView _videoView;
        private Video _video;
        private RelativeLayout _relativeLayout;

        public MauiVideoPlayer(Context context, Video video) 
            : base(context)
        {
            _context = context;
            _video = video;
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

                    _videoView.StopPlayback();
                    _videoView.SetOnPreparedListener(null);
                    _videoView.SetOnCompletionListener(null);
                    _videoView.SetOnErrorListener(null);

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
                    
                    VideoLoadingState = EVideoLoadingState.Preparing;
                }
            }
            catch
            {
            }
        }

        public void PlayRequested()
        {
            if (_videoView is not null && !_videoView.IsPlaying && VideoLoadingState == EVideoLoadingState.Prepared)
            {
                _videoView.Start(); 
            }
        }

        public void PauseRequested()
        {
            if (_videoView is not null && _videoView.IsPlaying)
            {
                _videoView.Pause(); 
            }
        }

        public void StopRequested()
        {
            if (_videoView is not null && _videoView.IsPlaying)
            {
                _videoView.StopPlayback();
                _videoView.Resume();
            }
        }

        #endregion

        #region -- Private helpres --

        private EVideoLoadingState VideoLoadingState
        {
            get => _video is not null
                ? _video.LoadingState
                : EVideoLoadingState.Unprepared;
            set
            {
                if (_video is IVideoController videoController)
                {
                    videoController.LoadingState = value;
                }
            }
        }

        private void OnVideoViewPrepared(object sender, EventArgs args)
        {
            VideoLoadingState = EVideoLoadingState.Prepared;

            if (_video is not null && _video.Action == EVideoAction.Play)
            {
                _videoView?.Start();
            }
        }

        private void OnVideoViewError(object sender, MediaPlayer.ErrorEventArgs e)
        {
            ResetVideoPlayer();

            if (_video?.VideoPlaybackErrorCommand is not null && _video.VideoPlaybackErrorCommand.CanExecute(null))
            {
                _video.VideoPlaybackErrorCommand.Execute(null);
            }
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
            VideoLoadingState = EVideoLoadingState.Unprepared;

            if (_videoView is not null && _videoView.IsPlaying)
            {
                _videoView.StopPlayback();
                _videoView.Resume();
            }

            _videoView?.SetVideoURI(null);

            _relativeLayout?.RemoveView(_videoView);
            _relativeLayout?.AddView(_videoView);

            _videoView?.SetZOrderOnTop(true);
        }

        #endregion
    }
}
