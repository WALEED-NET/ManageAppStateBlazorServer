using System;
using ManageAppStateBlazorServer.Interfaces;

namespace ManageAppStateBlazorServer.Implementation;

public class AppState : IAppState
{
    public string Message { get; set; } = "";
    public int Counter { get; set; }

}
