# Training-Environment-Carcassonne
Training environment for a Carcassonne AI. The training environment contains two scenes, one for training the AI using nine separate playing fields, which is what should primarily be used. And another scene that uses only
one playing field, which could be useful for getting a closer look at exactly how the training is progressing.

![This is an image](https://cdn.discordapp.com/attachments/886890473715826708/918445023253630986/unknown.png) ![This is an image](https://cdn.discordapp.com/attachments/886890473715826708/918445878803578900/unknown.png)

## Installation
### ML-Agents Toolkit
In order to use the Training Environment the user needs to download the ML-Agents Toolkit. This project has been tested with release 18(https://github.com/Unity-Technologies/ml-agents).
And it is recommended to follow their documentation in order to get it installed properly. NOTE: it is important that the ml-agents-release_18 folder is downloaded and placed in
a convenient location as it will be used during training. 

### Anaconda
Another program we would recommend for using the Training Environment is Anaconda(https://www.anaconda.com/products/individual). We used Anaconda with Python version 3.8.8. It is also possible
to run the program from most other terminals. 

### Unity 
This project has been produced and tested using Unity Version: 2020.3.19f1.
## Training
### Prerequisites
Before the Step by Step Guide is carried out make sure that the following things are done:
- Drag the Carcassonne.yaml file from the `AIConfig` folder of this project into this directory: `\ml-agents-release_18\config\ppo`
- Make sure that the AI Prefab within the Prefabs folder of this project has the Behavior Name "Carcassonne", this can be checked using the inspector in Unity. 
### Step by Step Guide
1. Open the Unity Project.
2. Launch Anaconda Prompt.
3. Type in the following command to create a virtual environment for the project:
```
conda create -n ml-agents python 3.8
```
4. Type in the following command to enter the virtual environment:
```
conda activate ml-agents
```
5. Type in the following command to cd into the ml-agents folder:
```
cd C:/USERS/YOURUSERNAME/Downloads/ml-agents-release_18
```
6. Enter the following command to initiate the training, replace NAMEOFTHERUN with your desired name for the training session:
```
mlagents-learn config/ppo/Carcassonne.yaml --run-id NAMEOFTHERUN
```
7. Open up Unity and press play to begin the training. 
8. The training will stop after the the agent has carried out the maximum number of steps defined in the .yaml file, if you wish to stop the training for any reason, simply press
the play button again in Unity. 
