using PetKeeperMobileApp.Enums;
using PetKeeperMobileApp.Models;

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
        SetStatusInfo(announcement.Status);
        AnimalLabel.Text = announcement.Animal;
        AddressLabel.Text = announcement.Address;

        OwnerStack.IsVisible = !string.IsNullOrEmpty(announcement.Owner);
        OwnerLabel.Text = announcement.Owner;

        EditStack.IsVisible = announcement.Status == StatusType.Created && IsOwnerView;
    }

    private static void OnAnnouncementChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is AnnouncementItem component && newValue is AnnouncementInfo announcement)
        {
            component.BindData(announcement);
        };
    }

    private void SetStatusInfo(StatusType status)
    {
        switch (status)
        {
            case StatusType.Created:
                StatusIconLabel.Text = "\uf111";
                StatusIconLabel.TextColor = Color.FromArgb("#064849");
                StatusLabel.Text = "aktualne";
                break;

            case StatusType.Pending:
                StatusIconLabel.Text = "\uf28b";
                StatusIconLabel.TextColor = Color.FromArgb("#EF8159");
                StatusLabel.Text = "z³o¿ono propozycjê";
                break;

            case StatusType.Accepted:
                StatusIconLabel.Text = "\uf058";
                StatusIconLabel.TextColor = Color.FromArgb("#064849");
                StatusLabel.Text = "potwierdzone";
                break;

            case StatusType.Ongoing:
                StatusIconLabel.Text = "\uf144";
                StatusIconLabel.TextColor = Color.FromArgb("#064849");
                StatusLabel.Text = "trwa";
                break;

            case StatusType.Finished:
                StatusIconLabel.Text = "\uf28d";
                StatusIconLabel.TextColor = Colors.Green;
                StatusLabel.Text = "zakoñczone";
                break;

            case StatusType.Canceled:
                StatusIconLabel.Text = "\uf057";
                StatusIconLabel.TextColor = Colors.Red;
                StatusLabel.Text = "anulowane";
                break;

            default:
                StatusIconLabel.Text = "\uf059";
                StatusIconLabel.TextColor = Colors.Red;
                StatusLabel.Text = "nieznany status";
                break;
        }
    }
}