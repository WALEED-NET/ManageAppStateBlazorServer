using System.Text.Json;
using ManageAppStateBlazorServer.Implementation;
using ManageAppStateBlazorServer.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ManageAppStateBlazorServer.Components;
public partial class MyCascadingAppState : ComponentBase, IAppState
{

    [Parameter]
    public RenderFragment ChildContent { get; set; }


    private readonly string StorageKey = "MyAppStateKey";

    private readonly int StorageTimeoutInSeconds = 30;

    bool loaded = false;

    [Inject]
    protected ProtectedLocalStorage localStorage { get; set; }

    // used for tracking changes
    // public IAppState GetCopy() => this.MemberwiseClone() as IAppState;
    public IAppState GetCopy()
    {
        IAppState state = (IAppState)this;
        string json = JsonSerializer.Serialize(state);
        AppState copy = JsonSerializer.Deserialize<AppState>(json);
        return copy;
    }
    public DateTime LastStorageSaveTime { get; set; }


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

            // Save to local storage
            new Task(async () =>
            {
                await Save();

            }).Start();
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

            // Save to local storage
            new Task(async () =>
            {
                await Save();

            }).Start();
        }
    }

    protected override void OnInitialized()
    {
        Message = "Initial Message";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Load();
            loaded = true;
            StateHasChanged();
        }
    }

    public async Task Save()
    {
        if (!loaded) return;

        // set LastSaveTime
        LastStorageSaveTime = DateTime.Now;
        // serialize 
        var state = (IAppState)this;
        var json = JsonSerializer.Serialize(state);
        // save
        await localStorage.SetAsync(StorageKey, json);
    }
    public async Task Load()
    {
        try
        {
            var data = await localStorage.GetAsync<string>(StorageKey);
            var state = JsonSerializer.Deserialize<AppState>(data.Value);
            if (state != null)
            {
                if (DateTime.Now.Subtract(state.LastStorageSaveTime).TotalSeconds <= StorageTimeoutInSeconds)
                {
                    // decide whether to set properties manually or with reflection

                    // comment to set properties manually
                    //this.Message = state.Message;
                    //this.Count = state.Count;

                    // set properties using Reflection
                    var t = typeof(IAppState);
                    var props = t.GetProperties();
                    foreach (var prop in props)
                    {
                        if (prop.Name != "LastStorageSaveTime")
                        {
                            object value = prop.GetValue(state);
                            prop.SetValue(this, value, null);
                        }
                    }

                }
            }
        }
        catch (Exception ex)
        {

        }
    }


}
