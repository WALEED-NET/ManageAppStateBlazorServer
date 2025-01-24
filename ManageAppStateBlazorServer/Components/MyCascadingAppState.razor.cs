using System.Text.Json;
using ManageAppStateBlazorServer.Implementation;
using ManageAppStateBlazorServer.Interfaces;
using Microsoft.AspNetCore.Components;

namespace ManageAppStateBlazorServer.Components;
public partial class MyCascadingAppState : ComponentBase, IAppState
{

    [Parameter]
    public RenderFragment ChildContent { get; set; }


    // used for tracking changes
    // public IAppState GetCopy() => this.MemberwiseClone() as IAppState;
    public IAppState GetCopy()
    {
        IAppState state = (IAppState)this;
        string json = JsonSerializer.Serialize(state);
        AppState copy = JsonSerializer.Deserialize<AppState>(json);
        return copy;
    }

    /// <summary>
    /// implemnt property Handler like so
    /// </summary>
    private string _message = "";
    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            StateHasChanged(); // Optionaly Force the component to re-render
        }
    }

    private int count = 0;
    public int Counter
    {
        get => count;
        set
        {
            count = value;
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        Message = "Initial Message";
    }



}
