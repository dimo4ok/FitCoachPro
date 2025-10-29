using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.Response
{
    public record Error (string Code, string Message) { }

    //public class Errors
    //{
    //    public string Code { get; init; }
    //    public string Message { get; init; }
    //}
}
