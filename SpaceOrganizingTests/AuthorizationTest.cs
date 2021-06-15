using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpaceOrganizingTests
{
    public static class AuthorizationTest
    {
        public static bool IsAnonymous(Controller controller, string methodName, Type[] methodTypes)
        {
            return GetMethodAttribute<AllowAnonymousAttribute>(controller, methodName, methodTypes) != null ||
                (GetControllerAttribute<AuthorizeAttribute>(controller) == null &&
                    GetMethodAttribute<AuthorizeAttribute>(controller, methodName, methodTypes) == null);

        }

        public static bool IsAuthorized(Controller controller, string methodName, Type[] methodTypes)
        {
            return GetMethodAttribute<AuthorizeAttribute>(controller, methodName, methodTypes) != null ||
                (GetControllerAttribute<AuthorizeAttribute>(controller) != null &&
                    GetMethodAttribute<AllowAnonymousAttribute>(controller, methodName, methodTypes) == null);
        }

        public static bool IsAuthorized(Controller controller, string methodName, Type[] methodTypes, string[] roles)
        {
            if (roles == null)
            {
                Console.WriteLine("roles e gol");
                return IsAuthorized(controller, methodName, methodTypes);
            }

            if (!IsAuthorized(controller, methodName, methodTypes))
            {
                Console.WriteLine("fara autorizatie");
                return false;
            }

            AuthorizeAttribute methodAttribute = GetMethodAttribute<AuthorizeAttribute>(controller, methodName, methodTypes);

            // Check to see if all roles are authorized
            if (roles != null)
            {
                foreach (string role in roles)
                {
                    foreach (string trueRole in methodAttribute.Roles.ToLower().Split(','))
                    {
                        if (trueRole == role.ToLower())
                            return true;
                    }
                }
            }

            return false;
        }

        private static T GetControllerAttribute<T>(Controller controller) where T : Attribute
        {
            Type type = controller.GetType();
            object[] attributes = type.GetCustomAttributes(typeof(T), true);
            T attribute = attributes.Count() == 0 ? null : (T)attributes[0];
            return attribute;
        }

        private static T GetMethodAttribute<T>(Controller controller, string methodName, Type[] methodTypes) where T : Attribute
        {
            Type type = controller.GetType();
            if (methodTypes == null)
            {
                methodTypes = new Type[0];
            }
            MethodInfo method = type.GetMethod(methodName, methodTypes);
            object[] attributes = method.GetCustomAttributes(typeof(T), true);
            T attribute = attributes.Count() == 0 ? null : (T)attributes[0];
            return attribute;
        }
    }
}
