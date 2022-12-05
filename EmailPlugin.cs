using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xScheduleWrapper;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net.Mail;
using System.Net;
using AegisImplicitMail;

namespace EmailPlugin
{
	public class EmailPlugin
	{
		//public event EventHandler<string> SendError;
		//public event EventHandler SendReloadDimensions;


		//bool _emailing = false;
		EmailSettings _settings = new EmailSettings();

		public string _showDir = "";

		public string GetMenuString()
		{
			return "EmailPlugin";
		}
		public string GetWebFolder()
		{
			return "";
		}

		public bool xSchedule_Action(string command, string parameters, string data, out string buffer)
		{
			return xScheduleWrapper.xScheduleWrapper.Do_xSchedule_Action(command, parameters, data, out buffer);
		}

		public bool Load(string showDir)
		{
			_showDir = showDir;
			return true;
		}

		public void Unload()
		{
		}

		public bool HandleWeb(string command, string parameters, string data, string reference, out string response)
		{
			response = "";
			return false;
		}

		public bool Start(string showDir, string url)
		{
			_showDir = showDir;
			ReadSetting();
			return true;
		}

		public void Stop()
		{

		}

		public void WipeSettings()
		{

		}

		public void NotifyStatus(string status)
		{
		}

		/// <summary>
		/// reload settings on event from form window
		/// </summary>
		private void Reload_Setting(object sender, EventArgs e)
		{
			ReadSetting();
		}

		/// <summary>
		/// read the email settings file from the show directory
		/// </summary>
		private bool ReadSetting()
		{
			try
			{
				var path = _showDir + "//EmailPlugin.xml";
				if (!System.IO.File.Exists(path))
				{
					WriteError("EmailPlugin.xml not found in '" + _showDir+ "'");
					return false;
				}
				System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(EmailSettings));
				System.IO.StreamReader file = new System.IO.StreamReader(path);
				_settings = (EmailSettings)reader.Deserialize(file);
				file.Close();
				return true;
			}
			catch (Exception ex)
			{
				WriteError("Throw: '" + ex.Message + "'");
				//MessageBox.Show(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// This function is called for each frame
		/// </summary>
		public void ManipulateBuffer(PixelBuffer buffer)
		{

		}

		public bool FireEvent(string type, string parameters)
		{
			//MessageBox.Show(parameters);
			return true;
		}

		public bool SendCommand(string command, string parameters, out string msg)
		{
			if (command.ToLower() == "sendemail")
			{
				if (SendEmail2(parameters, parameters))
				{
					msg = "Sent Email.";
					return true;
				}
				msg = "Sending Email Failed.";
				return false;
			}
			msg = "Command not support.";
			return false;
		}

		/// <summary>
		/// This Sends the Email, I hope.
		/// </summary>
		bool SendEmail2(string subject, string body)
		{
			if (string.IsNullOrEmpty(_settings.Server) ||
				string.IsNullOrEmpty(_settings.FromEmail) ||
				string.IsNullOrEmpty(_settings.ToEmail) ||
				string.IsNullOrEmpty(_settings.Username) ||
				string.IsNullOrEmpty(_settings.Password))
			{
				WriteError("settings was empty in EmailPlugin.xml: '" + _showDir + "'");
				return false;
			}
			try
			{
				var mymessage = new MimeMailMessage();
				mymessage.From = new MimeMailAddress(_settings.FromEmail);
				mymessage.To.Add(_settings.ToEmail);
				mymessage.Subject = subject;
				mymessage.Body = body;

				//Create Smtp Client
				var mailer = new MimeMailer(_settings.Server, _settings.Port);
				mailer.User = _settings.Username;
				mailer.Password = _settings.Password;
				mailer.Timeout = 5000;
				var secType = _settings.Security.ToLower();
				if (secType.Contains("ssl"))
				{
					mailer.SslType = SslMode.Ssl;
				}
				else if (secType.Contains("tls"))
				{
					mailer.SslType = SslMode.Tls;
				}
				else if (secType.Contains("none") || secType.Contains("no") || secType.Contains("false"))
				{
					mailer.SslType = SslMode.None;
				}
				else
				{
					mailer.SslType = SslMode.Auto;
				}

				mailer.AuthenticationMode = AuthenticationType.Base64;

				if (!mailer.TestConnection())
				{
					WriteError("Failed to connect to : '" + _settings.Server + "'");
				}

				mailer.SendMailAsync(mymessage);
				return true;
				}
			catch (Exception ex)
			{
				WriteError("Throw: '" + ex.Message + "'");
			}
			return false;
		}

		void WriteError(string error)
		{
			try
			{
				var path = _showDir + "//EmailError.txt";
				using (StreamWriter sw = new StreamWriter(path))
				{
					sw.WriteLine(error);
					sw.WriteLine("Server: " + _settings.Server);
					sw.WriteLine("Port: " + _settings.Port.ToString());
					sw.WriteLine("FromEmail: " + _settings.FromEmail);
					sw.WriteLine("ToEmail: " + _settings.ToEmail);
					sw.WriteLine("Username: " + _settings.Username);
					sw.WriteLine("Security: " + _settings.Security);
					//sw.WriteLine("Password: " + _settings.Password);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}