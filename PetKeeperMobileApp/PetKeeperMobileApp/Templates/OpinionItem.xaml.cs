using PetKeeperMobileApp.Models;

namespace PetKeeperMobileApp.Templates;

public partial class OpinionItem : ContentView
{
	public OpinionItem()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty IsMyViewProperty =
        BindableProperty.Create(nameof(IsMyView), typeof(bool), typeof(OpinionItem), false);

    public static readonly BindableProperty OpinionProperty =
        BindableProperty.Create(nameof(Opinion), typeof(OpinionInfo), typeof(OpinionItem), null, propertyChanged: OnOpinionChanged);

    public bool IsMyView
    {
        get => (bool)GetValue(IsMyViewProperty);
        set => SetValue(IsMyViewProperty, value);
    }

    public OpinionInfo Opinion
    {
        get => (OpinionInfo)GetValue(OpinionProperty);
        set => SetValue(OpinionProperty, value);
    }

    public void BindData(OpinionInfo opinion)
    {
        if (IsMyView)
        {
            UserLabel.Text = opinion.AuthorInfo!.Username;
            Photo.Source = opinion.AuthorInfo!.Photo;
        }
        else
        {
            UserLabel.Text = opinion.KeeperInfo!.Username;
            Photo.Source = opinion.KeeperInfo!.Photo;
        }
        ScoreLabel.Text = opinion.Rating.ToString();
        if (string.IsNullOrEmpty(opinion.Description))
            DescriptionLabel.IsVisible = false;
        DescriptionLabel.Text = opinion.Description;
        TermLabel.Text = DateTimeOffset.FromUnixTimeSeconds((long)opinion.CreatedDate).DateTime.ToString("dd.MM.yyyy HH:mm"); ;
    }

    private static void OnOpinionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is OpinionItem component && newValue is OpinionInfo opinion)
        {
            component.BindData(opinion);
        };
    }
}