# Process-Tracing
This a PoC for sending toast notifications from a c# app informing the user on the amount of time spent on Proclaim.
I used Excel for the PoC as I don't have an install of Proclaim.

To test this app yourself:
1. Run VS as administrator
2. Debug app
3. Open Excel for a few seconds
4. Close Excel
5. You should see a toast notification with the amount of time spent on Excel

The app aggregates time spent in Excel across multiple sessions. Currently this is only reset once app is stopped.
