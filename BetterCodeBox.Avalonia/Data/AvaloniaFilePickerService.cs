using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Dialogs;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Hosting.Internal;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace BetterCodeBox.Desktop.Data;

public class AvaloniaFilePickerService
{
    public async Task<string?> SelectFile()
    {
        if(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            var mainWindow = lifetime.MainWindow;

            if (mainWindow is not null) 
            {
                var selectedFiles = await mainWindow.StorageProvider.OpenFilePickerAsync(
                    new FilePickerOpenOptions { 
                        AllowMultiple = false, 
                        Title = "select text file" 
                    });
                return selectedFiles.FirstOrDefault()?.TryGetLocalPath();
            }
        }
        return null;
    }
    
    // Same as above, but for saving a file
    public async Task<string?> SaveFile()
    {
        if(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            var mainWindow = lifetime.MainWindow;

            if (mainWindow is not null) 
            {
                var selectedFiles = await mainWindow.StorageProvider.SaveFilePickerAsync(
                    new FilePickerSaveOptions { 
                        Title = "save results file" 
                    });
                return selectedFiles?.TryGetLocalPath();
            }
        }
        return null;
    }
}
