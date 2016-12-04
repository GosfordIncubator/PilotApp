using System;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using System.IO;
using System.Text;

namespace Drone_Controller.Droid
{
	[Activity (Label = "Drone_Controller.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		public static string ip = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var ping = FindViewById<Button>(Resource.Id.ping);
			var takeOff = FindViewById<Button>(Resource.Id.takeOff);
			var land = FindViewById<Button>(Resource.Id.land);

			ip = getIp();
			//var ipChange = FindViewById<Button>(Resource.Id.ip);

			ping.Click += delegate
			{
				try
				{
					byte[] data = new byte[2];
					data[0] = 0;
					data[1] = 0;
					TcpClient client = new TcpClient("192.168.1.2", 8001);
					MyClass.SendData(client.GetStream(), data);
					client.Close();
				}
				catch
				{

				}
			};

			takeOff.Click += delegate
			{
				try
				{
					takeOff.Enabled = false;
					land.Enabled = true;
					byte[] data = new byte[2];
					data[0] = 1;
					data[1] = 1;
					TcpClient client = new TcpClient(ip, 8001);
					MyClass.SendData(client.GetStream(), data);
					client.Close();
				}catch
				{
					
				}
			};

			land.Click += delegate
			{
				try
				{
					land.Enabled = false;
					takeOff.Enabled = true;
					byte[] data = new byte[2];
					data[0] = 2;
					data[1] = 1;
					TcpClient client = new TcpClient("192.168.1.2", 8001);
					MyClass.SendData(client.GetStream(), data);
					client.Close();
				}
				catch
				{

				}
			};

			/*ipChange.Click += delegate
			{
				StartActivity(typeof(Activity1));
			};*/
		}

		static string NetworkGateway()
		{
			string ip = null;

			foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (f.OperationalStatus == OperationalStatus.Up)
				{
					foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
					{
						ip = d.Address.ToString();
					}
				}
			}

			return ip;
		}

		static string getIp()
		{
			string gateway = NetworkGateway();
			string[] array = gateway.Split('.');
			string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + 2;
			return ping_var;
		}
	}
}


