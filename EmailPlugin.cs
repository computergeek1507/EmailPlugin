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

namespace EmailPlugin
{
	public class EmailPlugin
	{
		//public event EventHandler<string> SendError;
		//public event EventHandler SendReloadDimensions;


		//bool _emailing = false;
		EmailSettings _settings;

		//int _pictureWidth = 0;
		//int _pictureHeight = 0;
		//int _frame = 0;

		public string _showDir = "";

		public string GetMenuString()
		{
			return "Email Plugin";
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
				MessageBox.Show(ex.Message);
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
				if (SendEmail(parameters, parameters))
				{
					msg = "Sending email.";
					return true;
				}
				msg = "Sending email Failed.";
				return false;
			}
			msg = "Command not support.";
			return false;
		}

		/// <summary>
		/// This Sends the Email, I hope.
		/// </summary>
		bool SendEmail(string subject, string body)
		{
			if (string.IsNullOrEmpty(_settings.Server) ||
				string.IsNullOrEmpty(_settings.FromEmail) ||
				string.IsNullOrEmpty(_settings.ToEmail) ||
				string.IsNullOrEmpty(_settings.Username) ||
				string.IsNullOrEmpty(_settings.Password))
			{
				return false;
			}
				//if (_emailing)
				//	return false;
				//_emailing = true;
			try
			{
				var smtpClient = new SmtpClient(_settings.Server)
				{
					Port = _settings.Port,
					Credentials = new NetworkCredential(_settings.Username, _settings.Password),
					EnableSsl = _settings.UseSSL,
				};

				smtpClient.Send(_settings.FromEmail, _settings.ToEmail, subject, body);
				return true;
			}
			catch (Exception )
			{

			}
			return false;
			//_emailing = false;
		}
	}
}