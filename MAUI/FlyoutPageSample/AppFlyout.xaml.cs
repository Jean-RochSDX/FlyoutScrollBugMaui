namespace FlyoutPageSample;

public partial class AppFlyout : FlyoutPage
{
    public EventHandler DetailPageChanged { get; set; }

	public AppFlyout()
	{
		InitializeComponent();

        flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = e.CurrentSelection.FirstOrDefault() as FlyoutPageItem;
        if (item != null)
        {
            Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
            DetailPageChanged?.Invoke(this, EventArgs.Empty);
            if (FlyoutLayoutBehavior != FlyoutLayoutBehavior.Split)
            {
                IsPresented = false;
            }
        }
    }
}