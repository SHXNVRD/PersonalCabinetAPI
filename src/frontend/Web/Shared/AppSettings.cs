namespace Web.Shared;

public class AppSettings
{
    private bool _isDarkMode;
    private bool _isDarkModeSystem;

    public event EventHandler? OnChange;

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set => SetValue(ref _isDarkMode, value);
    }

    public bool IsDarkModeSystem
    {
        get => _isDarkModeSystem;
        set => SetValue(ref _isDarkModeSystem, value);
    }

    private void SetValue(ref bool field, bool value)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        OnChange?.Invoke(this, EventArgs.Empty);
    }
}