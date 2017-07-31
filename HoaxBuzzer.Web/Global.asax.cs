using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using HoaxBuzzer.Web.Business;

namespace HoaxBuzzer.Web
{
    public class Global : HttpApplication
    {

        private static VotingLogic _votingLogic = null;

        public static void UseGlobalVotingLogic(Action<VotingLogic> action)
        {
            lock (_votingLogic.Sync)
            {
                action(_votingLogic);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex is ThreadAbortException)
                return;
            Console.WriteLine(ex.ToString());
            try
            {
                UseGlobalVotingLogic(l =>
                {
                    l.PublishDebugMessage(ex);
                });
            }
            catch
            {
                // dont thorw exceptions in the global exception handler
            }
            
        }



        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            _votingLogic = new VotingLogic();
        }
    }
}