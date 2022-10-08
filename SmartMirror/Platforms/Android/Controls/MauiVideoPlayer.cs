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
        private VideoView _videoView;
        private MediaController _mediaController;
        private Context _context;
        private Video _video;
        private bool _isPrepared;
        private bool _isNeecessaryToPlay;

        public MauiVideoPlayer(Context context, Video video) : base(context)
        {
            _context = context;
            _video = video;
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

        public void UpdateTransportControlsEnabled()
        {
            if (_video.IsTransportControlsEnabled)
            {
                _mediaController = new MediaController(_context);
                _mediaController.SetMediaPlayer(_videoView);
                _videoView?.SetMediaController(_mediaController);
            }
            else
            {
                _videoView?.SetMediaController(null);

                if (_mediaController != null)
                {
                    _mediaController.SetMediaPlayer(null);
                    _mediaController = null;
                }
            }
        }

        public void UpdateSource()
        {
            bool hasSetSource = _isPrepared = false;
            
            RefreshView();

            try
            {
                if (!string.IsNullOrWhiteSpace(_video.Source))
                {
                    _videoView.SetVideoURI(Uri.Parse(_video.Source));

                    hasSetSource = true;
                }
            }
            catch (Exception)
            {
            }

            if (hasSetSource)
            {
                if (_isNeecessaryToPlay || _video.IsAutoPlay)
                {
                    _videoView.Start();
                }
            }
            else
            {
                _videoView.StopPlayback();
                _videoView.Resume();
            }
        }

        public void UpdatePosition()
        {
            if (Math.Abs(_videoView.CurrentPosition - _video.Position.TotalMilliseconds) > 1000)
            {
                _videoView.SeekTo((int)_video.Position.TotalMilliseconds);
            }
        }

        public void UpdateStatus()
        {
            EVideoStatus status = EVideoStatus.NotReady;

            if (_isPrepared)
            {
                status = _videoView.IsPlaying
                    ? EVideoStatus.Playing
                    : EVideoStatus.Paused;
            }

            ((IVideoController)_video).Status = status;

            _video.Position = TimeSpan.FromMilliseconds(_videoView.CurrentPosition);
        }

        public void PlayRequested(TimeSpan position)
        {
            _isNeecessaryToPlay = true;

            _videoView.Start();
        }

        public void PauseRequested(TimeSpan position)
        {
            _isNeecessaryToPlay = false;

            _videoView.Pause();
        }

        public void StopRequested(TimeSpan position)
        {
            _isNeecessaryToPlay = false;

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

        private void OnVideoViewPrepared(object sender, EventArgs args)
        {
            _isPrepared = true;
        } 

        #endregion
    }
}
