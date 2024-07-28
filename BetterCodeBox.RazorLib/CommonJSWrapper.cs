using Microsoft.JSInterop;

namespace BetterCodeBox.RazorLib;

public class CommonJSWrapper
{
    private readonly IJSRuntime _jsRuntime;

    public CommonJSWrapper(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    // Save As File
    public async Task SaveAsFile(string filename, byte[] data)
    {
        string base64 = Convert.ToBase64String(data);
        await _jsRuntime.InvokeVoidAsync("saveAsFile", filename, base64);
    }
    
    // Override for Save As File that takes a string
    public async Task SaveAsFile(string filename, string data)
    {
        await SaveAsFile(filename, System.Text.Encoding.UTF8.GetBytes(data));
    }
    
    // Scroll to Element
    public async Task ScrollToElement(string elementId)
    {
        await _jsRuntime.InvokeVoidAsync("scrollToElement", elementId);
    }
}