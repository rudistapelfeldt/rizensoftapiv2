using Swashbuckle.AspNetCore.Annotations;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RizenSoftApiV2.Models
{
    public class UserRequest
	{
		public User User { get; set; }
	}
}

