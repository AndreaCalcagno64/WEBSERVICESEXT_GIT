using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //// 1. CORREZIONE CORS: Rimuovi lo slash finale "/" dall'URL
            //// Il browser confronta l'origine esattamente: "https://monitoraggio2127-svil.regione.liguria.it" != "https://monitoraggio2127-svil.regione.liguria.it/"
            //var cors = new EnableCorsAttribute("https://monitoraggio2127-svil.regione.liguria.it", "*", "GET,POST");
            //config.EnableCors(cors);

            // 2. FORZA JSON: Aggiungi questa riga per evitare il formato XML di default
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                // 3. COERENZA ROTTE: Se usi [RoutePrefix] nel controller, 
                // questa rotta di default potrebbe andare in conflitto o richiedere l'Action.
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}