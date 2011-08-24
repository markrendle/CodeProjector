#Code Projector

Whenever I'm doing presentations with live coding, I always struggle with the amount of screen real-estate that's taken up on the 1024x768 projector by the VS2010 toolbars and tool windows. What I need is something that will just take the important part - the actual code - and display that on a projector, while I have the full Visual Studio on my laptop screen. So I made this.

It was incredibly easy to write, since Visual Studio is a WPF application. I just Snooped a running instance to find that the document well is contained within a control named "MainDockTarget". In my plug-in code, I just searched the entire Visual Tree from Application.Current.MainWindow downwards looking for that control, then used it as the source for a VisualBrush. Set that brush as the background of a Window, add a check-box to make that Window full-screen, and done.

I have no intention of releasing this as an extension which can be installed through Extension Manager, because I don't need the hassle of supporting it, but if anybody wants to fork it and release it like that, fork away!

MIT License
