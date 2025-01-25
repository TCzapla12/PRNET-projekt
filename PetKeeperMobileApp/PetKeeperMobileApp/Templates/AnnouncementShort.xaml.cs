using PetKeeperMobileApp.Models;

namespace PetKeeperMobileApp.Templates;

public partial class AnnouncementShort : ContentView
{
	public AnnouncementShort()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty IsOwnerViewProperty =
        BindableProperty.Create(nameof(IsOwnerView), typeof(bool), typeof(AnnouncementItem), false);

    public static readonly BindableProperty AnnouncementProperty =
        BindableProperty.Create(nameof(Announcement), typeof(AnnouncementInfo), typeof(AnnouncementItem), null, propertyChanged: OnAnnouncementChanged);

    public bool IsOwnerView
    {
        get => (bool)GetValue(IsOwnerViewProperty);
        set => SetValue(IsOwnerViewProperty, value);
    }

    public AnnouncementInfo Announcement
    {
        get => (AnnouncementInfo)GetValue(AnnouncementProperty);
        set => SetValue(AnnouncementProperty, value);
    }

    private string annId;

    public string AnnId
    {
        get => annId;
        set
        {
            if (annId != value)
            {
                annId = value;
                OnPropertyChanged();
            }
        }
    }

    public void BindData(AnnouncementInfo announcement)
    {
        AnnId = announcement.Id!;
        ProfitLabel.Text = announcement.Profit.ToString();
        NegotiableLabel.IsVisible = announcement.IsNegotiable;
        StartTermLabel.Text = DateTimeOffset.FromUnixTimeSeconds((long)announcement.StartTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
        EndTermLabel.Text = DateTimeOffset.FromUnixTimeSeconds((long)announcement.EndTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
        AddressLabel.Text = announcement.Address;
        if (IsOwnerView) OngoingStack.IsVisible = true;
        if (IsOwnerView) TypeImage.Source = "paw.jpg";
        else TypeImage.Source = "briefcase.jpg";
    }

    private static void OnAnnouncementChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AnnouncementShort component && newValue is AnnouncementInfo announcement)
        {
            component.BindData(announcement);
        };
    }
}