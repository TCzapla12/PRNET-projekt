using CommunityToolkit.Mvvm.Input;
using Grpc.Core;
using PetKeeperMobileApp.Utils;

namespace PetKeeperMobileApp.Templates;

public partial class ErrorItem : ContentView
{
	public ErrorItem()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty ButtonCommandProperty =
        BindableProperty.Create(nameof(ButtonCommand), typeof(RelayCommand), typeof(ErrorItem), null);

    public static readonly BindableProperty ErrorExceptionProperty =
        BindableProperty.Create(nameof(ErrorException), typeof(Exception), typeof(ErrorItem), propertyChanged: OnExceptionChanged);

    public RelayCommand ButtonCommand
	{
		get => (RelayCommand)GetValue(ButtonCommandProperty);
		set => SetValue(ButtonCommandProperty, value);
    }

    public Exception ErrorException
	{
        get => (Exception)GetValue(ErrorExceptionProperty);
        set => SetValue(ErrorExceptionProperty, value);
    }

    public void BindData(Exception exception)
    {
        TitleLabel.Text = Wordings.ERROR;
        if (exception is RpcException rpcException)
        {
            if (rpcException.StatusCode == StatusCode.Unauthenticated)
            {
                DescriptionLabel.Text = Wordings.LOGOUT_DESCRIPTION;
                RefreshButton.Text = Wordings.LOGOUT;
                ButtonCommand = new RelayCommand(async () => await Helpers.Logout());
            }
            else
            {
                DescriptionLabel.Text = rpcException.Status.Detail;
            }
        }
        else
        {
            DescriptionLabel.Text = exception.Message;
        }
    }

    private static void OnExceptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ErrorItem component && newValue is Exception exception)
        {
            component.BindData(exception);
        };
    }
}