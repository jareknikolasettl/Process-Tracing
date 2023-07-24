using Microsoft.Toolkit.Uwp.Notifications;
using System.Management;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

class Process
{
    public static DateTime StartTime { get; set; }
    public static double RecordedHours { get; set; }
    public static void Main()
    {
        ManagementEventWatcher startWatch = new ManagementEventWatcher(
          new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
        startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
        startWatch.Start();
        ManagementEventWatcher stopWatch = new ManagementEventWatcher(
          new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
        stopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
        stopWatch.Start();
        Console.WriteLine("Press ENTER to exit");
        Console.ReadLine();
        startWatch.Stop();
        stopWatch.Stop();
    }
    
    //Using Excel for the PoC as I don't have a Proclaim install
    static void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
    {
        var processName = e.NewEvent.Properties["ProcessName"].Value;
        if ((string)processName == "EXCEL.EXE")
        {
            Console.WriteLine("END TRACK");
            //Log Time
            RecordedHours = Math.Abs(Math.Round(StartTime.Subtract(DateTime.Now).TotalSeconds, 2));    
            //Trigger popup: "Paul, you have recorded NNN units of time today. Have you recorded all your time?"
            ShowToast();
        }

        Console.WriteLine("Process stopped: {0}", processName);
    }

    static void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
    {
        var processName = e.NewEvent.Properties["ProcessName"].Value;
        if ((string)processName == "EXCEL.EXE")
        {
            Console.WriteLine("BEGIN TRACK");
            //Log Time
            StartTime = DateTime.Now;   
        }
        Console.WriteLine("Process started: {0}", processName);
    }

    static void ShowToast()
    {
        // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText($"Paul, you have recorded {RecordedHours} seconds today. Have you recorded all your time?")
            .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
    }
}