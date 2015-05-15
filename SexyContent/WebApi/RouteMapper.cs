﻿using System.Web.Http;
using System.Web.Http.Dispatcher;
using DotNetNuke.Web.Api;
using Microsoft.Practices.Unity;
using ToSic.SexyContent.WebApi;

namespace ToSic.SexyContent
{
    public class RouteMapper : IServiceRouteMapper
    {

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            // Route Concept
            // starting with eav means it's a rather low-level admin function, always needs an AppId
            // eav
            // eav-???
            // starting with app means that it's a app-specific action, more for the JS developers working with content
            // app-content  will do basic content-actions like get one, edit, update, delete
            // app-query    will try to request a query
            // app-api      will call custom c# web-apis of a specific app



            mapRouteManager.MapHttpRoute("2sxc", "EAV", "EAV/{controller}/{action}", new[] { "ToSic.SexyContent.EAVExtensions.EavApiProxies" });

            mapRouteManager.MapHttpRoute("2sxc", "app-content", "app-content/{contenttype}/{id}", new { controller = "AppContent", id = RouteParameter.Optional }, new[] { "ToSic.SexyContent.WebApi" });

            mapRouteManager.MapHttpRoute("2sxc", "View", "View/{controller}/{action}", new[] { "ToSic.SexyContent.ViewAPI" });
            mapRouteManager.MapHttpRoute("2sxc", "app-api", "app-api/{controller}/{action}", new[] { "ToSic.SexyContent.Apps" });
            mapRouteManager.MapHttpRoute("2sxc", "app-query", "app-query/{name}", new { controller = "Query"}, new[] { "ToSic.SexyContent.ViewAPI" });
            mapRouteManager.MapHttpRoute("2sxc", "default", "{controller}/{action}", new[] { "ToSic.SexyContent.GettingStarted" });

            var config = GlobalConfiguration.Configuration;
            var previousSelector = config.Services.GetService(typeof(IHttpControllerSelector)) as IHttpControllerSelector;
            config.Services.Replace(typeof(IHttpControllerSelector), new AppApiControllerSelector(config) { PreviousSelector = previousSelector });

            // Also register Unity Dependency-Injection here, since this will certainly run once early during bootup
            var cont = Eav.Factory.Container; 
            cont.RegisterType(typeof(Eav.Serializers.Serializer), typeof(Serializers.Serializer), new InjectionConstructor());//, null, null, null);
        }

    }
}