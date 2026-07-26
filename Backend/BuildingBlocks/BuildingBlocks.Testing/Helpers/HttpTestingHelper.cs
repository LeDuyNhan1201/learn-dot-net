using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using BuildingBlocks.SharedKernel.DTOs;
using BuildingBlocks.SharedKernel.Errors.Models;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Testing.Helpers;

public static class HttpTestingHelper
{
    public static Task<T> AssertResponseAsync<T>(
        this HttpResponseMessage response, 
        HttpStatusCode expectedHttpStatusCode, 
        string? expectedErrorCode = null)
    where T : BaseResponse
    {
        var body = response.Content.ReadFromJsonAsync<T>(TestContext.Current.CancellationToken).Result;
        Assert.NotNull(body);
        
        response.StatusCode.Should().Be(expectedHttpStatusCode);
        
        if (expectedErrorCode is null)
        {
            return Task.FromResult(body);
        }
        
        body.Code.Should().Be(expectedErrorCode);
        
        return Task.FromResult(body);
    }
    
    public static Task<CustomValidationResponse> AssertValidationResponseAsync(this HttpResponseMessage response)
    {
        return response.AssertResponseAsync<CustomValidationResponse>(
            HttpStatusCode.BadRequest, ValidationErrors.PrefixCode);
    }
    
    public static void ShouldHaveError<T>(
        this CustomValidationResponse response,
        Expression<Func<T, object?>> property,
        string message)
    {
        var field = GetJsonPropertyName(property);
        response.Errors.Should().ContainKey(field);
        response.Errors[field].Should().Contain(message);
    }

    public static void ShouldHaveError<T>(
        this CustomValidationResponse response,
        Expression<Func<T, object?>> property,
        params string[] messages)
    {
        var field = GetJsonPropertyName(property);

        response.Errors.Should().ContainKey(field);
        response.Errors[field].Should().Contain(messages);
    }

    private static string GetJsonPropertyName<T, TValue>(Expression<Func<T, TValue>> expression)
    {
        if (expression.Body is not MemberExpression member)
        {
            throw new ArgumentException("Expression must be a property access.", nameof(expression));
        }

        var property = (PropertyInfo)member.Member;
        return property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
    }
}