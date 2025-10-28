using System.IO;

public static class SettingsRepository
{
    private const string FileName = "game_settings.json";
    
    public static GameSettingsModel Get()
    {
        if (JsonDataService.TryLoad<GameSettingsModel>(FileName, out var loaded))
        {
            return loaded ?? new GameSettingsModel();
        }
        return new GameSettingsModel();
    }

    public static void Save(GameSettingsModel settings)
    {
        JsonDataService.Save(FileName, settings);
    }
}