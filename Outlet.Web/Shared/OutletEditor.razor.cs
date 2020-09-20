using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Outlet.Web.Shared
{
    public partial class OutletEditor
    {
        private const bool useCodeEditor = false;

        [Parameter]
        public string Text { get; set; } = "";

        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

        private async Task OnTextChanged(ChangeEventArgs e)
        {
            Text = useCodeEditor ? await JSRuntime.InvokeAsync<string>("getCodeEditorValue", Array.Empty<object>()) : e.Value.ToString();
            await TextChanged.InvokeAsync(Text);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (useCodeEditor && firstRender)
            {
                await JSRuntime.InvokeAsync<string>("initCodeEditor", Array.Empty<object>());
            }
        }
    }
}
