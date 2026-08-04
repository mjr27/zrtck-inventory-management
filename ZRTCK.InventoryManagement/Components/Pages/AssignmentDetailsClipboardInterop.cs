using Microsoft.JSInterop;

namespace ZRTCK.InventoryManagement.Components.Pages;

public sealed class AssignmentDetailsClipboardInterop : IAsyncDisposable
{
    private const string ModulePath = "./Components/Pages/AssignmentDetails.razor.js";
    private const string CopyMethod = "copy";

    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public AssignmentDetailsClipboardInterop(IJSRuntime js) => _js = js;

    public async ValueTask CopyAsync(string text)
    {
        _module ??= await _js.InvokeAsync<IJSObjectReference>("import", ModulePath);
        await _module.InvokeVoidAsync(CopyMethod, text);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module != null)
                await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
