using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace Wp8.Framework.Utils.Behaviors
{
    public class PinchZomBehavior : Behavior<Image>
    {
        private double _totalImageScale = 1d;
        private Point _imagePosition = new Point(0, 0);
        private const double MaxImageZoom = 5;
        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;
        private Image _imgZoom;

        protected override void OnAttached()
        {
            _imgZoom = AssociatedObject;

            _imgZoom.RenderTransform = new CompositeTransform { ScaleX = 1, ScaleY = 1, TranslateX = 0, TranslateY = 0 };
            var listener = GestureService.GetGestureListener(AssociatedObject);
            listener.PinchStarted += OnPinchStarted;
            listener.PinchDelta += OnPinchDelta;
            listener.DragDelta += OnDragDelta;
            listener.DoubleTap += OnDoubleTap;
            base.OnAttached();
        }

        #region Pinch and Zoom Logic


        #region Event handlers

        /// <summary>
        /// Initializes the zooming operation
        /// </summary>
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(_imgZoom, 0);
            _oldFinger2 = e.GetPosition(_imgZoom, 1);
            _oldScaleFactor = 1;
        }

        /// <summary>
        /// Computes the scaling and translation to correctly zoom around your fingers.
        /// </summary>
        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;
            if (!IsScaleValid(scaleFactor))
                return;

            var currentFinger1 = e.GetPosition(_imgZoom, 0);
            var currentFinger2 = e.GetPosition(_imgZoom, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                _imagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImageScale(scaleFactor);
            UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Moves the image around following your finger.
        /// </summary>
        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (IsDragValid(1, translationDelta))
                UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Resets the image scaling and position
        /// </summary>
        private void OnDoubleTap(object sender, GestureEventArgs e)
        {
            ResetImagePosition();
        }

        #endregion

        #region Utils

        /// <summary>
        /// Computes the translation needed to keep the image centered between your fingers.
        /// </summary>
        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
             currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
             currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
             currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
             currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }

        /// <summary>
        /// Updates the scaling factor by multiplying the delta.
        /// </summary>
        private void UpdateImageScale(double scaleFactor)
        {
            _totalImageScale *= scaleFactor;
            ApplyScale();
        }

        /// <summary>
        /// Applies the computed scale to the image control.
        /// </summary>
        private void ApplyScale()
        {
            ((CompositeTransform)_imgZoom.RenderTransform).ScaleX = _totalImageScale;
            ((CompositeTransform)_imgZoom.RenderTransform).ScaleY = _totalImageScale;
        }

        /// <summary>
        /// Updates the image position by applying the delta.
        /// Checks that the image does not leave empty space around its edges.
        /// </summary>
        private void UpdateImagePosition(Point delta)
        {
            var newPosition = new Point(_imagePosition.X + delta.X, _imagePosition.Y + delta.Y);

            if (newPosition.X > 0) newPosition.X = 0;
            if (newPosition.Y > 0) newPosition.Y = 0;

            if ((_imgZoom.ActualWidth * _totalImageScale) + newPosition.X < _imgZoom.ActualWidth)
                newPosition.X = _imgZoom.ActualWidth - (_imgZoom.ActualWidth * _totalImageScale);

            if ((_imgZoom.ActualHeight * _totalImageScale) + newPosition.Y < _imgZoom.ActualHeight)
                newPosition.Y = _imgZoom.ActualHeight - (_imgZoom.ActualHeight * _totalImageScale);

            _imagePosition = newPosition;

            ApplyPosition();
        }

        /// <summary>
        /// Applies the computed position to the image control.
        /// </summary>
        private void ApplyPosition()
        {
            ((CompositeTransform)_imgZoom.RenderTransform).TranslateX = _imagePosition.X;
            ((CompositeTransform)_imgZoom.RenderTransform).TranslateY = _imagePosition.Y;
        }

        /// <summary>
        /// Resets the zoom to its original scale and position
        /// </summary>
        private void ResetImagePosition()
        {
            _totalImageScale = 1;
            _imagePosition = new Point(0, 0);
            ApplyScale();
            ApplyPosition();
        }

        /// <summary>
        /// Checks that dragging by the given amount won't result in empty space around the image
        /// </summary>
        private bool IsDragValid(double scaleDelta, Point translateDelta)
        {
            if (_imagePosition.X + translateDelta.X > 0 || _imagePosition.Y + translateDelta.Y > 0)
                return false;

            if ((_imgZoom.ActualWidth * _totalImageScale * scaleDelta) + (_imagePosition.X + translateDelta.X) < _imgZoom.ActualWidth)
                return false;

            if ((_imgZoom.ActualHeight * _totalImageScale * scaleDelta) + (_imagePosition.Y + translateDelta.Y) < _imgZoom.ActualHeight)
                return false;

            return true;
        }

        /// <summary>
        /// Tells if the scaling is inside the desired range
        /// </summary>
        private bool IsScaleValid(double scaleDelta)
        {
            return (_totalImageScale * scaleDelta >= 1) && (_totalImageScale * scaleDelta <= MaxImageZoom);
        }

        #endregion
        #endregion
    }
}
