using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using SmartMirror.Controls.Video;
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

        public MauiVideoPlayer(Context context, Video video) : base(context)
        {
            _context = context;
            _video = video;

            RefreshView();
        }

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

        #region -- Public helpres --

        public void UpdateTransportControlsEnabled()
        {
            if (_video.AreTransportControlsEnabled)
            {
                _mediaController = new MediaController(_context);
                _mediaController.SetMediaPlayer(_videoView);
                _videoView.SetMediaController(_mediaController);
            }
            else
            {
                _videoView.SetMediaController(null);

                if (_mediaController != null)
                {
                    _mediaController.SetMediaPlayer(null);
                    _mediaController = null;
                }
            }
        }

        public void UpdateSource()
        {
            RefreshView();

            bool hasSetSource = false;
            string uri = _video.Source;

            _isPrepared = false;

            try
            {
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    _videoView.SetVideoURI(Uri.Parse(uri));
                    hasSetSource = true;
                }
            }
            catch (Exception)
            {
            }

            if (!hasSetSource)
            {
                _videoView.StopPlayback();
                _videoView.Resume();
            }
            else if (hasSetSource && _video.AutoPlay)
            {
                _videoView.Start();
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

        public void PlayRequested(TimeSpan position) => _videoView.Start();

        public void PauseRequested(TimeSpan position) => _videoView.Pause();

        public void StopRequested(TimeSpan position)
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

        private void OnVideoViewPrepared(object sender, EventArgs args)
        {
            _isPrepared = true;
            ((IVideoController)_video).Duration = TimeSpan.FromMilliseconds(_videoView.Duration);
        } 

        #endregion
    }
}
