using System;

namespace ManageAppStateBlazorServer.Interfaces;

public interface IAppState
{
    string Message { get; set; }
    int Counter { get; set; }

    public DateTime LastStorageSaveTime { get; set; }
}
