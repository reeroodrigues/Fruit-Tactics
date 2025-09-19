using System.IO;

public static class SettingsRepository
{
    private const string FileName = "settings.json";
    private static GameSettingsModel _cache;

    public static GameSettingsModel Get()
    {
        if (_cache != null)
            return _cache;

        if (!JsonDataService.TryLoad(FileName, out _cache))
            _cache = new GameSettingsModel();
        return _cache;
    }

    public static void Save(GameSettingsModel model = null)
    {
        if (model != null)
            _cache = model;
        JsonDataService.Save(FileName, _cache ?? new GameSettingsModel());
    }
}