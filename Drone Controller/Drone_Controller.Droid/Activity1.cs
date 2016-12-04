using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Drone_Controller.Droid
{
	[Activity(Label = "Activity1")]
	public class Activity1 : Activity
	{
		public List<string> ips = new List<string>();
		public List<string> ids = new List<string>();

		public string selectedIp = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{

			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Activity1);

			Ping_all();

			Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);
			spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, ids);

			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			var confirm = FindViewById<Button>(Resource.Id.confirm);

			confirm.Click += delegate
			{
				var main = new Intent(this, typeof(MainActivity));
				main.PutExtra("Ip", selectedIp);
				StartActivity(main);
			};

		}

		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

			string toast = string.Format("You selected {0}", spinner.GetItemAtPosition(e.Position));
			selectedIp = ips[e.Position];
			Toast.MakeText(this, toast, ToastLength.Long).Show();
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

		public void Ping_all()
		{

			string gate_ip = NetworkGateway();

			string[] array = gate_ip.Split('.');

			for (int i = 2; i <= 255; i++)
			{

				string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;

				Ping(ping_var, 4, 1000);

			}

		}

		public void Ping(string host, int attempts, int timeout)
		{
			for (int i = 0; i < attempts; i++)
			{
				new Thread(delegate ()
				{
					try
					{
						Ping ping = new Ping();
						ping.PingCompleted += new PingCompletedEventHandler(PingCompleted);
						ping.SendAsync(host, timeout, host);
					}
					catch
					{
					}
				}).Start();
			}
		}

		private void PingCompleted(object sender, PingCompletedEventArgs e)
		{
			string ip = (string)e.UserState;
			if (e.Reply != null && e.Reply.Status == IPStatus.Success)
			{
				string hostname = GetHostName(ip);
				string[] arr = new string[3];

				arr[0] = ip;
				arr[1] = hostname;

				if(!containsValue(ips, arr[0]))
				{
					ips.Add(arr[0]);
					ids.Add(arr[1]);
				}

			}
		}

		public string GetHostName(string ipAddress)
		{
			try
			{
				IPHostEntry entry = Dns.GetHostEntry(ipAddress);
				if (entry != null)
				{
					return entry.HostName;
				}
			}
			catch (SocketException)
			{
			}

			return null;
		}

		private Boolean containsValue(List<string> ips, string ip)
		{
			Boolean inList = false;
			for(int i = 0; i < ips.Count; i++)
			{
				if(ips[i] == ip)
				{
					inList = true;
					break;
				}
			}
			return inList;
		}
	}
}