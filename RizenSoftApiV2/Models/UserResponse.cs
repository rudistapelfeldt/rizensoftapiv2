﻿using Swashbuckle.AspNetCore.Annotations;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RizenSoftApiV2.Models
{
	public class UserResponse :BaseResponse
	{
        public List<User> User { get; set; }
	}
}

