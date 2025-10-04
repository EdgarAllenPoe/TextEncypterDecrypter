using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TextEncrypterDecrypter.Core.Services;

namespace TextEncrypterDecrypter.Core.ViewModels;

/// <summary>
/// Main window view model
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private readonly IEncryptionService _encryptionService;
    private readonly ISettingsService _settingsService;

    private string _text = string.Empty;
    private string _password = string.Empty;
    private string _encryptedText = string.Empty;
    private bool _isLoading = false;
    private string _statusMessage = string.Empty;

           public MainViewModel(IEncryptionService encryptionService, ISettingsService settingsService)
           {
               _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
               _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

               EncryptCommand = new AsyncRelayCommand(EncryptAsync, CanEncrypt);
               DecryptCommand = new AsyncRelayCommand(DecryptAsync, CanDecrypt);
           }

    /// <summary>
    /// The text to encrypt or decrypted text
    /// </summary>
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

    /// <summary>
    /// The password for encryption/decryption
    /// </summary>
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    /// <summary>
    /// The encrypted text result
    /// </summary>
    public string EncryptedText
    {
        get => _encryptedText;
        set => SetProperty(ref _encryptedText, value);
    }

    /// <summary>
    /// Whether an operation is in progress
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Status message for user feedback
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Command to encrypt text
    /// </summary>
    public ICommand EncryptCommand { get; }

    /// <summary>
    /// Command to decrypt text
    /// </summary>
    public ICommand DecryptCommand { get; }

    private bool CanEncrypt()
    {
        return !string.IsNullOrWhiteSpace(Text) && 
               !string.IsNullOrWhiteSpace(Password) && 
               !IsLoading;
    }

    private bool CanDecrypt()
    {
        return !string.IsNullOrWhiteSpace(EncryptedText) && 
               !string.IsNullOrWhiteSpace(Password) && 
               !IsLoading;
    }

    private async Task EncryptAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Encrypting...";
            
            var encrypted = await _encryptionService.EncryptAsync(Text, Password);
            EncryptedText = encrypted;
            StatusMessage = "Text encrypted successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Encryption failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DecryptAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Decrypting...";
            
            var decrypted = await _encryptionService.DecryptAsync(EncryptedText, Password);
            Text = decrypted;
            StatusMessage = "Text decrypted successfully!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Decryption failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

           protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
           {
               if (EqualityComparer<T>.Default.Equals(field, value)) return false;
               field = value;
               OnPropertyChanged(propertyName);
               
               // Update command states when relevant properties change
               if (propertyName == nameof(Text) || propertyName == nameof(Password))
               {
                   ((AsyncRelayCommand)EncryptCommand).RaiseCanExecuteChanged();
               }
               if (propertyName == nameof(EncryptedText) || propertyName == nameof(Password))
               {
                   ((AsyncRelayCommand)DecryptCommand).RaiseCanExecuteChanged();
               }
               
               return true;
           }
}

/// <summary>
/// Async relay command implementation
/// </summary>
public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (() => true);
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && _canExecute();
    }

    public async void Execute(object? parameter)
    {
        if (CanExecute(parameter))
        {
            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute();
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
