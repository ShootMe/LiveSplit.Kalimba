# LiveSplit.Kalimba
Autosplitter for the game Kalimba

- Splits are predetermined.
- Starts when choosing Kalimba Journey or Dark Void on the Choose Your Spiritual Path screen.
- Splits happen when starting the next level in the tree.
- Final split is when you bop the bird's head in Jurakanchief for the final time or when you grab the last totem at the end of Dark Void.

## Setting up the autosplitter in LiveSplit
- Go to the [releases](https://github.com/ShootMe/LiveSplit.Kalimba/releases) section in this repository.
- Download the latest LiveSplit.Kalimba.dll and LiveSplit32.exe
- Place the LiveSplit32.exe inside your LiveSplit folder as shown below:

![LiveSplit32](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplit32.png)

- You need to use the LiveSplit32.exe for the autosplitter to work properly with this game.
- Place the LiveSplit.Kalimba.dll inside the Components folder as shown below:

![LiveSplit Kalimba](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitKalimba.png)

- Open LiveSplit32.exe and you should see something like the below:

![LiveSplit Setup](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitInitial.png)

- Right click on LiveSplit and go to edit layout:

![LiveSplit Setup](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitEditLayout.png)

- Add the autosplitter to your layout:

![LiveSplit Setup](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitAddAutosplitter.png)

- Since the autosplitter tracks level times for you as well, you can view them in your splits if you want to.
- Add a new column to your splits in the edit layout panel and setup as follows:

![LiveSplit Setup](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitAddGameTime.png)

- You should see something like this now:
 
![LiveSplit Setup](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitGameAndReal.png)

- The Level Times are stored in your splits under the Game Time section:
 
![LiveSplit Setup](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitGameTimeSplits.png)

- Also apart of the autosplitter is the ability to reset to a new game file when on the main menu.
 
![Kalimba](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/kalimbaMainMenu.png)

- If you start the timer or it is already running/ended when you are on the main menu you will get this:
 
![Kalimba](https://raw.githubusercontent.com/ShootMe/LiveSplit.Kalimba/master/Images/livesplitResetGame.png)

- Choosing yes will reset the game state back to a new file and you can start a new run without ever leaving the game.
- Choosing no will do nothing.
