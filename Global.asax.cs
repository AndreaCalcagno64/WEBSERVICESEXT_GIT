using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

namespace WebApplication1
{
    public class Global : HttpApplication
    {
        // 1. Definiamo la lista degli URL autorizzati
        private readonly string[] _allowedOrigins = {
            "https://monitoraggio2127-svil.regione.liguria.it",
            "https://cdpbo-test.regione.liguria.it", // Esempio: produzione
            "https://cdpbo.regione.liguria.it", // Esempio: produzione
            "http://localhost:3934"                         // Esempio: sviluppo locale
        };

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var origin = Context.Request.Headers["Origin"];

            // 2. Verifichiamo se l'origin della richiesta è nella nostra whitelist
            bool isAllowed = false;
            if (!string.IsNullOrEmpty(origin))
            {
                foreach (var allowed in _allowedOrigins)
                {
                    if (origin.Equals(allowed, StringComparison.OrdinalIgnoreCase))
                    {
                        isAllowed = true;
                        break;
                    }
                }
            }

            if (Context.Request.HttpMethod == "OPTIONS")
            {
                Context.Response.ClearHeaders();

                if (isAllowed)
                {
                    // Se l'URL è autorizzato, lo restituiamo dinamicamente
                    Context.Response.AddHeader("Access-Control-Allow-Origin", origin);
                    Context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization, X-Requested-With");
                    Context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                    Context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
                }

                Context.Response.StatusCode = 200;
                Context.ApplicationInstance.CompleteRequest();
            }
            else if (isAllowed)
            {
                // Anche per le chiamate reali (GET/POST), aggiungiamo l'origin verificato
                Context.Response.AddHeader("Access-Control-Allow-Origin", origin);
                Context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            }
        }
    }
}