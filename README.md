# Requirements
## IDE
Unity 2022.3.12f1
## Assets

# How to Use
1. 'Play_Style_A' and 'Play_Style_B' are independent Unity projects. Add and open each of them to the Unity Hub project.
2. Open the 'Prospect' scene in Assets/Scenes.
3. Attach all the scripts in Assets/BiFuzz to 'DogPolyart'.
4. When you attach a script, it becomes a component. In the added component, uncheck 'Grad_init' and 'Grad_local' to disable them.
5. Set the 'Grad_init' and 'Grad_local' components of 'DogPolyart' as shown in the image below.
![Grad_init](README_Images/Grad_init.png)
![Grad_local](README_Images/Grad_local.png)
6. Assign the number of frames the test will run to the variable exeTime in Assets/BiFuzz/ExeTime.cs.
7. Empty Assets/Logs and run the scene. Test results will be saved as a CSV file in Assets/Logs.

# Directory Structure

# Screencast
[URL]("URL")