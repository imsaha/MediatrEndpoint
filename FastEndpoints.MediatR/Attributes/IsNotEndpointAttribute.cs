using System.Reflection;

namespace FastEndpoints.MediatR.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class IsNotEndpointAttribute : Attribute
{
}

public static class FilterNoEndpointsExtension
{
    public static void IgnoreIsNotEndpoints(this EndpointOptions options)
    {
        options.Filter = ep =>
        {
            if (ep.EndpointTags?.Contains("Ignore") is true)
            {
                return false; // don't register this endpoint
            }

            var notEndpoint = ep.EndpointType.GetCustomAttribute<IsNotEndpointAttribute>();
            if (notEndpoint is not null)
            {
                return false; // don't register this endpoint
            }

            return true;
        };
    }
}
