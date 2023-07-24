using Microsoft.Toolkit.Uwp.Notifications;
using System.Management;

class Process
{
    public static DateTime StartTime { get; set; }
    public static double RecordedHours { get; set; }
    public static void Main()
    {
        ManagementEventWatcher startWatch = new ManagementEventWatcher(
          new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
        startWatch.EventArrived += new EventArrivedEventHandler(StartWatch_EventArrived);
        startWatch.Start();
        ManagementEventWatcher stopWatch = new ManagementEventWatcher(
          new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
        stopWatch.EventArrived += new EventArrivedEventHandler(StopWatch_EventArrived);
        stopWatch.Start();
        Console.WriteLine("Press ENTER to exit");
        Console.ReadLine();
        startWatch.Stop();
        stopWatch.Stop();
    }
    
    //Using Excel for the PoC as I don't have a Proclaim install
    static void StopWatch_EventArrived(object sender, EventArrivedEventArgs e)
    {
        var processName = e.NewEvent.Properties["ProcessName"].Value;
        if ((string)processName == "EXCEL.EXE")
        {
            Console.WriteLine("END TRACK");
            RecordedHours += Math.Abs(Math.Round(StartTime.Subtract(DateTime.Now).TotalSeconds, 2));    
            ShowToast();
        }

        Console.WriteLine("Process stopped: {0}", processName);
    }

    static void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
    {
        var processName = e.NewEvent.Properties["ProcessName"].Value;
        if ((string)processName == "EXCEL.EXE")
        {
            Console.WriteLine("BEGIN TRACK");
            StartTime = DateTime.Now;   
        }
        Console.WriteLine("Process started: {0}", processName);
    }

    static void ShowToast()
    {
        new ToastContentBuilder()
            .AddArgument("action", "viewConversation")
            .AddArgument("conversationId", 9813)
            .AddText($"Paul, you have recorded {RecordedHours} seconds today. Have you recorded all your time?")
            .Show(); 
    }
}