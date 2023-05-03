﻿using System;
using Microsoft.Owin.Hosting;

namespace Mirle.WebAPI.Event.V2BYMA30
{
    public class WebApiHost
    {
        private string _baseAddress = "http://127.0.0.1:9000/";
        private IDisposable _webService;

        public WebApiHost(Startup startup, string sIP)
        {
        //    sIP = "127.0.0.1:9000";
        //    //sIP = "127.0.0.1";
            _baseAddress = $"http://{sIP}/";
            _webService = WebApp.Start(url: _baseAddress, startup: startup.Configuration);
        }

        ~WebApiHost()
        {
            _webService.Dispose();
        }
    }
}