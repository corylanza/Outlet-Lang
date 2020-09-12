using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Outlet.Web.Shared
{
    public partial class OutletEditor
    {
        [Parameter]
        public string Text { get; set; } = "";

        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

        private async Task OnTextChanged(ChangeEventArgs e)
        {
            Text = e.Value.ToString();
            await TextChanged.InvokeAsync(Text);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            //await JSRuntime.InvokeAsync<string>("initCodeEditor", Array.Empty<object>());
        }
    }
}
