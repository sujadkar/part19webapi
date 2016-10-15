using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using EmployeeDataAccess;

namespace EmployeeService.Controllers
{
	[EnableCorsAttribute("*","*","*")]

	public class EmployeesController : ApiController
    {
		//[HttpGet]
		//public IEnumerable<Employee> GetSomething()
		//{
		//	using (EmployeeDBEntities entites = new EmployeeDBEntities())
		//	{
		//		return entites.Employees.ToList();
		//	}
		//}

		[BasicAuthentication]
		public HttpResponseMessage Get(string gender = "All")
		{
			string username = Thread.CurrentPrincipal.Identity.Name;
			using (EmployeeDBEntities entites = new EmployeeDBEntities())
			{
				switch (username.ToLower())
				{
			//		case "all": return Request.CreateResponse(HttpStatusCode.OK, entites.Employees.ToList());
						
					case "male": return Request.CreateResponse(HttpStatusCode.OK, entites.Employees.Where(e => e.Gender.Equals("male")).ToList());
						
					case "female":return Request.CreateResponse(HttpStatusCode.OK, entites.Employees.Where(e => e.Gender.Equals("female")).ToList());
					default:
						return Request.CreateResponse(HttpStatusCode.BadRequest);
						//return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"Value for gender must be all,male or female." + gender +" is invalid."); 				
				}		
			}
		}

		public HttpResponseMessage Get(int id)
		{
			using (EmployeeDBEntities entites = new EmployeeDBEntities())
			{
				var entity = entites.Employees.FirstOrDefault(e => e.ID == id);
				if(entity != null)
				{
					return Request.CreateResponse(HttpStatusCode.OK, entity);
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.NotFound, "Employee with id = " + id.ToString() + "Not Found");
				}	
			}
		}

		public HttpResponseMessage Post([FromBody]Employee employee)
		{
			try
			{
				using (EmployeeDBEntities entites = new EmployeeDBEntities())
				{
					entites.Employees.Add(employee);
					entites.SaveChanges();

					var message = Request.CreateResponse(HttpStatusCode.Created, employee);
					message.Headers.Location = new Uri(Request.RequestUri + employee.ID.ToString());
					return message;
				}
			}
			catch (Exception exp)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exp);
			}
			
		}

		public HttpResponseMessage Delete(int id)
		{
			try
			{
				using (EmployeeDBEntities entities = new EmployeeDBEntities())
				{
					var entity = entities.Employees.FirstOrDefault(e => e.ID.Equals(id));
					if (entity == null)
					{
						return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with Id" + id.ToString() + " Not Found to delete");
					}
					else
					{
						entities.Employees.Remove(entity);
						entities.SaveChanges();
						return Request.CreateResponse(HttpStatusCode.OK);
					}
				}
			}
			catch(Exception exp)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exp);
			}
		}

		public HttpResponseMessage Put(int id,[FromBody] Employee employee)
		{
			try
			{
				using (EmployeeDBEntities entities = new EmployeeDBEntities())
				{
					var entity = entities.Employees.FirstOrDefault(e => e.ID.Equals(id));
					if (entity == null)
					{
						return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with Id" + id.ToString() + "Not Found");
					}
					else
					{
						entity.FirstName = employee.FirstName;
						entity.LastName = employee.LastName;
						entity.Salary = employee.Salary;
						entities.SaveChanges();
						return Request.CreateResponse(HttpStatusCode.OK);
					}
				}
			}
			catch(Exception exp)
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exp);
			}
		}
	}
}
