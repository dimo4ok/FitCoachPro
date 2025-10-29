using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitCoachPro.Application.Common.Models.Response
{
    public class Result
    {
        public HttpType HttpMethodType { get; init; }
        public bool IsSuccess { get; init; }
        public int StatusCode { get; init; }
        public object? Data { get; init; } = null;
        public List<Error>? Errors { get; init; } = new();

        public static Result Success(HttpType httpMethodType = HttpType.GET, int statusCode = 200)
            => new Result
            {
                HttpMethodType = httpMethodType,
                IsSuccess = true,
                StatusCode = statusCode
            };

        public static Result Fail(List<Error> errors, HttpType httpMethodType = HttpType.GET, int statusCode = 404)
            => new Result
            {
                HttpMethodType = httpMethodType,
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = errors
            };
    }

    public class Result<T> : Result
    {
        [JsonPropertyOrder(3)]
        public new T? Data { get; init; }

        public static Result<T> Success(T data, HttpType httpMethodType = HttpType.GET, int statusCode = 200)
            => new Result<T>
            {
                HttpMethodType = httpMethodType,
                IsSuccess = true,
                StatusCode = statusCode,
                Data = data
            };

        new public static Result<T> Fail(List<Error> errors, HttpType httpMethodType = HttpType.GET, int statusCode = 404)
            => new Result<T>
            {
                HttpMethodType = httpMethodType,
                IsSuccess = false,
                StatusCode = statusCode,
                Errors = errors
            };

        //public static Result<T> FromResult<U>(Result<U> result, T? data)
        //{
        //    if (result.IsSuccess)
        //        return Result<T>.Success(data, result.HttpMethodType, result.StatusCode);
        //    else
        //        return Result<T>.Fail(result.Errors, result.HttpMethodType, result.StatusCode);
        //}

        public static Result<T> FromResult<U>(Result<U> result)
            => result.IsSuccess
                ? Result<T>.Success(default!, result.HttpMethodType)
                : Result<T>.Fail(result.Errors, result.HttpMethodType, result.StatusCode);

        public static Result<T> FromResult<U>(Result<U> result, T data)
            => result.IsSuccess
                ? Result<T>.Success(data, result.HttpMethodType)
                : Result<T>.Fail(result.Errors, result.HttpMethodType, result.StatusCode); 
    }
}
