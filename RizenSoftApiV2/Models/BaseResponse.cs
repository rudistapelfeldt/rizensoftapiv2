using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RizenSoftApiV2.Models
{
    public abstract class BaseResponse
	{
        public bool Success { get; set; }

        public string? ErrorCode { get; set; }

        public string? Error { get; set; }
    }
}

