# Windows Error Reporting for .Net

### Description

This Class provides a simple way to enable/disable Windows Error Reporting (WER), 
we based all in this [guide](https://docs.microsoft.com/en-us/windows/desktop/wer/windows-error-reporting)

### Usage

Is simple to use, **copy and paste this WindowsErrorReporting.cs file** to you project, and call
in your startup of main process, _tipically is Program.cs_.


        static void Main(string[] args)
        {
            /// Will write logs in Execution Directory, but feel free to use custom one
            GemiisYeah.WER.WindowsErrorReporting.Enable(Environment.CurrentDirectory);
            /// Or to custom ImageFile in custom Path
            GemiisYeah.WER.WindowsErrorReporting.Enable("myApp.exe", @"c:\MyDumps", 3, DumpType.Mini);
            


To **disable** it if you need you can use

            GemiisYeah.WER.WindowsErrorReporting.Disable("myApp.exe");
            // Or for Default ImageFile
            GemiisYeah.WER.WindowsErrorReporting.Disable();


