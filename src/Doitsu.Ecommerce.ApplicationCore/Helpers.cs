using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Doitsu.Ecommerce.ApplicationCore
{
    public static class EnumExtensionHelper
    {
        public static string GetDisplayName(this Enum enu)
        {
            var attr = GetDisplayAttribute(enu);
            return attr != null ? attr.Name : enu.ToString();
        }

        public static string GetDescription(this Enum enu)
        {
            var attr = GetDisplayAttribute(enu);
            return attr != null ? attr.Description : enu.ToString();
        }

        private static DisplayAttribute GetDisplayAttribute(object value)
        {
            Type type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }

            // Get the enum field.
            var field = type.GetField(value.ToString());
            return field == null ? null : field.GetCustomAttribute<DisplayAttribute>();
        }
    }

    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }


    }

    public static class ClaimsPrincipalHelper
    {
        public static int TakeUserIdFromClaims(this ClaimsPrincipal claims)
        {
            int.TryParse(claims.FindFirstValue(Constants.ClaimTypeConstants.USER_ID), out var userId);
            if(userId == 0) 
            {
                int.TryParse(claims.FindFirstValue("sub"), out var sub);
                return sub;
            }
            return userId;
        }

        public static string TakeFullNameFromClaims(this ClaimsIdentity claimsIdentity)
        {
            var listClaims = claimsIdentity.Claims.ToImmutableList();
            return listClaims.FirstOrDefault(c => c.Type == Constants.ClaimTypeConstants.FULL_NAME)?.Value ?? string.Empty;
        }
    }

    public static class RazorPageHelper
    {
        public static string IsSelected(this IHtmlHelper htmlHelper, string controllers, string actions, string cssClass = "selected")
        {
            string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedActions = (actions ?? currentAction).Split(',');
            IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }

        public static string MakeContentToShortContentByString(this string data, int length)
        {
            if (data.IsNullOrEmpty()) return data;
            return $"{data.Substring(0, length)}...";
        }

        public static string ParseDecimalToCurrencyVnd(this decimal data)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            return string.Format(cul, "{0:C0}", data);
        }

        public static string ConcatImageResizeConfiguration(this string data, int width = 0, int height = 0, string rmode = "")
        {
            if(data.IsNullOrEmpty()) return data;
            var queryData = new List<string>();
            if(width > 0) queryData.Add($"width={width}");
            if(height > 0) queryData.Add($"height={height}");
            if(rmode.IsNotNullOrEmpty()) queryData.Add($"rmode={rmode}");
            return $"{data}?{queryData.Aggregate((a,b) => $"{a}&{b}")}";
        }
    }

    public static class ExtensionMethods
    {
        public static string GetVietnamDong(this decimal value)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            return value.ToString("#,###", cul.NumberFormat);
        }
    }
}