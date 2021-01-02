using Outlet.ForeignFunctions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "http")]
    public static class Http
    {
        [ForeignFunction(Name = "get")]
        public static string Get(string url)
        {
            using var http = new HttpClient();
            var response = Task.Run(async () => await http.GetAsync(url)).Result;
            var content = Task.Run(async () => await response.Content.ReadAsStringAsync()).Result;
            return content;
        }
    }
}
