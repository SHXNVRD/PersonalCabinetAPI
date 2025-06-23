using Blazored.LocalStorage;

namespace Web.Services;

public class ThemeService
{
    private const string DarkModeSettingKey = "IsDarkMode";

    private bool _isInitialized;
    
    private bool _isDarkMode;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (_isDarkMode == value)
                return;

            _isDarkMode = value;
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public event EventHandler? ThemeChanged;

    private readonly ILocalStorageService _localStorage;

    public ThemeService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        ThemeChanged += OnThemeChangedAsync;
    }

    public async Task InitializeAsync(bool useDarkModeAsDefault = true)
    {
        if (!_isInitialized)
        {
            if (await _localStorage.ContainKeyAsync(DarkModeSettingKey))
                IsDarkMode = await _localStorage.GetItemAsync<bool>(DarkModeSettingKey);
            else
                IsDarkMode = useDarkModeAsDefault;
            
            _isInitialized = true;
        }
    }

    private async void OnThemeChangedAsync(object? sender, EventArgs e)
        => await _localStorage.SetItemAsync(DarkModeSettingKey, IsDarkMode);
}