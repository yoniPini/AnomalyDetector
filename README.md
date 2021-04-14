# ex1

Flight Inspection App, ex1, Advanced Programming 2, biu

Sections :

			• Introduction
			• Folders structure
			• Pre requirements
			• Installing guide
			• About the example anomaly detections plugins
			• About the csv and xml files
			• Youtube link

			• Developing
			• Further explanation (for developers)

Introduction :

				An app for learn and detect flight records, using graphs, stats info,
				anomaly detection algorithims(as runtime plugins-dll)
				
				• note that the graph not suitable for high speed (larger than 1)

Folders structure :

			ex1/				root folder of the github

			ex1/ex1/			the source code for the project, also incude the icon

			ex1/ex1/dll/		the dll which were made to this project
								and have to be also, but not only, known in compile time.
								see also ex1/plugins/how_created_dll/ for implementation.

			ex1/plugins/		including 2 dll for detection algorithim(and dll that they use),
								read more about it in "Detection plugin" section.
								
			ex1/plugins/how_created_dll/	 how the plugins were made. see also ex1/ex1/dll/
							  were the interface is.

			ex1/uml/			2 uml diagrams
			
			ex1/publish/        installer for user-edge (see "Pre requirements" and "Installing guide")
								all the folder is zipped in    ex1/publish/publish.zip  
								(except to the zip itself, of course) 
								
			ex1/example/		example of xml flight protocol and corresponds csv files (normal and test flight)


Pre requirements :

			• Windows 10
			• Install FG simulator from https://www.flightgear.org/  (under their license, regardless of this project)
			• .net FRAMEWORK 4.7.2
			• if you will use the default plugins you should also make sure that cpp for windows installed on your pc
				I don't know excactly the compoments, but if you install "Desktop development with c++", espacilly the
						"windows 10 sdk"
						"msvc"
						"c++ core desktop features"
				than probably it will be enough.

Installing guide :

			• For user use, install FG, and afterwards copy    ex1/publish/     folder to your pc
			  start the "setup.exe" file
			• You can uninstall the app via control-panel (named "ex1")
			
			• For developers, just fork the github and work on the   ex1/ex1.sln  via visual studio 2019

Detections plugins :

			• The program can load Anomaly detection algorithims, each algorithim in dll file.
			  It can be done via the "Add New Detector" button.
			  The file must be compatible with the interface the progam excpect for.
			  Two algorithims are given in folder plugins :
					• SimpleAnomalyDetection.dll - detecting anomaly of "correlative features" due to linear regression.
					• HybridAnomalyDetection.dll - same as SimpleAnomalyDetection.dll but if there is correlation,
					but smaller, it uses minimal circle.
					The file "anomaly_detector_helper.dll" must be with the same folder of those specific algorithims.

			• !!!!!! Notice that adding Detector is only the first step !!!!!!
			  The second step is to learn the correlative and detect anomalies according,
			  meaning in the program "Add New Detector" button will only add,
			  only choosing("combo box") the added algorithim will apply the learning and detecting.
			  Due to technical issues, the learning using the given 2 dll will take something like 7-15 sec.
			  It producing file ends with ".tmp" which you can delete after the program closed.

			• More word for developers:
					because the two dll depends on unmanged function (anomaly_detector_helper.dll from cpp),
					and because we enabled pInvokeStackImbalance MDA(in "App.config") to support and avoid mistake
					of p/invoke(calling function convention),
					the calling of the Learn and detect functions which was written in cpp are very slow.

					see in developing section below what you need to develop your own plugin
					(do it whole in .net framework, it faster to run)
					Anyway, you can see in  ex1/plugins/how_created_dll/  how the hybrid plugin were written (the second is similar).


csv and xml files :

			• the format of the csv must be float numbers,
				seperated by comma "," and
				"enter"(line feed) in the end of line (including last line)
				All the rows must have the amount of nums("feature" value), as the first line
				The number of "featurs" and their meaning, should be as it is mentioned
				in the valid flight gear protocol xml in the input subtree.
				(see: reg_flight.csv, anomaly_flight.csv, playback_small.xml  all are in  ex1/example/)

			• the hz of the records(lines \ timesteps) is const as 10 HZ
			(every 10 timestep = 1 seconds, therefore speed 5 will go over 50 timesteps in seconds)

Youtube link :

			• Describing how the user(pilot for example) can use in this app
				(hebrew, private link)
				https://youtu.be/xmPmlZ3lWq8

Developing :

			• changes in the code using IDE
				you should work with visual studio 2019,
				which has ".net desktop development" package installed
				(start menu -> search -> visual studio installer -> modify vs2019)

				Notice you may need to build the project so the xaml preview will be ok.

				Also, binding failure due to exception of not exist key in dictionary TableListener.FeaturesValue
				is ok since it's when there is no data about it...

				Try not to move the joystick(small circle in the bigger) since its margin  property is bound
				and when moving it within the xaml designer view-> the binding removed.

			• we use NuGet Packages : OxyPlot.Core ; OxyPlot.Wpf ; System.Diagnostics.Process

			• we use asm ref to : System.ComponentModel ; IAnomalyDetector.dll ; IOxyPlotDrawable.dll ;
				("plugin of anomaly detection algorithim" are dlls which loaded RUNTIME from user)

			• About multi-threading
				right now- only few thread are created by the program and only 1 other child process(at time) (the fg simulator)
				you should avoid thread as much as you can, since it might cause collision
				\ trying to operate on invalid values(e.g. null pointer exception)
				the program use property in some cases since the TextBox for example, dont allow access from other thread.

			• avoid infinty loop of events
				espacilly the mechanism of IsRunning / IsPaused

			• plugin
				you can make your own plugin, each plugin should be exactly :
				a .net framework 4.7.2 which becomes dll, public class named AnomalyDetector, in namespace DLL,
				have a default / constructor with no params, which implements the IAnomalyDetector.dll

				The correlative shape that describes the algorithm(e.g line for linear regression)
				should be in the member objectOfCorrelation in the returned CorrelatedFeatures(see the interface)
				For DLL.Line and DLL.Circle from "IAnomalyDetector.dll"
				there is a default implemention how to draw them in the graph of correltion,
				However, if you want your implementation\ want other object to be drawn on the graph,
				the object must be of type IOxyPlotDrawable from "IOxyPlotDrawable.dll",
				a type that returns landmarks of pulling a pen that make the outer line of the shape.

				Both interfaces are in  ex1/ex1/dll/

Further explanation :

			• Basiclly, there are view models in the MainWindow (one view for UI)
			(1 for selecting feature, 3 for each graph, 1 for stats, 1 for joysick, 1 for the player and the path details)
			those view-models gives commands and subsribed to event of model,
			which subscribed to the center, the "engine" of this program = the FlightGearPlayerModel class which raises events
			(changing the "current time" = current time step in the TableSeries loaded from the csv) using a member of timer

			• 2 Uml diagrams in  ex1/uml/
