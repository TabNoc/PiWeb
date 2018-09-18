using System;
using System.Threading;
using TabNoc.PiWeb.WateringWebServer.other.Hardware;

namespace TabNoc.PiWeb.WateringWebServer.TempManager
{
	public class TemperatureManager : IDisposable
	{
		private const int TemperatureOkSwitchOffCount = 3;
		private const float TemperatureOkValue = 05f;

		private readonly Timer _timer;
		private bool _heatSourceOn = false;
		private int _temperatureOkCounter = 0;

		public TemperatureManager()
		{
			_timer = new Timer(TimerCallback, null, 0, 1 * 1000);
		}

		private void SwitchOff()
		{
			_heatSourceOn = false;
			RelaisControl2.Deactivate(7, "Temperature_Auto");
		}

		private void SwitchOn()
		{
			_heatSourceOn = true;
			RelaisControl2.Activate(7, false, "Temperature_Auto");
		}

		private void TimerCallback(object state)
		{
			try
			{
				lock (this)
				{
					(DateTime dateTime, float temperature) = TemperaturReader.ReadInternTemperature();
					temperature = temperature / 1000;
					Console.WriteLine(temperature);

					// Switch HeatSourceOn off, if the Temperature was ok for Time x
					if (_temperatureOkCounter > TemperatureOkSwitchOffCount)
					{
						_temperatureOkCounter = -1;
						SwitchOff();
					}

					// Switch HeatSourceOn on, if the Temperature is to low
					if (_heatSourceOn == false && temperature < TemperatureOkValue)
					{
						SwitchOn();
					}

					// count if Temperature is ok and the HeatSource is on
					if (_heatSourceOn == true && temperature > TemperatureOkValue)
					{
						_temperatureOkCounter++;
					}
					else if (_heatSourceOn == true && temperature < TemperatureOkValue)
					{
						_temperatureOkCounter = 0;
					}
				}
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
				throw;
			}
		}

		#region Dispose Pattern

		private bool _disposed;

		~TemperatureManager()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		private void Dispose(bool disposing)
		{
			try
			{
				if (!_disposed)
				{
					_timer.Dispose();
					_disposed = true;
				}
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e);
				Console.ResetColor();
			}
		}

		#endregion Dispose Pattern
	}
}
