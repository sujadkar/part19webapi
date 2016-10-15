using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Security.Principal;

namespace EmployeeService
{
	public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			if(actionContext.Request.Headers.Authorization == null)
			{
				actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
			}
			else
			{
				string authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
				string decodedauthenticationtoken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
				string[] usernamepasswordarray = decodedauthenticationtoken.Split(':');
				string username = usernamepasswordarray[0];
				string password = usernamepasswordarray[1];

				if(EmployeeSecurity.Login(username,password)==true)
				{
					Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null); 
				}
				else
				{
					actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
				}
			}
		}
	}
}