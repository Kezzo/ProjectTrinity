using System;
using System.Net;
using System.Threading.Tasks;
using GuerrillaNtp;
using UnityEngine;

public class NetworkTimeService 
{
	private TimeSpan offset;
	private readonly DateTime UtcStartDateTime = new DateTime(1970, 1, 1);

	public DateTime NetworkDateTime 
	{
		get 
		{
			return DateTime.UtcNow + offset;
		}
	}

	public long NetworkTimestamp
	{
		get
		{
			return (long) NetworkDateTime.Subtract(UtcStartDateTime).TotalSeconds;
		}
	}

	public long NetworkTimestampMs
	{
		get
		{
			return (long) NetworkDateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
		}
	}

	public void Synch (IPAddress ip, int port = 123, Action onTimeSynched = null) 
	{
		Task.Run (() => {
			TimeSpan offsetBuffer = new TimeSpan();
			
			using (var ntp = new NtpClient(ip, port)) {
				// doing it three times for more accurate result.
				for (int i = 0; i < 3; i++)
				{
					offsetBuffer += ntp.GetCorrectionOffset();
				}
			}

			offset = TimeSpan.FromMilliseconds(offsetBuffer.TotalMilliseconds / 3);
			Debug.LogFormat("Server synched time is: {0} offset was: {1}", NetworkDateTime.ToString(), offset.ToString());

			if(onTimeSynched != null) 
			{
				onTimeSynched();
			}
		});
	}

	public void Synch (string synchHost, Action onTimeSynched = null) 
	{
		Synch(Dns.GetHostAddresses(synchHost)[0], 123, onTimeSynched);
	}
}
