using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Templates;

public partial class AnnouncementItem : ContentView
{
    public AnnouncementItem()
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
        DescriptionLabel.Text = announcement.Description;
        StartTermLabel.Text = DateTimeOffset.FromUnixTimeSeconds((long)announcement.StartTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
        EndTermLabel.Text = DateTimeOffset.FromUnixTimeSeconds((long)announcement.EndTerm).DateTime.ToString("dd.MM.yyyy HH:mm");
         
        (StatusIconLabel.Text, StatusIconLabel.TextColor, StatusLabel.Text) = Helpers.GetStatusInfo(announcement.Status);

        AnimalLabel.Text = announcement.Animal;
        AddressLabel.Text = announcement.Address;

        if (IsOwnerView)
        {
            EditStack.IsVisible = announcement.Status == StatusType.Created;
            PendingStack.IsVisible = announcement.Status == StatusType.Pending;
            OngoingStack.IsVisible = announcement.Status == StatusType.Ongoing;
            KeeperStack.IsVisible = !string.IsNullOrEmpty(announcement.Keeper);
            KeeperLabel.Text = announcement.Keeper;
        }
        else
        {
            OwnerStack.IsVisible = true;
            OwnerLabel.Text = announcement.Owner;
        }
    }

    private static void OnAnnouncementChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AnnouncementItem component && newValue is AnnouncementInfo announcement)
        {
            component.BindData(announcement);
        };
    }
}