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
        public string? ID { get; init; }
        public async Task<string> GetValue() => await JSRuntime.InvokeAsync<string>("getCodeEditorValue", new object[] { ID ?? "editor" });

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await JSRuntime.InvokeAsync<string>("highlightCode", new object[] { ID ?? "editor" });
            }
        }


    }
}
