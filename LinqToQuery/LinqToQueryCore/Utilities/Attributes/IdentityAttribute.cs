﻿//Author:Riyasat Ali 
// http://www.riytechnologies.com:
//Linkedin: https://www.linkedin.com/in/riyasat-ali/

using System;

namespace LinqToQueryCore.Utilities.Attributes
{
	public class IdentityAttribute : Attribute
	{
		public bool AutoGenerated { get; set; }
	}
}