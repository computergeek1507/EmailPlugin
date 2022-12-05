using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailPlugin
{
	public class EmailSettings
	{
		public string FromEmail { get; set; } = "";
		public string ToEmail { get; set; } = "";
		public int Port { get; set; } = 587;
		public string Username { get; set; } = "";
		public string Password { get; set; } = "";
		public string Server { get; set; } = "";
		public string Security { get; set; } = "none";
		public bool UseTLS { get; set; } = true;
	}
}
