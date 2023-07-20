using CoreGraphics;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using UIKit;

namespace FlyoutPageSample.Platforms.iOS.Renderer
{
    /// <summary>
    /// iOS list view flyout page renderer. With a 50/50 <see cref="FlyoutLayoutBehavior.Split"/> behavior for tablet
    /// and a <see cref="FlyoutLayoutBehavior.Popover"/> behavior for phones.
    /// </summary>
    internal sealed class IOSFlyoutPageRenderer : PhoneFlyoutPageRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOSFlyoutPageRenderer"/> class.
        /// </summary>
        public IOSFlyoutPageRenderer()
        {
            _behaviorWidthThreshold = GetBehaviorWidthThreshold();
            FlyoutOverlapsDetailsInPopoverMode = false;
        }

        /// <inheritdoc />
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Element.SizeChanged += OnSizeChanged;
            Page.IsPresentedChanged += OnIsPresentedChanged;
            AppFlyout.DetailPageChanged += OnDetailPageChanged;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            Element.SizeChanged -= OnSizeChanged;
            Page.IsPresentedChanged -= OnIsPresentedChanged;
            AppFlyout.DetailPageChanged -= OnDetailPageChanged;
            base.Dispose(disposing);
        }

        private FlyoutPage Page => Element as FlyoutPage;
        private AppFlyout AppFlyout => Element as AppFlyout;
        private Page CurrentDetailPage => ((Element as FlyoutPage).Detail as NavigationPage).CurrentPage;
        private readonly double _behaviorWidthThreshold;

        /// <summary>
        /// On size change event handler.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event.</param>
        private void OnDetailPageChanged(object sender, EventArgs eventArgs)
        {
            SetNewLayoutSize();
        }

        /// <summary>
        /// On size change event handler.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event.</param>
        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            var newBehavior = GetNewFlyoutLayoutBehavior();
            SetFlyoutLayoutBehavior(newBehavior);
            SetNewLayoutSize();
        }

        /// <summary>
        /// On is presented change event handler.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="eventArgs">The event.</param>
        private void OnIsPresentedChanged(object sender, EventArgs eventArgs)
        {
            SetPopoverPageSize();
        }

        private FlyoutLayoutBehavior GetNewFlyoutLayoutBehavior()
        {
            FlyoutLayoutBehavior newBehavior;

            if (GetFrame().Width > _behaviorWidthThreshold && DeviceInfo.Current.Idiom == DeviceIdiom.Tablet)
            {
                newBehavior = FlyoutLayoutBehavior.Split;
            }
            else
            {
                newBehavior = FlyoutLayoutBehavior.Popover;
            }

            return newBehavior;
        }

        private void SetFlyoutLayoutBehavior(FlyoutLayoutBehavior newBehavior)
        {
            if (Page.FlyoutLayoutBehavior != newBehavior)
            {
                Page.FlyoutLayoutBehavior = newBehavior;
            }
        }

        private void SetNewLayoutSize()
        {
            if (Page.FlyoutLayoutBehavior == FlyoutLayoutBehavior.Split)
            {
                SetSplitPagesSize();
            }
            else if (Page.FlyoutLayoutBehavior == FlyoutLayoutBehavior.Popover)
            {
                SetPopoverPageSize();
            }
        }

        private void SetSplitPagesSize()
        {
            // The FlyoutPage has a fix ration between the the FlyoutMenu and the DetailPage. DetailPageWidth = 1.25 * FlyoutMenuWidth.
            // The renderer allow us only to set the size of the detail page.
            // In order to have a 50/50 ratio between the FlyoutMenu and the DetailPage we will set a width wider than the screen width
            // and create a padding on the right side of screen to display all the data which need to be displayed.
            var frame = GetFrame();
            var flyoutMenuWidth = frame.Width / 2;
            var detailPageWidth = flyoutMenuWidth * 1.25;
            var totalSize = flyoutMenuWidth + detailPageWidth;
            var detailPageVoidWidth = totalSize - frame.Width;

            SetElementWidth(detailPageWidth);
            CurrentDetailPage.Padding = new Thickness(0, 0, (int)detailPageVoidWidth, 0);
        }

        private void SetPopoverPageSize()
        {
            var frame = GetFrame();
            var flyoutMenuIsShown = Page.IsPresented;
            if (flyoutMenuIsShown)
            {
                // The FlyoutPage has a fix ratio between the the FlyoutMenu and the DetailPage. DetailPageWidth = 1.25 * FlyoutMenuWidth.
                // The renderer allow us only to set the size of the detail page.
                // In order to only see the FlyoutMenu we set a bigger width than needed.
                SetElementWidth(frame.Width * 1.25);
            }
            else
            {
                SetElementWidth(frame.Width);
            }
        }

        private void SetElementWidth(double width)
        {
            SetElementSize(new Size(width, Page.Height));
        }

#pragma warning disable CA1422 // Validate platform compatibility
        private static CGRect GetFrame() => UIApplication.SharedApplication.Windows[0].Frame;
#pragma warning restore CA1422 // Validate platform compatibility

        private static double GetBehaviorWidthThreshold()
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            return (int)(Math.Max(mainDisplayInfo.Width, mainDisplayInfo.Height) / 2) / mainDisplayInfo.Density;
        }
    }
}
